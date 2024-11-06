using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public EquipmentManager equipmentManager;

    public GameObject inventoryPanel;  // Панель инвентаря
    public GameObject equipmentPanel;  // Панель экипировки
    public GameObject inventoryItemButtonPrefab;  // Префаб кнопки для предметов в инвентаре
    public Transform inventoryGrid;  // Грид для инвентаря
    public Button[] equipmentButtons;  // Массив кнопок для панели экипировки (фиксированные слоты)

    void Start()
    {
        // Инициализация UI элементов
        UpdateInventoryUI();
        UpdateEquipmentUI();
    }

    // Обновляем инвентарь
    public void UpdateInventoryUI()
    {
        // Удаляем все старые кнопки
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        // Создаем новые кнопки для каждого предмета в инвентаре
        foreach (Item item in inventoryManager.inventoryItems)
        {
            GameObject newButton = Instantiate(inventoryItemButtonPrefab, inventoryGrid);
            newButton.GetComponent<Image>().sprite = item.icon;
            newButton.GetComponent<Button>().onClick.RemoveAllListeners();
            newButton.GetComponent<Button>().onClick.AddListener(() => EquipItemFromInventory(item));
        }
    }

    // Обновляем панель экипировки (фиксированные кнопки)
    void UpdateEquipmentUI()
    {
        // Пройдем по массиву кнопок экипировки
        for (int i = 0; i < equipmentButtons.Length; i++)
        {
            Item equippedItem = null;

            // Получаем предмет из экипировки в зависимости от типа
            switch (i)
            {
                case 0:
                    equippedItem = equipmentManager.helmet;
                    break;
                case 1:
                    equippedItem = equipmentManager.chestplate;
                    break;
                case 2:
                    equippedItem = equipmentManager.boots;
                    break;
                case 3:
                    equippedItem = equipmentManager.pants;
                    break;
                case 4:
                    equippedItem = equipmentManager.gloves;
                    break;
                case 5:
                    equippedItem = equipmentManager.weapon;
                    break;
            }

            if (equippedItem != null)
            {
                // Если предмет экипирован, показываем его иконку на кнопке
                equipmentButtons[i].GetComponent<Image>().sprite = equippedItem.icon;
                equipmentButtons[i].onClick.RemoveAllListeners();
                equipmentButtons[i].onClick.AddListener(() => UnequipItem(i));
                equipmentButtons[i].gameObject.SetActive(true);  // Показываем кнопку
            }
            else
            {
                // Если слот пуст, скрываем кнопку
                equipmentButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // Экипировать предмет из инвентаря
    void EquipItemFromInventory(Item item)
    {
        // Экипируем предмет в соответствующий слот
        equipmentManager.EquipFromInventory(item);

        // Убираем предмет из инвентаря
        inventoryManager.RemoveItem(item);

        // Обновляем UI
        UpdateInventoryUI();
        UpdateEquipmentUI();
    }

    // Снять предмет с экипировки
    void UnequipItem(int index)
    {
        Item selectedItem = null;

        // Определяем, какой предмет снимать в зависимости от индекса
        switch (index)
        {
            case 0:
                selectedItem = equipmentManager.helmet;
                break;
            case 1:
                selectedItem = equipmentManager.chestplate;
                break;
            case 2:
                selectedItem = equipmentManager.boots;
                break;
            case 3:
                selectedItem = equipmentManager.pants;
                break;
            case 4:
                selectedItem = equipmentManager.gloves;
                break;
            case 5:
                selectedItem = equipmentManager.weapon;
                break;
        }

        if (selectedItem != null)
        {
            // Снимаем предмет с экипировки
            equipmentManager.Unequip(selectedItem);

            // Добавляем предмет обратно в инвентарь
            inventoryManager.AddItem(selectedItem);

            // Обновляем UI
            UpdateInventoryUI();
            UpdateEquipmentUI();
        }
    }

    // Закрыть панель инвентаря
    public void PressOnExitButton()
    {
        inventoryPanel.SetActive(false);
    }
}
