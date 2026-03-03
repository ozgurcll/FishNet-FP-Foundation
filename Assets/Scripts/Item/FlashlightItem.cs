using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class FlashlightItem : NetworkBehaviour, IUsable
{
    [Header("Flashlight Settings")]
    [SerializeField] private Light spotLight; // Fenerin Işık objesi
    [SerializeField] private AudioClip clickSound; // Tık sesi (İsteğe bağlı)
    private AudioSource audioSource;

    // YENİ SİSTEM FISHNET: Işığın açık/kapalı durumu. 
    // Değiştiği an OnLightStateChanged metodunu tetikler.
    public readonly SyncVar<bool> isLightOn = new SyncVar<bool>();

    private void Awake()
    {
        // audioSource = GetComponent<AudioSource>();
        isLightOn.OnChange += OnLightStateChanged;
    }

    private void OnDestroy()
    {
        isLightOn.OnChange -= OnLightStateChanged;
    }

    // Karakter sol tık yaptığında IUsable arayüzü sayesinde bu metot tetiklenir
    public void UseItem()
    {
        ToggleLightServerRpc();
    }

    // Bu komut Sunucuya (Server) gider ve ışık durumunu tersine çevirir (Açıksa kapat, kapalıysa aç)
    [ServerRpc(RequireOwnership = false)]
    private void ToggleLightServerRpc()
    {
        isLightOn.Value = !isLightOn.Value;
    }

    // SyncVar değiştiğinde (0 Update) tüm oyuncularda otomatik çalışan metot
    private void OnLightStateChanged(bool oldState, bool newState, bool asServer)
    {
        if (spotLight != null) 
            spotLight.enabled = newState;
            
        if (audioSource != null && clickSound != null) 
            audioSource.PlayOneShot(clickSound); // Açma/Kapama sesini çal
    }
}