using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public string itemName;
    public ItemType itemType; // ��� �������� (��������, ���������, ���� � �.�.)
    public Sprite icon; // ��������� ���� ��� ������
    public int itemValue; // �������� ��� ������ �������������� ��������
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