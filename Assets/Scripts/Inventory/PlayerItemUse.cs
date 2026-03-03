using UnityEngine;
using FishNet.Object;

public class PlayerItemUse : NetworkBehaviour
{
    [SerializeField] private PlayerEquipment equipment;

    // Player.cs üzerinden Sol Tık (Use) tuşuna basıldığında tetiklenecek
    public void TryUseCurrentItem()
    {
        if (!base.IsOwner) return;

        // PlayerEquipment scriptimizden Sağ Eli (RightHand) al
        Transform rightHand = equipment.GetRightHand();

        // Sağ elin içindeki aktif (görünür) olan eşyayı bul
        foreach (Transform child in rightHand)
        {
            // Eğer çocuk obje aktifse ve üzerinde IUsable kimliği (Örn: FlashlightItem) varsa
            if (child.gameObject.activeSelf && child.TryGetComponent(out IUsable usableItem))
            {
                usableItem.UseItem(); // Eşyayı kullan!
                break; // İşlem tamam, döngüden çık
            }
        }
    }
}