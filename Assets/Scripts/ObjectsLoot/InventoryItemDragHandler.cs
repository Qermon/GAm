using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventorySlot sourceSlot;  // Исходный слот, из которого перетаскивается предмет
    public InventorySlot targetSlot;  // Целевой слот, в который перетаскивается предмет

    private Image draggedItemIcon;  // Иконка перетаскиваемого предмета
    private Vector3 startPosition;  // Исходная позиция объекта
    private CanvasGroup canvasGroup;  // CanvasGroup для управления визуальным состоянием

    private void Start()
    {
        // Инициализация переменных
        draggedItemIcon = GetComponent<Image>();  // Получаем иконку предмета
        canvasGroup = GetComponent<CanvasGroup>();  // Получаем CanvasGroup для управления прозрачностью
    }

    // Начало перетаскивания
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;  // Сохраняем исходную позицию объекта
        canvasGroup.blocksRaycasts = false;  // Отключаем взаимодействие с событиями
        draggedItemIcon.enabled = true;  // Включаем отображение иконки

        // Очищаем исходный слот
        if (sourceSlot != null)
        {
            sourceSlot.ClearSlot();
        }
    }

    // Обработка перетаскивания
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;  // Перемещаем объект за курсором
    }

    // Конец перетаскивания
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;  // Включаем взаимодействие с событиями

        // Проверяем, можем ли мы поместить предмет в целевой слот
        if (targetSlot != null && targetSlot.IsEmpty())
        {
            targetSlot.SetItem(sourceSlot.item);  // Перемещаем предмет в целевой слот
        }
        else
        {
            // Если слот не пуст или нет подходящего слота, возвращаем предмет в исходный слот
            if (sourceSlot != null)
            {
                sourceSlot.SetItem(sourceSlot.item);  // Возвращаем предмет обратно в исходный слот
            }
        }

        draggedItemIcon.enabled = false;  // Отключаем отображение иконки
        transform.position = startPosition;  // Возвращаем предмет на его исходную позицию
    }
}
