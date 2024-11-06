using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots;  // ����� ���������
    public List<ArmorSlot> armorSlots; // ����� �����

    // ����� ��� ���������� �������� � ���������
    public void AddItem(Item newItem)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())  // ���� ���� ����
            {
                slot.SetItem(newItem);  // ��������� ������� � ����
                return;  // ��������� �����, ������� ��������
            }
        }

        Debug.Log("��� ����� � ���������");  // ���� ��� ������ ������
    }

    // �������� �������� �� ���������
    public void RemoveItem(Item itemToRemove)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == itemToRemove)
            {
                slot.ClearSlot();
                return;
            }
        }

        Debug.Log("������� �� ������ � ���������");
    }

    // �������� ��� �������� � ���������
    public void ShowInventoryItems()
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item != null)
            {
                Debug.Log("Item in slot: " + slot.item.itemName);
            }
            else
            {
                Debug.Log("Slot is empty");
            }
        }
    }

}
