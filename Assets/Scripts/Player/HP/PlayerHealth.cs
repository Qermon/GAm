using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 1000; // Максимальное здоровье
    public int currentHealth; // Текущее здоровье
    public float regen = 0.01f; // Количество здоровья, восстанавливаемого каждую секунду
    private bool isRegenerating = false; // Флаг для отслеживания регенерации
    public HealthBar healthBar; // Ссылка на компонент полоски здоровья
    [SerializeField] private Image barrierImage; // Полоска барьера, добавленная в инспекторе

    public int defense = 10; // Уровень защиты игрока (0-200)
    private const int maxDefense = 200; // Максимальный уровень защиты
    private const float maxDamageReduction = 0.8f; // Максимальное уменьшение урона (80%)
    public float investment = 0; // Значение инвестиций
    public float lifesteal = 0; // Значение вампиризма
    public float pickupRadius = 1f; // Радиус сбора предметов
    public int luck = 0; // Уровень удачи

    public int shieldAmount = 0; 
    private float shieldPercent; // Процент от максимального здоровья для щита
    private int shieldBuffCount = 0; // Для хранения количества активированных баффов
    public float maxShieldAmount = 0; // Для отслеживания предыдущего значения щита

    private bool shieldOnKillBuffActive = false; // Флаг активности баффа
    private const float shieldChance = 0.05f; // 5% шанс
    private const float shieldPercentage = 0.1f; // 10% от макс. здоровья




    private CircleCollider2D collectionRadius; // Ссылка на триггер-коллайдер для сбора предметов
    void Start()
    {
        // Инициализация текущего здоровья
        currentHealth = maxHealth;
        shieldAmount = 0;
        shieldPercent = 0.25f; // 25% от максимального здоровья
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        collectionRadius = gameObject.AddComponent<CircleCollider2D>();
        collectionRadius.isTrigger = true;
        collectionRadius.radius = pickupRadius;

        if (healthBar == null)
        {
            Debug.LogError("HealthBar reference is missing on PlayerHealth!");
        }

        UpdateBarrierUI(); // Инициализация отображения барьера
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

        TryApplyShieldOnKill(); // Пытаемся добавить щит с 5% шансом
    }

    public float CalculateInvestmentBonus(float currentGold)
    {
        // Фиксированное количество золота за каждые 10 инвестиций
        float fixedGoldBonus = Mathf.Floor(investment / 10) * 10; // Например, 10 золота за каждые 10 инвестиций

        // Процент получения золота в зависимости от уровня инвестиций
        float investmentPercentage = (Mathf.Floor(investment / 10) * 0.01f); // 1% за каждые 10 инвестиций
        float percentageGoldBonus = currentGold * investmentPercentage; // Рассчитываем процентное бонусное золото

        // Итоговое бонусное золото
        float totalBonusGold = fixedGoldBonus + percentageGoldBonus;

        return totalBonusGold;
    }


   
    // Метод для активации щита в начале каждой волны
    public void ActivateShield()
    {
        // Рассчитываем щит в процентах от максимального здоровья
        float shieldPercent = 0.25f; // 25%
        float shieldFromHealth = maxHealth * shieldPercent;

        // Добавляем щит на основе нового расчета
        shieldAmount += Mathf.FloorToInt(shieldFromHealth);
        maxShieldAmount = shieldAmount; // Сохраняем текущее значение щита

        // Выводим информацию о текущем щите
        Debug.Log($"Щит активирован: текущий щит = {shieldAmount} (из них {Mathf.FloorToInt(shieldFromHealth)} от максимального здоровья)");
        UpdateBarrierUI(); // Обновляем визуальное отображение барьера
    }

    // Метод для активации барьера
    public void ActivateBarrier()
    {
     
        UpdateBarrierUI();
        Debug.Log($"Барьера активировано: {shieldAmount}");
    }

    // Метод для обновления полоски барьера
    
    public void UpdateBarrierUI()
    {
        if (barrierImage != null)
        {
            // Если shieldAmount больше предыдущего значения, заполняем полоску полностью
            if (shieldAmount > maxShieldAmount)
            {
                maxShieldAmount = shieldAmount;
                barrierImage.fillAmount = 1f;
            }
            else if (shieldAmount <= 0)
            {
                barrierImage.fillAmount = 0f; // Если щита нет, полоска не отображается
                
            }
            else
            {
                barrierImage.fillAmount = (float)shieldAmount / maxShieldAmount; 
            }

            
        }
        else
        {
            Debug.LogWarning("Полоска барьера не назначена!");
        }

        // Отладочная информация о максимальном значении барьера
        Debug.Log($"Текущее значение барьера: {shieldAmount}, Максимальное значение барьера: {maxShieldAmount}");
    }

    public void TakeDamage(int damage)
    {
        int damageToTake = damage;
        CheckHealth(); // Проверяем текущее здоровье

        // Если щит активен, уменьшаем урон
        if (shieldAmount > 0)
        {
            if (damage <= shieldAmount)
            {
                shieldAmount -= damage;
                damageToTake = 0; // Урон полностью поглощён щитом
            }
            else
            {
                damageToTake -= (int)shieldAmount; // Урон, который проходит через щит
                shieldAmount = 0; // Щит теперь активен
            }
            UpdateBarrierUI(); // Обновляем визуальное отображение барьера
        }

        // Расчет уменьшенного урона на основе защиты
        float damageReduction = Mathf.Min(defense / 10 * 0.04f, 0.8f);
        int reducedDamage = Mathf.RoundToInt(damageToTake * (1 - damageReduction));
        currentHealth -= reducedDamage;

        // Проверка, не упал ли игрок до 0 здоровья
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die());
        }

        // Проверяем, нужно ли установить максимальное здоровье в 0
        if (shieldAmount <= 0) // Проверка, открыт ли магазин
        {
            maxShieldAmount = 0;
        }

        healthBar.SetHealth(currentHealth);
        Debug.Log($"Получен урон: {damageToTake}, Уменьшенный урон: {reducedDamage}, Оставшийся щит: {shieldAmount}");
    
    }

    public void AddShield(int additionalShield)
    {
        shieldAmount += additionalShield;

      
        UpdateBarrierUI();
        Debug.Log($"Барьера добавлено: +{additionalShield}, общий барьер: {shieldAmount}");
    }

    private void CheckHealth()
    {
        // Проверяем, если текущее здоровье меньше или равно 29% от максимального
        if (currentHealth <= maxHealth * 0.29f)
        {
            shieldBuffCount = 0; // Обнуляем количество баффов
            Debug.Log("Shield buff count обнулен, текущее здоровье: " + maxHealth/currentHealth);
        }
    }


  


    public void AddShieldBuff()
    {
        shieldBuffCount++; // Увеличиваем количество активированных баффов
        Debug.Log($"Добавлен бафф щита. Количество баффов: {shieldBuffCount}");
    }

    public void DecreaseShield(int amount)
    {
        shieldAmount -= amount;
        shieldAmount = Mathf.Max(shieldAmount, 0); // Убедитесь, что shieldAmount не меньше 0
        
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

    public void UpdateShield()
    {
        // Сбрасываем щит
        shieldAmount = 0;

        // Рассчитываем новый щит на основе количества активированных баффов
        float shieldPercent = 0.25f; // 25%
        float shieldFromHealth = maxHealth * shieldPercent * shieldBuffCount; // Новый щит на основе активированных баффов
        shieldAmount += Mathf.FloorToInt(shieldFromHealth);
        maxShieldAmount = shieldAmount;

        Debug.Log($"Щит обновлён: текущий щит = {shieldAmount} (из них {Mathf.FloorToInt(shieldFromHealth)} от максимального здоровья на основе {shieldBuffCount} баффов)");
    }

    public void ActivateShieldOnKillBuff()
    {
        shieldOnKillBuffActive = true;
        Debug.Log("Бафф 'Щит при убийстве' активирован!");
    }

    // Метод для применения щита при убийстве
    public void TryApplyShieldOnKill()
    {
        if (shieldOnKillBuffActive && Random.value <= shieldChance)
        {
            int shieldToAdd = Mathf.FloorToInt(maxHealth * shieldPercentage);
            AddShield(shieldToAdd);
            Debug.Log($"Добавлен щит {shieldToAdd} (10% от макс. здоровья) при убийстве врага.");
        }
    }


}