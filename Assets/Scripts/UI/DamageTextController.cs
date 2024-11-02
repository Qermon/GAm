using UnityEngine;
using TMPro;

public class DamageTextController : MonoBehaviour
{
    public float moveUpSpeed = 1.0f;        // Скорость перемещения текста вверх
    public float duration = 1.0f;           // Время, через которое текст исчезает
    public Color criticalHitColor = Color.red; // Цвет для критического урона
    public Color normalHitColor = Color.white; // Цвет для обычного урона

    private TextMeshProUGUI damageText;
    private float timer;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>(); // Получаем компонент
    }

    public void SetDamage(int damage, bool isCriticalHit)
    {
        if (damageText != null)
        {
            damageText.text = damage.ToString(); // Установка текста урона
            damageText.color = isCriticalHit ? criticalHitColor : normalHitColor; // Установка цвета
        }
        else
        {
            Debug.LogError("Damage text component is null!");
        }
    }

    private void Update()
    {
        // Поднимаем текст вверх
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime);

        // Запускаем таймер для удаления текста через duration
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
