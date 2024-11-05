using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // Singleton ��� ������� �� ������ �������

    public List<Item> inventoryItems = new List<Item>(); // ������ ��������� � ���������
    public Item equippedItem; // ������������� ������� (����� ��������� �� ��������� ���������)
    private InventoryUIManager inventoryUIManager;

    [Header("Item List")]
    public List<Item> allItems = new List<Item>(); // ������ ���� ��������� (����� ���������������)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ��������� ������ ��� ����� �����
            LoadInventory();
        }
        else
        {
            Destroy(gameObject); // ������� ��������
        }
    }


    private void Start()
    {
        inventoryUIManager = FindObjectOfType<InventoryUIManager>();
        LoadInventory(); // ��������� ��������� ��� ������
    }

    public void AddItemToInventory(Item item)
    {
        if (item != null)
        {
            // ��������� ������ ��� ��������
            item.icon = LoadItemIcon(item.itemName, item.itemType);

            inventoryItems.Add(item); // ��������� ������� � ���������
            Debug.Log($"������� {item.itemName} �������� � ���������."); // ������� ��������� ��� ��������
            SaveInventory(); // ��������� ��������� ����� ����������
        }
        else
        {
            Debug.LogWarning("������� �������� null ������� � ���������.");
        }
    }

    public void EquipItem(Item item)
    {
        equippedItem = item; // ��������� �������
        // �������������� ������ ��� ��������� ������������� ������
    }

    public void ClearInventory()
    {
        inventoryItems.Clear(); // ������� ��������� (���� �����)
        equippedItem = null; // ���������� ���������� (���� �����)
        SaveInventory(); // ��������� ��������� ����� �������
    }

    // ���������� ���������
    private void SaveInventory()
    {
        PlayerPrefs.SetInt("InventoryCount", inventoryItems.Count);
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            PlayerPrefs.SetString($"ItemName_{i}", inventoryItems[i].itemName);
            PlayerPrefs.SetInt($"ItemType_{i}", (int)inventoryItems[i].itemType);
        }
        PlayerPrefs.Save();
        Debug.Log("��������� ��������.");
    }


    // �������� ���������
    private void LoadInventory()
    {
        int count = PlayerPrefs.GetInt("InventoryCount", 0);
        inventoryItems.Clear(); // �������� ������������ ���������

        for (int i = 0; i < count; i++)
        {
            string itemName = PlayerPrefs.GetString($"ItemName_{i}", string.Empty);
            ItemType itemType = (ItemType)PlayerPrefs.GetInt($"ItemType_{i}", 0);

            // ��������� ������
            Sprite itemIcon = LoadItemIcon(itemName, itemType);

            // ������� ������� ������ ���� ������ ���� ������� ���������
            if (itemIcon != null)
            {
                Item loadedItem = new Item
                {
                    itemName = itemName,
                    itemType = itemType,
                    icon = itemIcon // ��������� ������
                };

                inventoryItems.Add(loadedItem);
            }
            else
            {
                Debug.LogWarning($"�� ������� ��������� ������� {itemName}, ������ �� �������.");
            }
        }

        inventoryUIManager.UpdateInventoryUI();
        Debug.Log("��������� ��������.");
    }


    private Sprite LoadItemIcon(string itemName, ItemType itemType)
    {
        // �������� ��������� ������ ��� ������� ����
        string iconPath = $"Icons/{itemType}/{itemName.ToLower()}"; // ���������� ��������� ����� ��� ������
        Sprite icon = Resources.Load<Sprite>(iconPath);

        // ���� ������ �� �������, ���������� ������ � ���������� ������ �� ���������
        if (icon == null)
        {
            Debug.LogError($"������ ��� {itemName} �� �������! ����: {iconPath}");
            icon = Resources.Load<Sprite>("Icons/DefaultIcon"); // ������������� ������ �� ���������
        }

        return icon;
    }

    // ����� ��� ���������� �������� � ���������
    public void AddItem(Item item)
    {
        inventoryItems.Add(item);
        SaveInventory(); // ��������� ��������� ����� ���������� ��������
    }
}
