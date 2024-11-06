using UnityEngine;
using UnityEngine.UI;

public class ArmorSlot : MonoBehaviour
{
    public ItemType allowedItemType;  // ��� ��������, ������� ����� ��������� � ���� ����
    public Image icon;                // ������ ��� ����������� ��������
    private Item currentItem;         // ������� ������� � �����

    // ����� ��� ��������� �������� � ����
    public void SetItem(Item item)
    {
        if (item.itemType == allowedItemType) // ���������, �������� �� ���
        {
            currentItem = item;
            icon.sprite = item.icon;
            icon.enabled = true;
        }
        else
        {
            Debug.LogWarning("�������� ��� �������� ��� ����� �����!");
        }
    }

    // ����� ��� ������� �����
    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    // ����� ��� ��������� �������� ��������
    public Item GetCurrentItem()
    {
        return currentItem;
    }
}
