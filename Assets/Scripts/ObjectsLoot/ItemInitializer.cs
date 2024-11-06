using UnityEngine;

public class ItemInitializer : MonoBehaviour
{
    public Sprite[] bootsIcons;
    public Sprite[] chestplateIcons;
    public Sprite[] glovesIcons;
    public Sprite[] helmetIcons;
    public Sprite[] pantsIcons;
    public Sprite[] weaponIcons;

    public InventoryManager inventoryManager;

    void Start()
    {
        InitializeRandomItems();
        InitializeItems();
    }

    void InitializeItems()
    {
        // Добавление предметов в инвентарь
        inventoryManager.AddItem(new Item("Boots 1", ItemType.Boots, bootsIcons[0], ItemStatType.Defense, 5));
        inventoryManager.AddItem(new Item("Chestplate 1", ItemType.Chestplate, chestplateIcons[0], ItemStatType.Defense, 10));
        inventoryManager.AddItem(new Item("Gloves 1", ItemType.Gloves, glovesIcons[0], ItemStatType.Agility, 3));
        inventoryManager.AddItem(new Item("Helmet 1", ItemType.Helmet, helmetIcons[0], ItemStatType.Health, 15));
        inventoryManager.AddItem(new Item("Pants 1", ItemType.Pants, pantsIcons[0], ItemStatType.Defense, 7));
        inventoryManager.AddItem(new Item("Weapon 1", ItemType.Weapon, weaponIcons[0], ItemStatType.Attack, 20));

        // Обновляем UI после того, как предметы добавлены
        inventoryManager.UpdateUI();
    }

    void InitializeRandomItems()
    {
        // Добавляем случайные предметы в инвентарь

        // Случайный предмет для ботинок
        AddRandomItem(ItemType.Boots, bootsIcons, ItemStatType.Defense);

        // Случайный предмет для бронежилета
        AddRandomItem(ItemType.Chestplate, chestplateIcons, ItemStatType.Defense);

        // Случайный предмет для перчаток
        AddRandomItem(ItemType.Gloves, glovesIcons, ItemStatType.Agility);

        // Случайный предмет для шлема
        AddRandomItem(ItemType.Helmet, helmetIcons, ItemStatType.Health);

        // Случайный предмет для штанов
        AddRandomItem(ItemType.Pants, pantsIcons, ItemStatType.Defense);

        // Случайный предмет для оружия
        AddRandomItem(ItemType.Weapon, weaponIcons, ItemStatType.Attack);
    }

    // Метод для создания случайного предмета
    void AddRandomItem(ItemType itemType, Sprite[] itemIcons, ItemStatType statType)
    {
        // Выбираем случайный индекс иконки
        int randomIndex = Random.Range(0, itemIcons.Length);

        // Создаем новый случайный предмет с случайным значением
        int randomValue = Random.Range(1, 21); // Случайное значение для характеристики (например, от 1 до 20)

        // Добавляем предмет в инвентарь
        inventoryManager.AddItem(new Item($"{itemType} {randomIndex + 1}", itemType, itemIcons[randomIndex], statType, randomValue));
    }
}
