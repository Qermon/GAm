using System.Collections.Generic;
using UnityEngine;

public class ItemInitializer : MonoBehaviour
{
    public List<Item> allItems; // Список для хранения всех предметов

    private void Start()
    {
        InitializeItems();
    }

    private void InitializeItems()
    {
        // Создание предметов для каждого типа
        for (int i = 1; i <= 3; i++)
        {
            allItems.Add(CreateItem($"boots_{i}", ItemType.Boots));
        }
        for (int i = 1; i <= 5; i++)
        {
            allItems.Add(CreateItem($"chestplate_{i}", ItemType.Chestplate));
        }
        for (int i = 1; i <= 8; i++)
        {
            allItems.Add(CreateItem($"gloves_{i}", ItemType.Gloves));
        }
        for (int i = 1; i <= 7; i++)
        {
            allItems.Add(CreateItem($"helmet_{i}", ItemType.Helmet));
        }
        for (int i = 1; i <= 3; i++)
        {
            allItems.Add(CreateItem($"pants_{i}", ItemType.Pants));
        }
        for (int i = 1; i <= 4; i++)
        {
            allItems.Add(CreateItem($"weapon_{i}", ItemType.Weapon));
        }
    }

    private Item CreateItem(string name, ItemType itemType)
    {
        Item item = new Item
        {
            itemName = name,
            itemType = itemType,
            icon = Resources.Load<Sprite>($"Icons/{itemType}/{name}"),
            itemValue = 1 // Установите значение или другие характеристики, если необходимо
        };

        return item;
    }
}
