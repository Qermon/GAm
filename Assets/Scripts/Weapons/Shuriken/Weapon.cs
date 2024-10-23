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
        Debug.Log($"Шанс критического удара: {criticalChance * 100}% | Случайное значение: {randomValue}");

        if (randomValue < criticalChance)
        {
            Debug.Log($"Критический удар! Урон: {criticalDamage}");
            return criticalDamage; // Критический урон
        }

        Debug.Log($"Обычный удар. Урон: {damage}");
        return damage; // Обычный урон
    }
}
