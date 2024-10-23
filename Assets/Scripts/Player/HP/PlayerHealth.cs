using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 1000; // Максимальное здоровье
    public int currentHealth; // Текущее здоровье
    public float regen = 0.01f; // Количество здоровья, восстанавливаемого каждую секунду
    private bool isRegenerating = false; // Флаг для отслеживания регенерации
    public HealthBar healthBar; // Ссылка на компонент полоски здоровья
    public Animator animator; // Ссылка на компонент Animator
    public int defense = 10; // Уровень защиты игрока (0-200)
    private const int maxDefense = 200; // Максимальный уровень защиты
    private const float maxDamageReduction = 0.8f; // Максимальное уменьшение урона (80%)
    public float investment = 0; // Значение инвестиций
    public float lifesteal = 0; // Значение вампиризма
    public float pickupRadius = 1f; // Радиус сбора предметов
    public int luck = 0; // Уровень удачи

    private CircleCollider2D collectionRadius; // Ссылка на триггер-коллайдер для сбора предметов
    void Start()
    {
        currentHealth = maxHealth;

        // Добавление радиуса сбора опыта
        collectionRadius = gameObject.AddComponent<CircleCollider2D>();
        collectionRadius.isTrigger = true; // Чтобы это был триггер
        collectionRadius.radius = pickupRadius; // Установите начальный радиус сбора

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

    // Метод для увеличения регенерации здоровья
    public void IncreaseHealthRegen(float percentage)
    {
        float increaseAmount = regen * percentage;
        regen += increaseAmount; // Увеличиваем скорость регенерации здоровья
        Debug.Log($"Скорость регенерации здоровья увеличена на {percentage}. Новая скорость регенерации: {regen}");
    }

    public void IncreaseArmor(int amount)
    {
        defense += amount; // Увеличиваем броню
        Debug.Log($"Броня увеличена на {amount}. Текущая броня: {defense}");
    }
    public void IncreaseMaxHealth(float percentage)
    {
        int increaseAmount = Mathf.FloorToInt(maxHealth * percentage); // Рассчитываем увеличение на основе процента от текущего maxHealth
        maxHealth += increaseAmount; // Увеличиваем максимальное здоровье
        currentHealth += increaseAmount; // Увеличиваем текущее здоровье на то же количество, чтобы игрок не терял здоровье
        healthBar.SetMaxHealth(maxHealth); // Обновляем максимальное здоровье на UI
        UpdateHealthUI(); // Обновляем текущую полоску здоровья на UI
        Debug.Log($"Максимальное здоровье увеличено на {increaseAmount}. Новое максимальное здоровье: {maxHealth}. Текущее здоровье: {currentHealth}");
    }



    // Метод для увеличения вампиризма
    public void IncreaseLifesteal(float percentage)
    {
        float increaseAmount = lifesteal * percentage;
        lifesteal += increaseAmount; // Увеличиваем вампиризм
        Debug.Log($"Вампиризм увеличен на {increaseAmount}. Текущий вампиризм: {lifesteal}");
    }

    // Метод для увеличения инвестиций
    public void IncreaseInvestment(float amount)
    {
        investment += amount; // Увеличиваем инвестиции
        Debug.Log($"Инвестиции увеличены на {amount}. Текущие инвестиции: {investment}");
    }

   

    // Метод для увеличения удачи
    public void IncreaseLuck(int amount)
    {
        luck += amount; // Увеличиваем уровень удачи
        Debug.Log($"Удача увеличена на {amount}. Текущий уровень удачи: {luck}");
    }

    // Метод для увеличения радиуса сбора предметов
    public void IncreasePickupRadius(float percentage)
    {
        float increaseAmount = pickupRadius * percentage;
        pickupRadius += increaseAmount; // Увеличиваем радиус сбора

        // Удаляем старый коллайдер, если он существует
        if (collectionRadius != null)
        {
            Destroy(collectionRadius);
        }

        // Создаем новый коллайдер
        collectionRadius = gameObject.AddComponent<CircleCollider2D>();
        collectionRadius.isTrigger = true; // Чтобы это был триггер
        collectionRadius.radius = pickupRadius; // Устанавливаем новый радиус

        Debug.Log($"Радиус сбора увеличен на {increaseAmount}. Новый радиус сбора: {pickupRadius}");
    }



    public void AddLifesteal(int amount)
    {
        lifesteal += amount;
        Debug.Log("Вампиризм увеличен на " + amount + "%. Текущий уровень вампиризма: " + lifesteal + "%");
    }

    // Метод для восстановления здоровья при убийстве врага
    public void HealOnKill(int enemyHealth)
    {
        int healAmount = Mathf.FloorToInt(enemyHealth * (lifesteal / 100f));
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
        Debug.Log("Восстановлено " + healAmount + " здоровья за убийство врага. Текущее здоровье: " + currentHealth);
        UpdateHealthUI();
    }

    public float CalculateInvestmentBonus(float currentGold)
    {
        // Фиксированное количество золота за каждые 10 инвестиций
        float fixedGoldBonus = Mathf.Floor(investment / 10) * 10; // Например, 10 золота за каждые 10 инвестиций

        // Процент получения золота в зависимости от уровня инвестиций
        float investmentPercentage = (Mathf.Floor(investment / 10) * 0.02f); // 2% за каждые 10 инвестиций
        float percentageGoldBonus = currentGold * investmentPercentage; // Рассчитываем процентное бонусное золото

        // Итоговое бонусное золото
        float totalBonusGold = fixedGoldBonus + percentageGoldBonus;

        return totalBonusGold;
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

        // Запуск регенерации после получения урона только если здоровье не на максимуме
        if (currentHealth < maxHealth)
        {
            StartHealthRegen();
        }
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
        // Проверяем, не идет ли уже регенерация и меньше ли текущее здоровье максимального
        if (!isRegenerating && currentHealth < maxHealth)
        {
            Debug.Log("Запуск регенерации.");
            isRegenerating = true;
            StartCoroutine(RegenerateHealth());
        }
    }


    // Пассивная регенерация здоровья
    private IEnumerator RegenerateHealth()
    {
        while (isRegenerating)
        {
            if (currentHealth < maxHealth)
            {
                int previousHealth = currentHealth; // Сохраняем текущее здоровье

                // Рассчитываем восстановленное здоровье как процент от maxHealth
                int healAmount = Mathf.FloorToInt(maxHealth * regen);
                currentHealth += healAmount;

                // Убедимся, что текущее здоровье не превышает максимума
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                Debug.Log($"Регенерация: текущее здоровье: {previousHealth} -> {currentHealth} из {maxHealth} (восстановлено {healAmount})");
                UpdateHealthUI(); // Обновляем UI здоровья
            }
            else
            {
                Debug.Log("Здоровье полностью восстановлено. Остановка регенерации.");
                StopHealthRegen();
                yield break; // Останавливаем корутину
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