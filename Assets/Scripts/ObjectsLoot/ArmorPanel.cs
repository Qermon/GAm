using UnityEngine;

public class ArmorPanel : MonoBehaviour
{
    public ArmorSlot helmetSlot;
    public ArmorSlot chestplateSlot;
    public ArmorSlot glovesSlot;
    public ArmorSlot pantsSlot;
    public ArmorSlot bootsSlot;

    // Метод для добавления предмета в слот брони
    public void AddItemToArmorSlot(Item item)
    {
        // Проверяем, в какой слот можно добавить предмет
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
                Debug.LogWarning("Этот предмет не подходит для слота брони.");
                break;
        }
    }
}
