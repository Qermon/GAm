using UnityEngine;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    public GameObject chestPanel; // Панель с кнопкой сундука
    public Button openChestButton; // Кнопка открытия сундука
    public InventoryUIManager inventoryUIManager; // Ссылка на ваш инвентарный менеджер
    public WaveManager waveManager; // Ссылка на WaveManager

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        openChestButton.onClick.AddListener(OpenChest);
        chestPanel.SetActive(false); // Делаем панель невидимой в начале
    }

    public void ShowChestPanel()
    {
        chestPanel.SetActive(true); // Показываем панель сундука
    }

    private void OpenChest()
    {
        Item newItem = GenerateRandomItem(); // Генерируем новый предмет
        inventoryUIManager.AddItemToInventory(newItem); // Добавляем предмет в инвентарь
        InventoryManager.Instance.AddItemToInventory(newItem);
        chestPanel.SetActive(false); // Закрываем панель сундука

        GameManager.GetInstance().RestartGameWithDelay(); // Перезапускаем игру
    }


    private Item GenerateRandomItem()
    {
        // Логика генерации случайного предмета
        ItemType randomType = (ItemType)Random.Range(0, System.Enum.GetValues(typeof(ItemType)).Length);
        Sprite randomIcon = LoadRandomIcon(randomType); // Получаем случайную иконку для предмета

        return new Item
        {
            itemName = randomType.ToString(),
            itemType = randomType,
            icon = randomIcon, // Используем случайную иконку
            itemValue = Random.Range(1, 100) // Пример случайного значения
        };
    }

    private Sprite LoadRandomIcon(ItemType itemType)
    {
        string path = $"Icons/{itemType}"; // Путь к папке с иконками
        Sprite[] icons = Resources.LoadAll<Sprite>(path); // Загружаем все спрайты из папки

        if (icons.Length > 0)
        {
            int randomIndex = Random.Range(0, icons.Length); // Выбор случайного индекса
            return icons[randomIndex]; // Возвращаем случайную иконку
        }

        return null; // Если иконки не найдены, возвращаем null
    }
}
