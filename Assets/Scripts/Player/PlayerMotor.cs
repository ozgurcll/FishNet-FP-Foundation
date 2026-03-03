using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    private bool isSprinting = false;

    [Header("Jump & Ground Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float airControlMultiplier = 0.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;


    [Header("Crouch Settings")]
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchHeight = 1f;       // Eğilinceki kapsül boyu
    [SerializeField] private float standingHeight = 2f;     // Normal kapsül
    private bool isCrouching = false;

    [Header("Crouch References")]
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private Transform cameraRoot;          // Kameranın bağlı olduğu kafa objesi
    [SerializeField] private float crouchCameraY = 0.25f;    // Eğilince kameranın Y eksenindeki yüksekliği
    [SerializeField] private float standingCameraY = .5f;  // Normal kamera yüksekliği


    private Vector3 moveInput;
    private Vector3 targetVelocity;

    private float currentSpeed; // O anki hızımızı tutacak

    private Rigidbody rb;

    private Transform mainCameraTransform;

    void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = walkSpeed;

    }

    public void SetSprint(bool isSprinting)
    {
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
    }

    public void SetCrouch(bool _isCrouching)
    {
        isCrouching = _isCrouching;
        UpdateSpeed();

        if (isCrouching)
        {
            // Kapsülün boyunu kısalt ve merkezini aşağı çek (Havada asılı kalmaması için)
            playerCollider.height = crouchHeight;
            // playerCollider.center = new Vector3(0, crouchHeight / 2f, 0);

            // Kamerayı (CameraRoot) aşağı indir
            cameraRoot.localPosition = new Vector3(cameraRoot.localPosition.x, crouchCameraY, cameraRoot.localPosition.z);
        }
        else
        {
            // Kapsülü ve kamerayı eski haline getir
            playerCollider.height = standingHeight;
            // playerCollider.center = new Vector3(0, standingHeight / 2f, 0);
            cameraRoot.localPosition = new Vector3(cameraRoot.localPosition.x, standingCameraY, cameraRoot.localPosition.z);
        }
    }

    public void Move(Vector2 input)
    {
        if (mainCameraTransform == null)
        {
            if (Camera.main != null) mainCameraTransform = Camera.main.transform;
            else return;
        }

        // 1. Kameranın yön vektörlerini al
        Vector3 forward = mainCameraTransform.forward;
        Vector3 right = mainCameraTransform.right;

        // 2. Y eksenini sıfırla ki karakter havaya veya yere doğru yürümeye çalışmasın
        forward.y = 0f;
        right.y = 0f;

        // 3. Vektörlerin uzunluğunu tekrar 1'e sabitle (Normalize)
        forward.Normalize();
        right.Normalize();

        // 4. Input ile kamera yönlerini çarpıp topla
        moveInput = (forward * input.y + right * input.x).normalized;

        targetVelocity = moveInput * currentSpeed;

        // 5. Mevcut düşüş/zıplama hızını (Yerçekimini) koru
        targetVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = targetVelocity;
    }

    // Karakter durduğunda kaymayı engellemek için
    public void Stop()
    {
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    public void Jump()
    {

        rb.linearVelocity = new Vector3(rb.linearVelocity.x * airControlMultiplier, 0f, rb.linearVelocity.z * airControlMultiplier);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void UpdateSpeed()
    {
        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (isSprinting)
            currentSpeed = sprintSpeed;
        else
            currentSpeed = walkSpeed;
    }
}