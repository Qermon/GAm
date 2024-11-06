using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Item item;  // Предмет, находящийся в слоте
    public Image icon;  // Иконка для отображения предмета

    // Метод для установки предмета в слот
    public void SetItem(Item newItem)
    {
        item = newItem;  // Устанавливаем новый предмет
        icon.sprite = newItem.icon;  // Устанавливаем иконку
        icon.enabled = true;  // Показываем иконку
    }

    // Метод для очистки слота
    public void ClearSlot()
    {
        item = null;  // Очищаем слот
        icon.enabled = false;  // Скрываем иконку
    }

    // Проверка, пуст ли слот
    public bool IsEmpty()
    {
        return item == null;  // Слот пуст, если item равен null
    }
}

