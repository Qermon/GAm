using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public string itemName;
    public ItemType itemType; // Тип предмета (например, нагрудник, шлем и т.д.)
    public Sprite icon; // Добавляем поле для иконки
    public int itemValue; // Значение или другие характеристики предмета
}

public enum ItemType
{
    Chestplate,
    Pants,
    Gloves,
    Helmet,
    Boots,
    Weapon
}