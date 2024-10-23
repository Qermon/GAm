using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : Weapon
{
    public GameObject shurikenPrefab; // Префаб сюрикена
    public int shurikenCount = 5; // Количество сюрикенов

    private GameObject[] shurikens; // Массив сюрикенов

    protected override void Start()
    {
        base.Start();

        if (shurikenPrefab == null)
        {
            return;
        }

        // Создаем сюрикены
        shurikens = new GameObject[shurikenCount];
        for (int i = 0; i < shurikenCount; i++)
        {
            shurikens[i] = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
            if (shurikens[i] == null)
            {
                Debug.LogError($"Сюрикен {i} не был создан! Проверьте префаб.");
                return; // Остановить выполнение, если создание не удалось
            }
            shurikens[i].transform.parent = transform; // Сделать игрока родителем
            shurikens[i].transform.localPosition = new Vector3(Mathf.Cos((360f / shurikenCount) * i * Mathf.Deg2Rad) * attackRange,
                                                                Mathf.Sin((360f / shurikenCount) * i * Mathf.Deg2Rad) * attackRange, 0);
            Collider2D collider = shurikens[i].AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // Сделать коллайдер триггером
            collider.tag = "Weapon"; // Установить тег для триггера

            // Добавляем компонент для обработки столкновений
            ShurikenCollision shurikenCollision = shurikens[i].AddComponent<ShurikenCollision>();
            shurikenCollision.weapon = this; // Передаем ссылку на текущее оружие
        }

        Debug.Log("Все сюрикены успешно созданы.");
    }

    protected override void Update()
    {
        base.Update();

        for (int i = 0; i < shurikenCount; i++)
        {
            if (shurikens == null || shurikens[i] == null) // Проверка на null
            {
                continue;
            }

            // Используем attackSpeed для расчета угла вращения
            float angle = Time.time * attackSpeed * 100 + (360f / shurikenCount) * i; // Умножаем на 100 для получения rotationSpeed
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * attackRange; // Заменено на attackRange
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * attackRange; // Заменено на attackRange

            shurikens[i].transform.localPosition = new Vector3(x, y, 0);
        }
    }

    protected override void PerformAttack()
    {
        // Атака при выполнении метода
        Debug.Log("Атака сюрикена выполнена с уроном: " + CalculateDamage());
    }
}

public class ShurikenCollision : MonoBehaviour
{
    public Weapon weapon; // Ссылка на оружие

    // Словарь для отслеживания времени последней атаки по каждому врагу
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();
    private float attackCooldown = 1f; // Время между атаками по одному и тому же врагу (1 секунда)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && CanAttackEnemy(enemy.gameObject)) // Проверяем, можем ли атаковать
            {
                float finalDamage = weapon.CalculateDamage(); // Рассчитываем финальный урон
                enemy.TakeDamage((int)finalDamage); // Наносим урон врагу
                Debug.Log("Урон нанесён: " + finalDamage);
                UpdateLastAttackTime(enemy.gameObject); // Обновляем время последней атаки
            }
        }
    }

    // Метод для проверки, можем ли мы атаковать врага (на основе времени последней атаки)
    private bool CanAttackEnemy(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            float timeSinceLastAttack = Time.time - lastAttackTimes[enemy];
            return timeSinceLastAttack >= attackCooldown; // Проверяем, прошло ли больше attackCooldown секунд
        }
        return true; // Если атаки по этому врагу еще не было, можем атаковать
    }

    // Метод для обновления времени последней атаки
    private void UpdateLastAttackTime(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            lastAttackTimes[enemy] = Time.time; // Обновляем время последней атаки
        }
        else
        {
            lastAttackTimes.Add(enemy, Time.time); // Добавляем запись о времени атаки, если врага еще нет в словаре
        }
    }
}
