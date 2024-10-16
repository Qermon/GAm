using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Максимальное здоровье
    public int currentHealth; // Текущее здоровье
    private int lifestealPercent = 0;
    private const int regenRate = 5; // Количество здоровья, восстанавливаемого каждую секунду
    private bool isRegenerating = false; // Флаг для отслеживания регенерации
    public HealthBar healthBar; // Ссылка на компонент полоски здоровья
    public Animator animator; // Ссылка на компонент Animator
    public int defense = 0; // Уровень защиты игрока (0-200)
    private const int maxDefense = 200; // Максимальный уровень защиты
    private const float maxDamageReduction = 0.8f; // Максимальное уменьшение урона (80%)

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth); // Установить максимальное здоровье на полоску
        }
        else
        {
            Debug.LogError("HealthBar reference is missing on PlayerHealth!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator reference is missing on PlayerHealth!");
        }
    }
    public void AddLifesteal(int amount)
    {
        lifestealPercent += amount;
        Debug.Log("Вампиризм увеличен на " + amount + "%. Текущий уровень вампиризма: " + lifestealPercent + "%");
    }

    // Метод для восстановления здоровья при убийстве врага
    public void HealOnKill(int enemyHealth)
    {
        int healAmount = Mathf.FloorToInt(enemyHealth * (lifestealPercent / 100f));
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
        Debug.Log("Восстановлено " + healAmount + " здоровья за убийство врага. Текущее здоровье: " + currentHealth);
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        // Рассчитываем уменьшение урона на основе уровня защиты
        float damageReduction = Mathf.Min(defense / 10 * 0.04f, maxDamageReduction);
        int reducedDamage = Mathf.RoundToInt(damage * (1 - damageReduction));

        currentHealth -= reducedDamage;

        // Ограничиваем текущее здоровье, чтобы оно не уходило ниже 0
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die()); // Запускаем корутину для смерти
        }

        Debug.Log("Игрок получил урон: " + reducedDamage + ", защита уменьшила урон на: " + (damage - reducedDamage));

        UpdateHealthUI(); // Обновляем UI здоровья
    }


    private IEnumerator Die()
    {
        Debug.Log("Player died!");
        gameObject.SetActive(false);
        yield return null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20); // Урон 20, например
        }
    }

    // Регистрация регенерации
    public void StartHealthRegen()
    {
        Debug.Log("Попытка запустить регенерацию.");
        if (!isRegenerating && currentHealth < maxHealth)
        {
            Debug.Log("Запуск регенерации.");
            isRegenerating = true;
            StartCoroutine(RegenerateHealth());
        }
        else
        {
            Debug.Log("Регенерация уже активна или здоровье на максимуме.");
        }
    }

    // Пассивная регенерация здоровья
    private IEnumerator RegenerateHealth()
    {
        while (isRegenerating)
        {
            if (currentHealth < maxHealth)
            {
                int previousHealth = currentHealth; // Сохраняем предыдущее здоровье
                currentHealth += Mathf.FloorToInt(regenRate);
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                Debug.Log($"Регенерация: текущее здоровье: {previousHealth} -> {currentHealth} из {maxHealth}");
                UpdateHealthUI();
            }
            else
            {
                Debug.Log("Здоровье полностью восстановлено. Остановка регенерации.");
                StopHealthRegen();
            }

            yield return new WaitForSeconds(1f); // Регенерация каждую секунду
        }
    }

    // Остановить регенерацию
    public void StopHealthRegen()
    {
        isRegenerating = false;
    }

    public void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
    }

    public void IncreaseDefense(int amount)
    {
        defense = Mathf.Clamp(defense + amount, 0, maxDefense);
        Debug.Log("Защита игрока увеличена до: " + defense);
    }
}