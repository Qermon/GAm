using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f; // Пример начального урона
    public float criticalDamage = 20f; // Урон при критическом ударе
    public float criticalChance = 0.1f; // Шанс критического удара (10%)
    public float attackSpeed = 1f; // Скорость атаки
    public float rotationSpeed; // Скорость вращения снарядов
    public float projectileSpeed; // Скорость снарядов

    // Время между ударами по одному врагу
    private Dictionary<GameObject, float> lastHitTimes = new Dictionary<GameObject, float>();
    public float hitCooldown = 1f; // Время в секундах между ударами по одному врагу

    protected float attackTimer; // Внутренний таймер для контроля атаки

    protected virtual void Start()
    {
        attackTimer = 1f; // Устанавливаем таймер атаки
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

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && CanHitEnemy(enemy.gameObject))
            {
                float finalDamage = CalculateDamage();
                enemy.TakeDamage((int)finalDamage);
                lastHitTimes[enemy.gameObject] = Time.time;
            }
        }
    }

    private bool CanHitEnemy(GameObject enemy)
    {
        if (lastHitTimes.TryGetValue(enemy, out float lastHitTime))
        {
            return (Time.time - lastHitTime) >= hitCooldown;
        }
        return true;
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


    protected virtual void Update()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }
}
