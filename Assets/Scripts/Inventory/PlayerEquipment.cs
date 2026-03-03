using UnityEngine;
using FishNet.Object;
using System;
using FishNet.Connection;

public class PlayerEquipment : NetworkBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Transform rightHandTransform; // Karakterin sağ el (veya kamera önü) objesi
    public Transform GetRightHand() => rightHandTransform;

    public event Action<string> OnDropStart; // UI'da yazıyı çıkart ve barı doldur
    public event Action OnDropCancel;        // UI'ı gizle ve iptal et

    private bool isDropping;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            // Çantadaki seçim eventini dinle
            inventory.OnSlotSelected += HandleSlotSelection;
        }
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnSlotSelected -= HandleSlotSelection;
    }

    #region Item Equipment Logic
    private void HandleSlotSelection(int slotIndex, ItemData selectedItem)
    {
        // 1. Durum: Eşya gizlendiyse (2. kez aynı tuşa basıldıysa)
        if (slotIndex == -1 || selectedItem == null)
        {
            // Sunucuya "Elindekileri gizle" komutu gönder ("" boş string yolluyoruz)
            ChangeEquipmentServerRpc("");
        }
        // 2. Durum: Yeni eşya seçildiyse
        else
        {
            // Sunucuya "Bu eşyayı elinde göster" komutu gönder (Eşyanın adını yolluyoruz)
            ChangeEquipmentServerRpc(selectedItem.itemName);
        }
    }

    // --- FISHNET MULTIPLAYER SENKRONİZASYONU ---

    // İstemciden (Bizden) -> Sunucuya (Server) gider
    [ServerRpc]
    private void ChangeEquipmentServerRpc(string itemName)
    {
        // Sunucu da bu bilgiyi alır almaz tüm oyunculara dağıtır
        ChangeEquipmentObserversRpc(itemName);
    }

    // Sunucudan (Server) -> O odadaki TÜM oyunculara (Gözlemcilere) gider
    [ObserversRpc]
    private void ChangeEquipmentObserversRpc(string itemName)
    {
        // Elimizin içindeki tüm obje modellerine bak (Levye, Fener vs.)
        foreach (Transform child in rightHandTransform)
        {
            // Eğer objenin adı, seçtiğimiz eşyanın adıyla aynıysa görünür yap, değilse gizle
            // (Örn: itemName "Levye" ise, eldeki "Levye" isimli obje açılır)
            bool shouldBeVisible = (child.name == itemName);
            child.gameObject.SetActive(shouldBeVisible);
        }
    }
    #endregion


    #region Item Drop Logic
    public void StartDrop()
    {
        // Elimizde bir şey yoksa fırlatma başlatma
        if (!base.IsOwner || inventory.GetCurrentSlot() == -1) return;

        isDropping = true;
        OnDropStart?.Invoke("Eşyayı Bırak"); // UI'a fırlatma yazısını gönder
    }

    public void CancelDrop()
    {
        if (!isDropping) return;

        isDropping = false;
        OnDropCancel?.Invoke(); // UI barını sıfırla
    }

    public void CompleteDrop()
    {
        if (!base.IsOwner || !isDropping) return;

        isDropping = false;
        OnDropCancel?.Invoke(); // İşlem bitti, UI barını kapat

        int currentSlot = inventory.GetCurrentSlot();
        if (currentSlot == -1) return;

        // Elimizin (RightHand) içindeki aktif objeyi bul
        foreach (Transform child in rightHandTransform)
        {
            if (child.gameObject.activeSelf && child.TryGetComponent(out PickableItem item))
            {
                // Çantadan veriyi sil ve UI slotunu boşalt
                inventory.RemoveItem(currentSlot);

                // Eşyanın çıkış noktasını kameranın biraz önü olarak ayarla ki içimize girmesin
                Vector3 dropPos = rightHandTransform.position + transform.forward * 0.5f;

                // Server'a fırlatma emrini gönder
                item.DropServerRpc(dropPos, transform.forward);
                break;
            }
        }
    }

    public void DropEverythingOnDeath()
    {
        if (!base.IsOwner) return;

        // Çantadaki 4 slota da bak
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.slots[i] != null)
            {
                // Elimizdeki tüm 3D modellere bakıp eşleşeni bul
                foreach (Transform child in rightHandTransform)
                {
                    if (child.name == inventory.slots[i].itemName && child.TryGetComponent(out PickableItem item))
                    {
                        // Eşyayı kafamızın önünden rastgele bir yöne fırlat!
                        Vector3 dropPos = transform.position + Vector3.up * 1f; // Yerden biraz yukarıda
                        Vector3 randomDirection = (transform.forward + UnityEngine.Random.insideUnitSphere * 0.5f).normalized;

                        // Server'a fırlatma emrini gönder
                        item.DropServerRpc(dropPos, randomDirection);
                        break;
                    }
                }

                // O slotu çantadan tamamen sil
                inventory.RemoveItem(i);
            }
        }
    }
    #endregion
}