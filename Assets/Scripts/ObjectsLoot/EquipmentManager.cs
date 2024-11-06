using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public Item helmet;
    public Item chestplate;
    public Item boots;
    public Item pants;
    public Item gloves;
    public Item weapon;

    // Метод для получения экипированного предмета по типу
    public Item GetEquippedItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Helmet: return helmet;
            case ItemType.Chestplate: return chestplate;
            case ItemType.Boots: return boots;
            case ItemType.Pants: return pants;
            case ItemType.Gloves: return gloves;
            case ItemType.Weapon: return weapon;
            default: return null;
        }
    }

    public void EquipFromInventory(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Helmet: helmet = item; break;
            case ItemType.Chestplate: chestplate = item; break;
            case ItemType.Boots: boots = item; break;
            case ItemType.Pants: pants = item; break;
            case ItemType.Gloves: gloves = item; break;
            case ItemType.Weapon: weapon = item; break;
        }
    }

    public void Unequip(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Helmet: helmet = null; break;
            case ItemType.Chestplate: chestplate = null; break;
            case ItemType.Boots: boots = null; break;
            case ItemType.Pants: pants = null; break;
            case ItemType.Gloves: gloves = null; break;
            case ItemType.Weapon: weapon = null; break;
        }
    }
}
