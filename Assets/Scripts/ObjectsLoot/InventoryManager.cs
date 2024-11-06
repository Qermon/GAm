using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<Item> inventoryItems = new List<Item>();

    public List<Item> equippedItems = new List<Item>(); // Список экипированных предметов
    public static InventoryManager Instance; // Добавляем статическое свойство для синглтона
    public InventoryUIManager inventoryUIManager;  // Ссылка на UI менеджер для обновления UI

    void Awake()
    {
        // Если экземпляр ещё не установлен, присваиваем его
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Опционально, чтобы не уничтожать объект при переходе между сценами
        }
        else
        {
            Destroy(gameObject); // Если экземпляр уже существует, уничтожаем новый объект
        }
    }

    void Start()
    {
        LoadInventory();
    }

    // Метод для добавления предмета в инвентарь
    // Метод для добавления предмета в инвентарь
    public void AddItem(Item item)
    {
        inventoryItems.Add(item);
    }

    public void RemoveItem(Item item)
    {
        inventoryItems.Remove(item);
    }

    // Обновление UI
    public void UpdateUI()
    {
        inventoryUIManager.UpdateInventoryUI();  // Обновляем UI через InventoryUIManager
    }

    public void EquipItem(Item item)
    {
        // Если в экипированном слоте уже есть предмет, заменяем его
        Item existingItem = equippedItems.Find(e => e.itemType == item.itemType);
        if (existingItem != null)
        {
            // Перемещаем обратно в инвентарь
            inventoryItems.Add(existingItem);
        }

        // Перемещаем предмет в экипированный слот
        equippedItems.Add(item);
        inventoryItems.Remove(item);
        SaveInventory();
    }

    public void UnequipItem(Item item)
    {
        equippedItems.Remove(item);
        inventoryItems.Add(item);
        SaveInventory();
    }

    void SaveInventory()
    {
        // Сохраняем инвентарь и экипированные предметы (например, в PlayerPrefs или в файл)
        // Здесь пример сохранения с использованием PlayerPrefs
        PlayerPrefs.SetInt("inventoryCount", inventoryItems.Count);
        PlayerPrefs.SetInt("equippedCount", equippedItems.Count);
    }

    void LoadInventory()
    {
        // Загружаем сохраненные данные инвентаря и экипированных предметов
        int inventoryCount = PlayerPrefs.GetInt("inventoryCount", 0);
        int equippedCount = PlayerPrefs.GetInt("equippedCount", 0);

        // Пример загрузки: создаем пустые списки (на реальной игре нужно загрузить конкретные предметы)
        inventoryItems.Clear();
        equippedItems.Clear();
    }




}