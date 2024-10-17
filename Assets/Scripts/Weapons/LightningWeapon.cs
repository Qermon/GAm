using UnityEngine;
using System.Collections;

public class LightningWeapon : Weapon
{
    public GameObject lightningPrefab; // Префаб молнии
    public float attackInterval = 1.0f; // Интервал между спавном молний
    public int lightningCount = 5; // Количество молний за раз
    public float spawnRadius = 2f; // Радиус спавна молний

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpawnLightning()); // Запускаем корутину для спавна молний
    }

    private IEnumerator SpawnLightning()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // Ждем перед следующим спавном
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
            randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
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

    private void Start()
    {
        Destroy(gameObject, 3f); // Уничтожаем молнию через 2 секунды
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(damage); // Наносим урон врагу
           
        }
    }

    public void SetDamage(int lightningDamage)
    {
        damage = lightningDamage; // Устанавливаем урон
    }
}