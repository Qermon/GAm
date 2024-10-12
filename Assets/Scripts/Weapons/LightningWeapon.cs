using System.Collections;
using UnityEngine;

public class LightningWeapon : MonoBehaviour
{
    public GameObject lightningPrefab; // Префаб молнии
    public float attackInterval = 1.0f; // Интервал между спавном молний
    public int lightningDamage = 10; // Урон молнии
    public int lightningCount = 5; // Количество молний за раз
    public float spawnRadius = 2f; // Уменьшенный радиус спавна молний

    private void Start()
    {
        StartCoroutine(SpawnLightning()); // Запускаем корутину для спавна молний
    }

    private IEnumerator SpawnLightning()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // Ждем перед следующим спавном
            for (int i = 0; i < lightningCount; i++)
            {
                SpawnLightningBolt();
            }
        }
    }

    private void SpawnLightningBolt()
    {
        // Генерируем случайную позицию в радиусе от игрока
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        GameObject spawnedLightning = Instantiate(lightningPrefab, randomPosition, Quaternion.identity);
        LightningBehaviour lightningBehaviour = spawnedLightning.AddComponent<LightningBehaviour>(); // Добавляем поведение молнии
        lightningBehaviour.SetDamage(lightningDamage); // Устанавливаем урон молнии
    }
}

public class LightningBehaviour : MonoBehaviour
{
    private int damage; // Урон молнии

    private void Start()
    {
        Destroy(gameObject, 2f); // Уничтожаем молнию через 2 секунды
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(damage); // Наносим урон врагу
            Destroy(gameObject); // Уничтожаем молнию после удара
        }
    }

    public void SetDamage(int lightningDamage)
    {
        damage = lightningDamage; // Устанавливаем урон
    }
}
