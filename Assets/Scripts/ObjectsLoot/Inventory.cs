using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots;  // Слоты инвентаря
    public List<ArmorSlot> armorSlots; // Слоты брони

    // Метод для добавления предмета в инвентарь
    public void AddItem(Item newItem)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())  // Если слот пуст
            {
                slot.SetItem(newItem);  // Добавляем предмет в слот
                return;  // Завершаем метод, предмет добавлен
            }
        }

        Debug.Log("Нет места в инвентаре");  // Если нет пустых слотов
    }

    // Удаление предмета из инвентаря
    public void RemoveItem(Item itemToRemove)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == itemToRemove)
            {
                slot.ClearSlot();
                return;
            }
        }

        Debug.Log("Предмет не найден в инвентаре");
    }

    // Показать все предметы в инвентаре
    public void ShowInventoryItems()
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item != null)
            {
                Debug.Log("Item in slot: " + slot.item.itemName);
            }
            else
            {
                Debug.Log("Slot is empty");
            }
        }
    }

}
