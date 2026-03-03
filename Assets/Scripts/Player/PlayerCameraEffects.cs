using System.Collections;
using FishNet.Object;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraEffects : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private CinemachineCamera cam; // Cinemachine 3 Kameran

    [Header("FOV (Görüş Açısı) Settings")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float sprintFOV = 75f;
    [SerializeField] private float transitionSpeed = 8f; // Geçişlerin yumuşaklık hızı

    [Header("Strafe Tilt (Kamera Yatması)")]
    [SerializeField] private float tiltAngle = 2f; // Sağa sola giderken kamera kaç derece yatsın?

    [Header("Headbob (Kafa Sallanması)")]
    // Cinemachine Noise ayarları: Frequency (Hız), Amplitude (Şiddet)
    [SerializeField] private float walkBobSpeed = 1.5f;
    [SerializeField] private float walkBobAmount = 0.5f;
    [SerializeField] private float sprintBobSpeed = 2.5f;
    [SerializeField] private float sprintBobAmount = 1f;

    private CinemachineBasicMultiChannelPerlin noiseComponent;
    private Coroutine effectCoroutine;

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Eğer bu karakter bizim değilse bu scripti tamamen kapat (CPU tasarrufu)
        if (!base.IsOwner)
        {
            this.enabled = false;
            return;
        }

        // Kameradaki Noise (Titreme) bileşenini bul
        noiseComponent = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();

        // Başlangıçta kafa sallanmasını sıfırla
        if (noiseComponent != null)
        {
            noiseComponent.AmplitudeGain = 0f;
            noiseComponent.FrequencyGain = 0f;
        }

        // Player'daki eventleri dinlemeye başla
        player.OnStartWalking += (xInput) => StartEffectTransition(normalFOV, walkBobSpeed, walkBobAmount, -xInput * tiltAngle);
        player.OnStartSprinting += (xInput) => StartEffectTransition(sprintFOV, sprintBobSpeed, sprintBobAmount, -xInput * tiltAngle);
        player.OnStopMoving += () => StartEffectTransition(normalFOV, 0f, 0f, 0f); // Durunca her şeyi sıfırla
    }

    private void OnDestroy()
    {
        // Memory Leak önlemi
        if (player != null)
        {
            player.OnStartWalking -= (xInput) => StartEffectTransition(normalFOV, walkBobSpeed, walkBobAmount, -xInput * tiltAngle);
            player.OnStartSprinting -= (xInput) => StartEffectTransition(sprintFOV, sprintBobSpeed, sprintBobAmount, -xInput * tiltAngle);
            player.OnStopMoving -= () => StartEffectTransition(normalFOV, 0f, 0f, 0f);
        }
    }

    // --- SIFIR UPDATE GEÇİŞ SİSTEMİ ---

    private void StartEffectTransition(float targetFOV, float targetBobFreq, float targetBobAmp, float targetTilt)
    {
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(TransitionRoutine(targetFOV, targetBobFreq, targetBobAmp, targetTilt));
    }

    private IEnumerator TransitionRoutine(float targetFOV, float targetFreq, float targetAmp, float targetTilt)
    {
        // Değerler hedefe yeterince yaklaşana kadar yumuşak geçiş (Lerp) yap
        while (Mathf.Abs(cam.Lens.FieldOfView - targetFOV) > 0.1f ||
               Mathf.Abs(cam.Lens.Dutch - targetTilt) > 0.1f ||
               (noiseComponent != null && Mathf.Abs(noiseComponent.AmplitudeGain - targetAmp) > 0.01f))
        {
            // FOV (Görüş Açısı) Geçişi
            cam.Lens.FieldOfView = Mathf.Lerp(cam.Lens.FieldOfView, targetFOV, Time.deltaTime * transitionSpeed);

            // Tilt (Z ekseni yatması - Dutch) Geçişi
            cam.Lens.Dutch = Mathf.Lerp(cam.Lens.Dutch, targetTilt, Time.deltaTime * transitionSpeed);

            // Headbob Geçişi
            if (noiseComponent != null)
            {
                noiseComponent.FrequencyGain = Mathf.Lerp(noiseComponent.FrequencyGain, targetFreq, Time.deltaTime * transitionSpeed);
                noiseComponent.AmplitudeGain = Mathf.Lerp(noiseComponent.AmplitudeGain, targetAmp, Time.deltaTime * transitionSpeed);
            }

            yield return null; // Bir sonraki kareyi bekle
        }

        // Hedefe ulaşıldığında küsuratları tam sayıya sabitle ve Coroutine'i BİTİR! (Sıfır işlemci yükü)
        cam.Lens.FieldOfView = targetFOV;
        cam.Lens.Dutch = targetTilt;
        if (noiseComponent != null)
        {
            noiseComponent.FrequencyGain = targetFreq;
            noiseComponent.AmplitudeGain = targetAmp;
        }
    }
}
