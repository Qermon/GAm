using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // Singleton для доступа из других классов

    public List<Item> inventoryItems = new List<Item>(); // Список предметов в инвентаре
    public Item equippedItem; // Экипированный предмет (можно расширить на несколько предметов)
    private InventoryUIManager inventoryUIManager;

    [Header("Item List")]
    public List<Item> allItems = new List<Item>(); // Список всех предметов (будет инициализирован)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем объект при смене сцены
            LoadInventory();
        }
        else
        {
            Destroy(gameObject); // Удаляем дубликат
        }
    }


    private void Start()
    {
        inventoryUIManager = FindObjectOfType<InventoryUIManager>();
        LoadInventory(); // Загружаем инвентарь при старте
    }

    public void AddItemToInventory(Item item)
    {
        if (item != null)
        {
            // Загружаем иконку для предмета
            item.icon = LoadItemIcon(item.itemName, item.itemType);

            inventoryItems.Add(item); // Добавляем предмет в инвентарь
            Debug.Log($"Предмет {item.itemName} добавлен в инвентарь."); // Выводим сообщение для проверки
            SaveInventory(); // Сохраняем инвентарь после добавления
        }
        else
        {
            Debug.LogWarning("Попытка добавить null предмет в инвентарь.");
        }
    }

    public void EquipItem(Item item)
    {
        equippedItem = item; // Экипируем предмет
        // Дополнительная логика для изменения характеристик игрока
    }

    public void ClearInventory()
    {
        inventoryItems.Clear(); // Очищаем инвентарь (если нужно)
        equippedItem = null; // Сбрасываем экипировку (если нужно)
        SaveInventory(); // Сохраняем инвентарь после очистки
    }

    // Сохранение инвентаря
    private void SaveInventory()
    {
        PlayerPrefs.SetInt("InventoryCount", inventoryItems.Count);
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            PlayerPrefs.SetString($"ItemName_{i}", inventoryItems[i].itemName);
            PlayerPrefs.SetInt($"ItemType_{i}", (int)inventoryItems[i].itemType);
        }
        PlayerPrefs.Save();
        Debug.Log("Инвентарь сохранен.");
    }


    // Загрузка инвентаря
    private void LoadInventory()
    {
        int count = PlayerPrefs.GetInt("InventoryCount", 0);
        inventoryItems.Clear(); // Очистить существующий инвентарь

        for (int i = 0; i < count; i++)
        {
            string itemName = PlayerPrefs.GetString($"ItemName_{i}", string.Empty);
            ItemType itemType = (ItemType)PlayerPrefs.GetInt($"ItemType_{i}", 0);

            // Загружаем иконку
            Sprite itemIcon = LoadItemIcon(itemName, itemType);

            // Создаем предмет только если иконка была успешно загружена
            if (itemIcon != null)
            {
                Item loadedItem = new Item
                {
                    itemName = itemName,
                    itemType = itemType,
                    icon = itemIcon // Загружаем иконку
                };

                inventoryItems.Add(loadedItem);
            }
            else
            {
                Debug.LogWarning($"Не удалось загрузить предмет {itemName}, иконка не найдена.");
            }
        }

        inventoryUIManager.UpdateInventoryUI();
        Debug.Log("Инвентарь загружен.");
    }


    private Sprite LoadItemIcon(string itemName, ItemType itemType)
    {
        // Пытаемся загрузить иконку для каждого типа
        string iconPath = $"Icons/{itemType}/{itemName.ToLower()}"; // Используем маленькие буквы для поиска
        Sprite icon = Resources.Load<Sprite>(iconPath);

        // Если иконка не найдена, показываем ошибку и возвращаем иконку по умолчанию
        if (icon == null)
        {
            Debug.LogError($"Иконка для {itemName} не найдена! Путь: {iconPath}");
            icon = Resources.Load<Sprite>("Icons/DefaultIcon"); // Устанавливаем иконку по умолчанию
        }

        return icon;
    }

    // Метод для добавления предмета в инвентарь
    public void AddItem(Item item)
    {
        inventoryItems.Add(item);
        SaveInventory(); // Сохраняем инвентарь после добавления предмета
    }
}
