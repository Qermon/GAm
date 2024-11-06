using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<Item> inventoryItems = new List<Item>();

    public List<Item> equippedItems = new List<Item>(); // ������ ������������� ���������
    public static InventoryManager Instance; // ��������� ����������� �������� ��� ���������
    public InventoryUIManager inventoryUIManager;  // ������ �� UI �������� ��� ���������� UI

    void Awake()
    {
        // ���� ��������� ��� �� ����������, ����������� ���
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �����������, ����� �� ���������� ������ ��� �������� ����� �������
        }
        else
        {
            Destroy(gameObject); // ���� ��������� ��� ����������, ���������� ����� ������
        }
    }

    void Start()
    {
        LoadInventory();
    }

    // ����� ��� ���������� �������� � ���������
    // ����� ��� ���������� �������� � ���������
    public void AddItem(Item item)
    {
        inventoryItems.Add(item);
    }

    public void RemoveItem(Item item)
    {
        inventoryItems.Remove(item);
    }

    // ���������� UI
    public void UpdateUI()
    {
        inventoryUIManager.UpdateInventoryUI();  // ��������� UI ����� InventoryUIManager
    }

    public void EquipItem(Item item)
    {
        // ���� � ������������� ����� ��� ���� �������, �������� ���
        Item existingItem = equippedItems.Find(e => e.itemType == item.itemType);
        if (existingItem != null)
        {
            // ���������� ������� � ���������
            inventoryItems.Add(existingItem);
        }

        // ���������� ������� � ������������� ����
        equippedItems.Add(item);
        inventoryItems.Remove(item);
        SaveInventory();
    }

    public void UnequipItem(Item item)
    {
        equippedItems.Remove(item);
        inventoryItems.Add(item);
        SaveInventory();
    }

    void SaveInventory()
    {
        // ��������� ��������� � ������������� �������� (��������, � PlayerPrefs ��� � ����)
        // ����� ������ ���������� � �������������� PlayerPrefs
        PlayerPrefs.SetInt("inventoryCount", inventoryItems.Count);
        PlayerPrefs.SetInt("equippedCount", equippedItems.Count);
    }

    void LoadInventory()
    {
        // ��������� ����������� ������ ��������� � ������������� ���������
        int inventoryCount = PlayerPrefs.GetInt("inventoryCount", 0);
        int equippedCount = PlayerPrefs.GetInt("equippedCount", 0);

        // ������ ��������: ������� ������ ������ (�� �������� ���� ����� ��������� ���������� ��������)
        inventoryItems.Clear();
        equippedItems.Clear();
    }




}