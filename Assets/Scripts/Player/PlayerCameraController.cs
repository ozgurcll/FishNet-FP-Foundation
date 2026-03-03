using FishNet.Object;
using FishNet.Managing.Timing;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private GameObject cinemachineCameraObj;

    private Camera mainCamera;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
        {
            cinemachineCameraObj.SetActive(false);
            this.enabled = false;
            return;
        }

        cinemachineCameraObj.SetActive(true);
        mainCamera = Camera.main;

        // FishNet'in tick sistemine subscribe ol
        base.TimeManager.OnTick += OnTick;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (base.IsOwner && base.TimeManager != null)
            base.TimeManager.OnTick -= OnTick;
    }

    private void OnTick()
    {
        if (!base.IsOwner || mainCamera == null) return;

        float cameraYaw = mainCamera.transform.eulerAngles.y;
        Quaternion newRotation = Quaternion.Euler(0f, cameraYaw, 0f);
        player.rb.MoveRotation(newRotation);
    }
}