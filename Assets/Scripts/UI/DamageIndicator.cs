using System.Collections; // Добавьте это
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image damageImage; // Ссылка на индикатор урона
    public float flashDuration = 0.2f; // Длительность мигания
    private Coroutine flashCoroutine;

    // Метод для вызова, когда игрок получает урон
    public void ShowDamageIndicator()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashDamageIndicator());
    }

    private IEnumerator FlashDamageIndicator()
    {
        damageImage.enabled = true;
        Color originalColor = damageImage.color;

        // Установка временного цвета с полупрозрачностью
        damageImage.color = new Color(1, 0, 0, 0.05f); // Полупрозрачный красный

        yield return new WaitForSeconds(flashDuration);

        // Возвращаем оригинальный цвет (включая альфа-канал)
        damageImage.color = originalColor;
        damageImage.enabled = false; // Скрываем индикатор
    }

}
