using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f; // Пример начального урона
    public float criticalDamage = 20f; // Урон при критическом ударе
    public float criticalChance = 0.1f; // Шанс критического удара (10%)
    public float attackSpeed = 1f; // Скорость атаки
    public float attackRange; // Дальность атаки
    public float rotationSpeed; // Скорость вращения снарядов
    public float projectileSpeed; // Скорость снарядов

    protected float attackTimer; // Внутренний таймер для контроля атаки

    private bool isCritChanceBuffPurchased = false; // Флаг, указывающий был ли куплен бафф
    private bool isCritChanceBuffActive = false; // Флаг, указывающий активен ли бафф
    private int critChanceBuffCount = 0; // Количество увеличений шанса критического удара

    private bool isCritDamageBuffPurchased = false; // Флаг, указывающий был ли куплен бафф
    private bool isCritDamageBuffActive = false; // Флаг, указывающий активен ли бафф
    private float critDamageBuffCount = 0f; // Сколько процентов увеличивается критический урон



    protected virtual void Start()
    {
        attackTimer = 0f; // Устанавливаем таймер атаки
    }

    public virtual void Attack()
    {
        if (attackTimer <= 0f) // Проверяем, можно ли атаковать
        {
            PerformAttack();
            attackTimer = 1f / attackSpeed; // Перезапускаем таймер атаки
        }
    }

    protected virtual void PerformAttack()
    {
        float finalDamage = CalculateDamage();
        bool isCriticalHit = finalDamage == criticalDamage;

        string damageMessage = isCriticalHit
            ? $"Критический удар! Урон: {finalDamage}"
            : $"Обычный урон: {finalDamage}";

        Debug.Log(damageMessage);
    }

    public void IncreaseDamage(float percentage)
    {
        float increaseAmount = damage * percentage; // Вычисляем увеличение урона на основе процента
        damage += increaseAmount; // Увеличиваем текущий урон
        Debug.Log($"Урон увеличен на {percentage * 100}%. Новый урон: {damage}");
    }

    public void IncreaseCritDamage(float percentage)
    {
        // Увеличиваем критический урон на заданный процент от базового значения
        criticalDamage += percentage; // Здесь percentage - это само значение, которое добавляется к критическому урону
        Debug.Log($"Критический урон увеличен на {percentage}%. Новый критический урон: {criticalDamage}%");
    }

    public void IncreaseCritChance(float percentage)
    {
        criticalChance += percentage; // Увеличиваем шанс критического удара
        Debug.Log($"Шанс критического удара увеличен на {percentage}%. Новый шанс критического удара: {criticalChance}%");
    }

    public void IncreaseAttackSpeed(float percentage)
    {
        float increaseAmount = attackSpeed * percentage; // Вычисляем увеличение скорости атаки на основе процента
        attackSpeed += increaseAmount; // Увеличиваем скорость атаки
        Debug.Log($"Скорость атаки увеличена на {percentage * 100}%. Новая скорость атаки: {attackSpeed}");
    }

    public void IncreaseAttackRange(float percentage)
    {
        float increaseAmount = attackRange * percentage; // Вычисляем увеличение дальности атаки на основе процента
        attackRange += increaseAmount; // Увеличиваем дальность атаки
        Debug.Log($"Дальность атаки увеличена на {percentage * 100}%. Новая дальность атаки: {attackRange}");
    }

    protected virtual void Update()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime; // Уменьшаем таймер
        }
    }

    public float CalculateDamage()
    {
        // Генерируем случайное значение
        float randomValue = Random.value;

        // Логируем шанс критического удара и случайное значение

        if (randomValue < criticalChance)
        {
            return criticalDamage; // Критический урон
        }
        return damage; // Обычный урон
    }

    public void PurchaseCritChanceBuff()
    {
        isCritChanceBuffPurchased = true; // Устанавливаем флаг, что бафф куплен
        ActivateCritChanceBuff(); // Активируем бафф
    }

    public void ActivateCritChanceBuff()
    {
        if (isCritChanceBuffPurchased && !isCritChanceBuffActive)
        {
            isCritChanceBuffActive = true;
            critChanceBuffCount = 0; // Сбросить счетчик увеличений
            StartCoroutine(CritChanceBuffRoutine());
        }
    }

    private IEnumerator CritChanceBuffRoutine()
    {
        // Увеличиваем шанс критического удара каждую секунду
        while (isCritChanceBuffActive)
        {
            IncreaseCritChance(0.005f); // Увеличиваем шанс на 0.5%
            critChanceBuffCount++; // Увеличиваем счетчик увеличений
            yield return new WaitForSeconds(1f); // Ждем 1 секунду
        }
    }

    public void DecreaseCritChance(float percentage)
    {
        criticalChance -= percentage; // Уменьшаем шанс критического удара
        Debug.Log($"Шанс критического удара уменьшен на {percentage * 100}%. Новый шанс критического удара: {criticalChance * 100}%");
    }
    public void CritChanceWave()
    {
        // Уменьшаем шанс критического удара на количество увеличений
        DecreaseCritChance(critChanceBuffCount * 0.005f); // Уменьшаем на общее количество увеличений
        isCritChanceBuffActive = false; // Деактивируем бафф
    }

    public void PurchaseCritDamageBuff()
    {
        isCritDamageBuffPurchased = true; // Устанавливаем флаг, что бафф куплен
        ActivateCritDamageBuff(); // Активируем бафф
    }

    public void ActivateCritDamageBuff()
    {
        if (isCritDamageBuffPurchased && !isCritDamageBuffActive)
        {
            isCritDamageBuffActive = true;
            critDamageBuffCount = 0; // Сбросить счетчик увеличений
            StartCoroutine(CritDamageBuffRoutine());
        }
    }

    private IEnumerator CritDamageBuffRoutine()
    {
        // Увеличиваем шанс критического удара каждую секунду
        while (isCritDamageBuffActive)
        {
            IncreaseCritDamage(1f); // Увеличиваем шанс на 1
            critDamageBuffCount++; // Увеличиваем счетчик увеличений
            yield return new WaitForSeconds(1f); // Ждем 1 секунду
        }
    }

    public void DecreaseCritDamage(float amount)
    {
        criticalDamage -= amount; // Уменьшаем шанс критического удара
        Debug.Log($" Критический урон уменьшен на {amount}%. Новый критический удар: {criticalDamage}%");
    }

    public void CritDamageWave()
    {
        DecreaseCritDamage(critDamageBuffCount); // Уменьшаем на общее количество увеличений
        isCritDamageBuffActive = false; // Деактивируем бафф
        critDamageBuffCount = 0f;
    }

}
