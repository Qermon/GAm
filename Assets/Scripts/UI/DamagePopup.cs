using UnityEngine;
using TMPro; // Импорт для работы с текстом в TextMeshPro

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro textMesh; // Ссылка на текстовый объект
    public float disappearTime = 1f; // Время до исчезновения текста
    public float floatSpeed = 2f; // Скорость, с которой текст поднимается

    private Color textColor; // Цвет текста
    private float disappearTimer; // Таймер для исчезновения
    private Vector3 moveVector; // Вектор для движения текста

    public void Setup(int damageAmount)
    {
        textMesh.text = damageAmount.ToString(); // Устанавливаем текст урона
        textColor = textMesh.color; // Сохраняем начальный цвет текста
        disappearTimer = disappearTime; // Устанавливаем таймер исчезновения
        moveVector = new Vector3(0, floatSpeed, 0); // Определяем вектор движения текста вверх
    }

    private void Update()
    {
        // Поднимаем текст над врагом
        transform.position += moveVector * Time.deltaTime;

        // Уменьшаем прозрачность текста по мере исчезновения
        disappearTimer -= Time.deltaTime;
        if (disappearTimer <= 0)
        {
            // Плавное исчезновение текста
            textColor.a -= Time.deltaTime / disappearTime;
            textMesh.color = textColor;
            if (textColor.a <= 0)
            {
                Destroy(gameObject); // Удаляем текст, когда он полностью исчезает
            }
        }
    }


}
