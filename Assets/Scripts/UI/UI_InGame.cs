using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private PlayerStamina localPlayerStamina;

    [Header("UI References")]
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        PlayerHealth.OnLocalHealthChanged += UpdateHealthText;
    }

    public void Initialize(PlayerStamina playerStamina)
    {
        localPlayerStamina = playerStamina;
        localPlayerStamina.OnStaminaChanged += UpdateStaminaBar;
    }

    private void OnDestroy()
    {
        if (localPlayerStamina != null)
        {
            localPlayerStamina.OnStaminaChanged -= UpdateStaminaBar;
        }

        PlayerHealth.OnLocalHealthChanged -= UpdateHealthText;
    }

    // --- STAMINA UI ---
    private void UpdateStaminaBar(float current, float max)
    {
        if (staminaFillImage != null)
            staminaFillImage.fillAmount = current / max;
    }

    private void UpdateHealthText(int current, int max)
    {
        if (healthText == null) return;

        // Yazıyı "HP: 100 / 100" veya sadece "100 / 100" formatında güncelle
        healthText.text = $"HP: {current} / {max}";

        // TASARIM DOKUNUŞU: Can 25 ve altındaysa kırmızı yap, yoksa yeşil kalsın
        if (current <= 25)
            healthText.color = Color.red;
        else
            healthText.color = Color.green;
    }
}