using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Item item;  // �������, ����������� � �����
    public Image icon;  // ������ ��� ����������� ��������

    // ����� ��� ��������� �������� � ����
    public void SetItem(Item newItem)
    {
        item = newItem;  // ������������� ����� �������
        icon.sprite = newItem.icon;  // ������������� ������
        icon.enabled = true;  // ���������� ������
    }

    // ����� ��� ������� �����
    public void ClearSlot()
    {
        item = null;  // ������� ����
        icon.enabled = false;  // �������� ������
    }

    // ��������, ���� �� ����
    public bool IsEmpty()
    {
        return item == null;  // ���� ����, ���� item ����� null
    }
}

