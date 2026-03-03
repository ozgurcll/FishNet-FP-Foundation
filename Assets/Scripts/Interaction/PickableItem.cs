using UnityEngine;
using FishNet.Object;
using FishNet.Component.Transforming;

[RequireComponent(typeof(Outline))]
public class PickableItem : NetworkBehaviour, IInteractable
{
    public ItemData itemData;
    private Outline outline;

    [Header("Equip Settings (Elde Duruş)")]
    public Vector3 holdPosition; // Eşyanın eldeki konumu (Örn: kılıç avuç içinde dursun)
    public Vector3 holdRotation; // Eşyanın eldeki açısı

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public string GetInteractPrompt()
    {
        return "Envantere Ekle";
    }

    public void OnFocus()
    {
        outline.enabled = true;
    }

    public void OnLoseFocus()
    {
        outline.enabled = false;
    }

    public void OnInteract(GameObject interactorPlayer)
    {
        if (interactorPlayer.TryGetComponent(out PlayerInventory inventory) &&
             interactorPlayer.TryGetComponent(out PlayerEquipment equipment))
        {
            // Eğer çanta dolu değilse ve eşya başarıyla eklendiyse
            if (inventory.AddItem(itemData))
            {
                outline.enabled = false;

                // YENİ: Objeyi silmek yerine Server'a "Bu eşyayı elime taşı" emri yolla
                PickupServerRpc(equipment.NetworkObject);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickupServerRpc(NetworkObject playerNetObj)
    {
        // 1. Çok Önemli: Yerdeki eşyanın ağ üzerindeki sahipliğini (Owner) alan kişiye veriyoruz!
        base.NetworkObject.GiveOwnership(playerNetObj.Owner);

        // 2. Tüm oyunculara bu eşyanın o kişinin eline geçtiğini haber veriyoruz.
        PickupObserversRpc(playerNetObj);
    }

    [ObserversRpc]
    private void PickupObserversRpc(NetworkObject playerNetObj)
    {
        PlayerEquipment equipment = playerNetObj.GetComponent<PlayerEquipment>();

        if (TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
        if (TryGetComponent(out Collider col)) col.enabled = false;

        if (TryGetComponent(out NetworkTransform netTransform)) netTransform.enabled = false;

        transform.SetParent(equipment.GetRightHand());

        transform.localPosition = holdPosition;
        transform.localRotation = Quaternion.Euler(holdRotation);

        gameObject.name = itemData.itemName;

        gameObject.SetActive(false);
    }



    [ServerRpc]
    public void DropServerRpc(Vector3 dropPosition, Vector3 dropDirection)
    {
        // 1. Eşyanın sahipliğini oyuncudan alıp tekrar Sunucuya (Server) veriyoruz
        base.NetworkObject.RemoveOwnership();

        // 2. Herkese "Bu eşyayı yere atın" diyoruz
        DropObserversRpc(dropPosition, dropDirection);
    }

    [ObserversRpc]
    private void DropObserversRpc(Vector3 dropPosition, Vector3 dropDirection)
    {
        // 1. Eşyayı elden çıkar (Bağımsız yap)
        transform.SetParent(null);
        transform.position = dropPosition;

        // 2. Görünür yap
        gameObject.SetActive(true);

        // 3. FİZİKLERİ GERİ AÇ VE FIRLAT
        if (TryGetComponent(out Collider col)) col.enabled = true;
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            // Eşyayı kameranın baktığı yöne doğru hafifçe fırlat (5f gücünde)
            rb.AddForce(dropDirection * 5f, ForceMode.Impulse);
        }

        // 4. NETWORK TRANSFORMU GERİ AÇ (Artık FishNet senkronize etmeye devam etsin)
        if (TryGetComponent(out FishNet.Component.Transforming.NetworkTransform netTransform))
            netTransform.enabled = true;
    }
}