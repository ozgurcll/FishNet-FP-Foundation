using UnityEngine;
using FishNet.Object;
using System;
using System.Collections;

public class PlayerStamina : NetworkBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float depleteRate = 15f;  // Saniyede ne kadar azalacak?
    public float regenRate = 10f;    // Saniyede ne kadar dolacak?
    public float regenDelay = 1f;    // Koşmayı bıraktıktan kaç saniye sonra dolmaya başlasın?

    private float currentStamina;
    private Coroutine staminaCoroutine;

    // UI'ı tetikleyecek Event (Mevcut, Maksimum)
    public event Action<float, float> OnStaminaChanged;

    // Nefes tamamen bittiğinde Player scriptini uyaracak Event
    public event Action OnStaminaExhausted;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            this.enabled = false;
            return;
        }

        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    // Player.cs koşmaya başladığında bu metodu çağıracak
    public void StartDepleting()
    {
        if (!base.IsOwner) return;

        if (staminaCoroutine != null) StopCoroutine(staminaCoroutine);
        staminaCoroutine = StartCoroutine(DepleteRoutine());
    }

    // Player.cs durduğunda veya yürümeye geçtiğinde bu metodu çağıracak
    public void StartRegenerating()
    {
        if (!base.IsOwner) return;

        if (staminaCoroutine != null) StopCoroutine(staminaCoroutine);
        staminaCoroutine = StartCoroutine(RegenRoutine());
    }

    // Stamina kontrolü (Koşmaya yetecek nefes var mı?)
    public bool CanSprint() => currentStamina > 0;

    // --- SIFIR UPDATE COROUTINE SİSTEMLERİ ---

    private IEnumerator DepleteRoutine()
    {
        while (currentStamina > 0)
        {
            currentStamina -= depleteRate * Time.deltaTime;
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);

            // Stamina sıfırlandığında
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                OnStaminaExhausted?.Invoke(); // Karakteri yürüme durumuna zorla
                StartRegenerating();          // Dolum rutinine geç
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator RegenRoutine()
    {
        // Nefes nefese kalma payı (Hemen dolmasın)
        yield return new WaitForSeconds(regenDelay);

        while (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            yield return null;
        }

        // Stamina tam dolduğunda değerleri sabitle ve DÖNGÜYÜ BİTİR! (CPU tasarrufu)
        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }
}