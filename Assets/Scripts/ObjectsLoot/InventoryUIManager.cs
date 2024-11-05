using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Inventory UI Elements")]
    public GameObject inventoryPanel;
    public Transform inventoryGrid;
    public GameObject itemSlotPrefab;

    [Header("Equipment Slots")]
    public Image helmetSlot;
    public Image chestSlot;
    public Image pantsSlot;
    public Image bootsSlot;
    public Image weaponSlot;
    public Image glovesSlot;

    private List<Item> inventoryItems = new List<Item>();

    void Start()
    {

        UpdateInventoryUI();
    }

    public void AddItemToInventory(Item newItem)
    {
        InventoryManager.Instance.AddItem(newItem);
        CreateItemSlot(newItem);
    }

    private void CreateItemSlot(Item item)
    {
        GameObject itemSlot = Instantiate(itemSlotPrefab, inventoryGrid);
        Image itemImage = itemSlot.GetComponent<Image>();
        itemImage.sprite = item.icon;

        Button itemButton = itemSlot.GetComponent<Button>();
        itemButton.onClick.AddListener(() => EquipItem(item, itemSlot));
    }

    private void EquipItem(Item item, GameObject itemSlot)
    {
        Image targetSlot = GetEquipmentSlot(item.itemType);
        if (targetSlot != null)
        {
            EquipToSlot(targetSlot, item, itemSlot);
        }
    }

    private Image GetEquipmentSlot(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Helmet => helmetSlot,
            ItemType.Chestplate => chestSlot,
            ItemType.Pants => pantsSlot,
            ItemType.Boots => bootsSlot,
            ItemType.Weapon => weaponSlot,
            ItemType.Gloves => glovesSlot,
            _ => null
        };
    }

    private void EquipToSlot(Image equipmentSlot, Item item, GameObject itemSlot)
    {
        if (equipmentSlot.sprite != null)
        {
            Item existingItem = GetItemFromSlot(equipmentSlot);
            if (existingItem != null)
            {
                AddItemToInventory(existingItem);
            }
        }

        equipmentSlot.sprite = item.icon;
        equipmentSlot.color = Color.white;

        Destroy(itemSlot);
        InventoryManager.Instance.inventoryItems.Remove(item);
        UpdateInventoryUI(); // Обновляем UI только если это необходимо
    }

    private Item GetItemFromSlot(Image equipmentSlot)
    {
        return InventoryManager.Instance.inventoryItems.Find(item => item.icon == equipmentSlot.sprite);
    }

    public void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in InventoryManager.Instance.inventoryItems)
        {
            CreateItemSlot(item);
        }
    }
}
