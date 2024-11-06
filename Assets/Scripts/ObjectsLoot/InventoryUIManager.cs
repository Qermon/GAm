using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public EquipmentManager equipmentManager;

    public GameObject inventoryPanel;  // ������ ���������
    public GameObject equipmentPanel;  // ������ ����������
    public GameObject inventoryItemButtonPrefab;  // ������ ������ ��� ��������� � ���������
    public Transform inventoryGrid;  // ���� ��� ���������
    public Button[] equipmentButtons;  // ������ ������ ��� ������ ���������� (������������� �����)

    void Start()
    {
        // ������������� UI ���������
        UpdateInventoryUI();
        UpdateEquipmentUI();
    }

    // ��������� ���������
    public void UpdateInventoryUI()
    {
        // ������� ��� ������ ������
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        // ������� ����� ������ ��� ������� �������� � ���������
        foreach (Item item in inventoryManager.inventoryItems)
        {
            GameObject newButton = Instantiate(inventoryItemButtonPrefab, inventoryGrid);
            newButton.GetComponent<Image>().sprite = item.icon;
            newButton.GetComponent<Button>().onClick.RemoveAllListeners();
            newButton.GetComponent<Button>().onClick.AddListener(() => EquipItemFromInventory(item));
        }
    }

    // ��������� ������ ���������� (������������� ������)
    void UpdateEquipmentUI()
    {
        // ������� �� ������� ������ ����������
        for (int i = 0; i < equipmentButtons.Length; i++)
        {
            Item equippedItem = null;

            // �������� ������� �� ���������� � ����������� �� ����
            switch (i)
            {
                case 0:
                    equippedItem = equipmentManager.helmet;
                    break;
                case 1:
                    equippedItem = equipmentManager.chestplate;
                    break;
                case 2:
                    equippedItem = equipmentManager.boots;
                    break;
                case 3:
                    equippedItem = equipmentManager.pants;
                    break;
                case 4:
                    equippedItem = equipmentManager.gloves;
                    break;
                case 5:
                    equippedItem = equipmentManager.weapon;
                    break;
            }

            if (equippedItem != null)
            {
                // ���� ������� ����������, ���������� ��� ������ �� ������
                equipmentButtons[i].GetComponent<Image>().sprite = equippedItem.icon;
                equipmentButtons[i].onClick.RemoveAllListeners();
                equipmentButtons[i].onClick.AddListener(() => UnequipItem(i));
                equipmentButtons[i].gameObject.SetActive(true);  // ���������� ������
            }
            else
            {
                // ���� ���� ����, �������� ������
                equipmentButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // ����������� ������� �� ���������
    void EquipItemFromInventory(Item item)
    {
        // ��������� ������� � ��������������� ����
        equipmentManager.EquipFromInventory(item);

        // ������� ������� �� ���������
        inventoryManager.RemoveItem(item);

        // ��������� UI
        UpdateInventoryUI();
        UpdateEquipmentUI();
    }

    // ����� ������� � ����������
    void UnequipItem(int index)
    {
        Item selectedItem = null;

        // ����������, ����� ������� ������� � ����������� �� �������
        switch (index)
        {
            case 0:
                selectedItem = equipmentManager.helmet;
                break;
            case 1:
                selectedItem = equipmentManager.chestplate;
                break;
            case 2:
                selectedItem = equipmentManager.boots;
                break;
            case 3:
                selectedItem = equipmentManager.pants;
                break;
            case 4:
                selectedItem = equipmentManager.gloves;
                break;
            case 5:
                selectedItem = equipmentManager.weapon;
                break;
        }

        if (selectedItem != null)
        {
            // ������� ������� � ����������
            equipmentManager.Unequip(selectedItem);

            // ��������� ������� ������� � ���������
            inventoryManager.AddItem(selectedItem);

            // ��������� UI
            UpdateInventoryUI();
            UpdateEquipmentUI();
        }
    }

    // ������� ������ ���������
    public void PressOnExitButton()
    {
        inventoryPanel.SetActive(false);
    }
}
