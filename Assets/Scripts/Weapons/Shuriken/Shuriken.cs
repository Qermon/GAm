using UnityEngine;

public class Shuriken : Weapon
{
    public GameObject shurikenPrefab; // Префаб сюрикена
    public int shurikenCount = 5; // Количество сюрикенов
    public float rotationRadius = 5f; // Радиус вращения вокруг игрока

    private GameObject[] shurikens; // Массив сюрикенов

    protected override void Start()
    {
        base.Start();

        // Создаем сюрикены
        shurikens = new GameObject[shurikenCount];
        for (int i = 0; i < shurikenCount; i++)
        {
            shurikens[i] = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
            shurikens[i].transform.parent = transform; // Сделать игрока родителем
            shurikens[i].transform.localPosition = new Vector3(Mathf.Cos((360f / shurikenCount) * i * Mathf.Deg2Rad) * rotationRadius,
                                                                Mathf.Sin((360f / shurikenCount) * i * Mathf.Deg2Rad) * rotationRadius, 0);
            Collider2D collider = shurikens[i].AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // Сделать коллайдер триггером
            collider.tag = "Weapon"; // Установить тег для триггера

            // Добавляем компонент для обработки столкновений
            ShurikenCollision shurikenCollision = shurikens[i].AddComponent<ShurikenCollision>();
            shurikenCollision.weapon = this; // Передаем ссылку на текущее оружие
        }
    }

    protected override void Update() // Добавлено ключевое слово override
    {
        base.Update(); // Вызов метода Update() из базового класса

        // Вращаем сюрикены вокруг игрока
        for (int i = 0; i < shurikenCount; i++)
        {
            float angle = Time.time * rotationSpeed + (360f / shurikenCount) * i; // Учитываем время и индекс
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * rotationRadius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * rotationRadius;

            // Обновляем позицию сюрикена
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                float finalDamage = weapon.CalculateDamage(); // Рассчитываем финальный урон
                enemy.TakeDamage((int)finalDamage); // Наносим урон врагу
                Debug.Log("Урон нанесён: " + finalDamage);
               
            }
        }
    }
}
