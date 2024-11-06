using UnityEngine;

public class ArmorPanel : MonoBehaviour
{
    public ArmorSlot helmetSlot;
    public ArmorSlot chestplateSlot;
    public ArmorSlot glovesSlot;
    public ArmorSlot pantsSlot;
    public ArmorSlot bootsSlot;

    // ����� ��� ���������� �������� � ���� �����
    public void AddItemToArmorSlot(Item item)
    {
        // ���������, � ����� ���� ����� �������� �������
        switch (item.itemType)
        {
            case ItemType.Helmet:
                helmetSlot.SetItem(item);
                break;
            case ItemType.Chestplate:
                chestplateSlot.SetItem(item);
                break;
            case ItemType.Gloves:
                glovesSlot.SetItem(item);
                break;
            case ItemType.Pants:
                pantsSlot.SetItem(item);
                break;
            case ItemType.Boots:
                bootsSlot.SetItem(item);
                break;
            default:
                Debug.LogWarning("���� ������� �� �������� ��� ����� �����.");
                break;
        }
    }
}
