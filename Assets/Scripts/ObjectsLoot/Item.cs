using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public ItemStatType statType; // ��� �������������� ��������
    public int itemValue; // �������� ��� ��������� �������������� (��������, +5 �����)

    // ����������� ��� �������� ��������
    public Item(string name, ItemType type, Sprite icon, ItemStatType statType, int itemValue)
    {
        itemName = name;
        itemType = type;
        this.icon = icon;
        this.statType = statType;
        this.itemValue = itemValue;
    }
}


public enum ItemType
{
    Helmet,
    Chestplate,
    Pants,
    Boots,
    Weapon,
    Gloves
}

public enum ItemStatType
{
    Health,
    Attack,
    Defense,
    Agility
}

