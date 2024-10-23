using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningWeapon : Weapon
{
    public GameObject lightningPrefab; // Префаб молнии
    public int lightningCount = 5; // Количество молний за раз

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpawnLightning()); // Запускаем корутину для спавна молний
    }

    private IEnumerator SpawnLightning()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / attackSpeed); // Ждем перед следующим спавном
            if (attackTimer <= 0f) // Проверяем, можно ли атаковать
            {
                for (int i = 0; i < lightningCount; i++)
                {
                    SpawnLightningBolt();
                }
                attackTimer = 1f / attackSpeed; // Устанавливаем таймер атаки
            }
        }
    }

    private void SpawnLightningBolt()
    {
        Vector2 randomPosition;

        // Ищем подходящую позицию, пока не найдем ее
        do
        {
            randomPosition = (Vector2)transform.position + Random.insideUnitCircle * attackRange; // Заменяем spawnRadius на attackRange
        } while (IsPositionBlocked(randomPosition)); // Проверяем, заблокирована ли позиция

        GameObject spawnedLightning = Instantiate(lightningPrefab, randomPosition, Quaternion.identity);
        LightningBehaviour lightningBehaviour = spawnedLightning.AddComponent<LightningBehaviour>(); // Добавляем поведение молнии
        lightningBehaviour.SetDamage((int)CalculateDamage()); // Устанавливаем урон молнии
    }

    private bool IsPositionBlocked(Vector2 position)
    {
        // Проверяем, есть ли объекты с тегом "Wall" в радиусе 0.1 от позиции
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, LayerMask.GetMask("Wall"));
        return hit != null; // Если hit не null, значит на позиции есть объект с тегом Wall
    }

    protected override void PerformAttack()
    {
        // Атака будет выполнена через корутину, поэтому ничего не делаем здесь
    }
}

public class LightningBehaviour : MonoBehaviour
{
    private int damage; // Урон молнии
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>(); // Словарь для отслеживания времени последней атаки
    private float attackCooldown = 0.5f; // Время между атаками по одному врагу (1 секунда)

    private void Start()
    {
        Destroy(gameObject, 3f); // Уничтожаем молнию через 3 секунды
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameObject enemy = collision.gameObject;
            if (CanAttackEnemy(enemy)) // Проверяем, можем ли атаковать врага
            {
                collision.GetComponent<Enemy>().TakeDamage(damage); // Наносим урон врагу
                UpdateLastAttackTime(enemy); // Обновляем время последней атаки
            }
        }
    }

    public void SetDamage(int lightningDamage)
    {
        damage = lightningDamage; // Устанавливаем урон
    }

    // Проверка, можно ли атаковать врага (учитывая время последней атаки)
    private bool CanAttackEnemy(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            float timeSinceLastAttack = Time.time - lastAttackTimes[enemy];
            return timeSinceLastAttack >= attackCooldown; // Проверяем, прошло ли достаточно времени
        }
        return true; // Если враг ещё не атакован, можем атаковать
    }

    // Обновление времени последней атаки
    private void UpdateLastAttackTime(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            lastAttackTimes[enemy] = Time.time; // Обновляем время последней атаки
        }
        else
        {
            lastAttackTimes.Add(enemy, Time.time); // Добавляем врага в словарь
        }
    }
}
