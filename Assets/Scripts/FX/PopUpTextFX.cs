using TMPro;
using UnityEngine;

public class PopUpTextFX : MonoBehaviour
{
    private TextMeshPro myText;
    private Transform mainCamTransform;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float disappearSpeed = 3f; // Yukarı çıkış hızı artışı
    [SerializeField] private float fadeSpeed = 4f;      // Kaybolma hızı

    [Header("Time Settings")]
    [SerializeField] private float lifeTime = 1f;

    private float textTimer;
    private Color startColor;

    void Awake()
    {
        myText = GetComponent<TextMeshPro>();
        startColor = myText.color;
    }

    void Start()
    {
        textTimer = lifeTime;
        if (Camera.main != null)
        {
            mainCamTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // 1. HAREKET: Y ekseninde (yukarı) hareket et (3D için Vector3.up)
        transform.position += Vector3.up * speed * Time.deltaTime;

        // 2. ZAMANLAYICI
        textTimer -= Time.deltaTime;

        if (textTimer < 0)
        {
            // 3. FADE OUT (Solma Efekti)
            float alpha = myText.color.a - (fadeSpeed * Time.deltaTime);
            myText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            // Hızlanarak kaybolsun
            if (myText.color.a < 0.5f)
            {
                speed = disappearSpeed;
            }

            // Tamamen görünmez olunca yok et
            if (myText.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    // 3D DÜNYADA METNİN KAMERAYA BAKMASI İÇİN KRİTİK KISIM
    void LateUpdate()
    {
        if (mainCamTransform != null)
        {
            // Metnin yüzünü kameraya döndür
            transform.LookAt(transform.position + mainCamTransform.forward);
        }
    }
}