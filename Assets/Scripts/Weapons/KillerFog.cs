using UnityEngine;
using System.Collections;

public class KillerFog : Weapon
{
    public GameObject projectilePrefab; // Префаб снаряда
    public int projectileCount = 4; // Количество снарядов в залпе
    public float projectileDelay = 0.2f; // Задержка между снарядами в одном залпе

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchSalvos()); // Запускаем корутину для выпуска залпов
    }

    private IEnumerator LaunchSalvos()
    {
        while (true) // Бесконечный цикл для постоянного выпуска залпов
        {
            if (IsEnemyInRange()) // Проверяем, есть ли враги в радиусе атаки
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    LaunchProjectile(); // Запускаем снаряд
                    yield return new WaitForSeconds(projectileDelay); // Ждем задержку между снарядами
                }
            }

            yield return new WaitForSeconds(attackSpeed); // Ждем задержку между залпами
        }
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0; // Если есть хотя бы один враг в радиусе атаки, возвращаем true
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // Устанавливаем тег для снаряда
        projectile.AddComponent<ProjectileFog>().Initialize(this); // Добавляем компонент ProjectileFog и передаем ссылку на текущее оружие
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Рисуем радиус атаки в редакторе
    }
}

public class ProjectileFog : MonoBehaviour
{
    private KillerFog weapon; // Ссылка на оружие
    public float speed = 6f; // Скорость снаряда
    private Enemy target; // Целевой враг

    public void Initialize(KillerFog weapon)
    {
        this.weapon = weapon; // Получаем ссылку на KillerFog
        FindRandomTarget(); // Находим случайную цель
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            StartCoroutine(MoveProjectile(direction));
        }
        else
        {
            Destroy(gameObject); // Уничтожаем снаряд, если врагов нет
            Debug.LogWarning("No available enemies; destroying projectile.");
        }
    }

    private void FindRandomTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, weapon.attackRange, LayerMask.GetMask("Mobs", "MobsFly")); // Используем радиус атаки из оружия
        if (enemies.Length > 0)
        {
            target = enemies[Random.Range(0, enemies.Length)].GetComponent<Enemy>(); // Выбираем случайного врага
        }
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        while (true) // Продолжаем двигать снаряд, пока он существует
        {
            if (target == null)
            {
                FindRandomTarget(); // Находим нового врага, если предыдущий уничтожен
                if (target == null)
                {
                    Destroy(gameObject); // Уничтожаем снаряд, если врагов больше нет
                    yield break; // Выходим из корутины
                }
                direction = (target.transform.position - transform.position).normalized; // Обновляем направление
            }

            transform.position += direction * speed * Time.deltaTime; // Двигаем снаряд

            // Поворачиваем снаряд в сторону движения
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Рассчитываем угол поворота
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Поворачиваем снаряд

            yield return null; // Ждем до следующего кадра

            // Проверяем, жив ли враг
            if (target != null && !target.gameObject.activeInHierarchy)
            {
                target = null; // Удаляем ссылку на цель, если она уничтожена
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                float finalDamage = weapon.CalculateDamage(); // Рассчитываем финальный урон
                enemy.TakeDamage((int)finalDamage); // Наносим урон врагу
                Destroy(gameObject); // Уничтожаем снаряд при попадании
            }
        }
    }
}
