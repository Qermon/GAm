using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;

    private void Start()
    {
        // Сохраняем исходный размер кнопки
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Увеличиваем кнопку на 10% при наведении
        transform.localScale = originalScale * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Восстанавливаем исходный размер при выходе
        transform.localScale = originalScale;
    }
}
