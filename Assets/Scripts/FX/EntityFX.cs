using FishNet.Object;
using TMPro;
using UnityEngine;

public class EntityFX : NetworkBehaviour
{
    [Header("Pop Up Config")]
    [SerializeField] private GameObject popUpTextPrefab;
    [SerializeField] private float heightOffset = 2.5f;
    [SerializeField] private float randomizePos = 0.5f;

   public void CreatePopUpText(string _text)
    {
        // Eğer Server isek, tüm Client'lara "Yazıyı Göster" emri ver
        if (base.IsServerInitialized)
        {
            SpawnPopUpObserverRpc(_text);
        }
        // Eğer Client isek ama Server değilsek (Host değilsek), 
        // Sunucudan rica etmemiz gerekebilir ama görsel efektler genelde 
        // sunucu otoritesinde tetiklenir (Örn: Can azalması sunucuda olur).
        // Eğer Client-side prediction kullanıyorsan ve anında görmek istiyorsan:
        else if (base.IsClientInitialized)
        {
            // Sadece kendi ekranımda göster (Lokal)
            SpawnTextLokal(_text); 
        }
    }

    // [ObserversRpc]: Bu fonksiyon Server tarafından çağrılır, 
    // ama Kodu o objeyi gören TÜM OYUNCULARIN bilgisayarında çalıştırır.
    [ObserversRpc]
    private void SpawnPopUpObserverRpc(string _text)
    {
        SpawnTextLokal(_text);
    }

    // Asıl Instantiate işlemini yapan fonksiyon (Lokal çalışır)
    private void SpawnTextLokal(string _text)
    {
        if (popUpTextPrefab == null) return;

        float randomX = Random.Range(-randomizePos, randomizePos);
        Vector3 positionOffset = new Vector3(randomX, heightOffset, 0);

        // Standart Instantiate kullanıyoruz çünkü bu obje NetworkObject olmak zorunda değil.
        // Sadece görsel bir efekt (Particle gibi), senkronize edilmesine gerek yok.
        // Herkes kendi bilgisayarında ayrı ayrı yaratsın yeter.
        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity);

        TextMeshPro tmPro = newText.GetComponent<TextMeshPro>();
        if (tmPro != null)
        {
            tmPro.text = _text;
        }
    }
}
