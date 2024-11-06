using UnityEngine;
using UnityEngine.UI;

public class ArmorSlot : MonoBehaviour
{
    public ItemType allowedItemType;  // Тип предмета, который можно поместить в этот слот
    public Image icon;                // Иконка для отображения предмета
    private Item currentItem;         // Текущий предмет в слоте

    // Метод для установки предмета в слот
    public void SetItem(Item item)
    {
        if (item.itemType == allowedItemType) // Проверяем, подходит ли тип
        {
            currentItem = item;
            icon.sprite = item.icon;
            icon.enabled = true;
        }
        else
        {
            Debug.LogWarning("Неверный тип предмета для этого слота!");
        }
    }

    // Метод для очистки слота
    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    // Метод для получения текущего предмета
    public Item GetCurrentItem()
    {
        return currentItem;
    }
}
