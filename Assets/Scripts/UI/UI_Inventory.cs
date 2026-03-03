using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [Header("UI Slots")]
    // Editörden 4 tane slotun Image componentlerini buraya sürükleyeceğiz
    [SerializeField] private Image[] slotImages;

    // Geçici olarak içi boş (saydam) gözükmesi için boş slot rengi
    private Color emptyColor = new Color(1, 1, 1, 0);
    private Color fullColor = new Color(1, 1, 1, 1);

    private PlayerInventory localInventory;

    private void Start()
    {
        // Oyun başlarken tüm slotları gizle
        foreach (var img in slotImages)
        {
            img.color = emptyColor;
            img.sprite = null;
        }
    }

    // Karakter doğduğunda bu fonksiyon çağrılacak (UI ile Karakteri bağlıyoruz)
    public void Initialize(PlayerInventory inventory)
    {
        localInventory = inventory;
        localInventory.OnInventoryChanged += UpdateSlotUI;
        localInventory.OnSlotSelected += HighlightSlot;
    }

    private void OnDestroy()
    {
        if (localInventory != null)
        {
            localInventory.OnInventoryChanged -= UpdateSlotUI;
            localInventory.OnSlotSelected -= HighlightSlot; // YENİ: Aboneliği iptal et
        }
    }

    // Event tetiklendiğinde çalışan fonsiyon
    private void UpdateSlotUI(int slotIndex, ItemData itemData)
    {
        if (itemData != null)
        {
            slotImages[slotIndex].sprite = itemData.itemIcon; // Resmi değiştir
            slotImages[slotIndex].color = fullColor; // Görünür yap
        }
        else
        {
            slotImages[slotIndex].sprite = null;
            slotImages[slotIndex].color = emptyColor; // Gizle
        }
    }

    private void HighlightSlot(int selectedIndex, ItemData selectedItem)
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i == selectedIndex)
            {
                // Seçili olan slotun boyutunu %15 büyüt (İstersen burada rengini de değiştirebilirsin)
                slotImages[i].rectTransform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
            }
            else
            {
                // Seçili olmayanları veya iptal edilenleri normal boyutuna döndür
                slotImages[i].rectTransform.localScale = Vector3.one;
            }
        }
    }
}
