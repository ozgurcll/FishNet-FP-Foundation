using System;
using UnityEngine;
using FishNet.Object;

public class PlayerInventory : NetworkBehaviour
{
    public ItemData[] slots = new ItemData[4];

    public event Action<int, ItemData> OnSlotSelected;
    public event Action<int, ItemData> OnInventoryChanged;
    
    private int currentSelectedSlot = -1;
    public int GetCurrentSlot() => currentSelectedSlot;

    public bool AddItem(ItemData itemToAdd)
    {
        // Eğer bu karakter bizim değilse işlem yapma
        if (!base.IsOwner) return false;

        // 4 slotu tek tek kontrol et
        for (int i = 0; i < slots.Length; i++)
        {
            // Eğer slot boşsa
            if (slots[i] == null)
            {
                slots[i] = itemToAdd; // Eşyayı slota koy

                // UI'a haber ver: "Şu slota şu eşya geldi!"
                OnInventoryChanged?.Invoke(i, itemToAdd);

                return true; // Eşya başarıyla alındı
            }
        }

        // Eğer döngü biterse ve boş yer bulamazsa çanta doludur
        Debug.Log("Envanter dolu!");
        return false;
    }

    public void SelectSlot(int slotIndex)
    {
        if (!base.IsOwner) return;

        // Kural 1: Eğer zaten seçili olan tuşa tekrar bastıysak, elleri boşalt (Gizle)
        if (currentSelectedSlot == slotIndex)
        {
            currentSelectedSlot = -1;
            OnSlotSelected?.Invoke(-1, null); // -1 ve null göndererek elleri boşalt diyoruz
        }
        // Kural 2: Farklı bir tuşa bastıysak ve o slotta gerçekten bir eşya varsa onu kuşan
        else if (slots[slotIndex] != null)
        {
            currentSelectedSlot = slotIndex;
            OnSlotSelected?.Invoke(currentSelectedSlot, slots[slotIndex]);
        }
    }

    public void RemoveItem(int slotIndex)
    {
        if (!base.IsOwner || slotIndex < 0 || slotIndex >= slots.Length) return;

        slots[slotIndex] = null; // Veriyi sil
        OnInventoryChanged?.Invoke(slotIndex, null); // UI slotunu boşalt

        SelectSlot(slotIndex); // Seçimi güncelle (Eşya gittiği için eller otomatik boşalacak)
    }
}