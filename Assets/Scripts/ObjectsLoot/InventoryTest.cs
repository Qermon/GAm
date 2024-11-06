using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    public Inventory inventory;

    private void Start()
    {
        // Создание 6 тестовых айтемов разных типов с загрузкой первой иконки из каждой папки
        Item helmet = new Item
        {
            itemName = "Steel Helmet",
            value = 10,
            itemType = ItemType.Helmet,
            icon = Resources.Load<Sprite>("Icons/Helmet/helmet_1")
        };

        Item chestplate = new Item
        {
            itemName = "Iron Chestplate",
            value = 15,
            itemType = ItemType.Chestplate,
            icon = Resources.Load<Sprite>("Icons/Chestplate/chestplate_1")
        };

        Item gloves = new Item
        {
            itemName = "Leather Gloves",
            value = 5,
            itemType = ItemType.Gloves,
            icon = Resources.Load<Sprite>("Icons/Gloves/gloves_1")
        };

        Item pants = new Item
        {
            itemName = "Steel Pants",
            value = 12,
            itemType = ItemType.Pants,
            icon = Resources.Load<Sprite>("Icons/Pants/pants_1")
        };

        Item boots = new Item
        {
            itemName = "Iron Boots",
            value = 8,
            itemType = ItemType.Boots,
            icon = Resources.Load<Sprite>("Icons/Boots/boots_1")
        };

        Item weapon = new Item
        {
            itemName = "Sword",
            value = 20,
            itemType = ItemType.Weapon,
            icon = Resources.Load<Sprite>("Icons/Weapon/weapon_1")
        };

        // Добавление всех айтемов в инвентарь
        inventory.AddItem(helmet);
        inventory.AddItem(chestplate);
        inventory.AddItem(gloves);
        inventory.AddItem(pants);
        inventory.AddItem(boots);
        inventory.AddItem(weapon);
    }
}
