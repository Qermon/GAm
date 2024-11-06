using UnityEngine;

public enum ItemType
{
    Helmet,
    Chestplate,
    Gloves,
    Pants,
    Boots,
    Weapon
}

[System.Serializable]
public class Item
{
    public string itemName;
    public int value;
    public ItemType itemType;
    public Sprite icon;
}


public class ItemUI : MonoBehaviour
{
    public Item item;
}