    using UnityEngine;
    using System.Collections.Generic;

    public abstract class Weapon : MonoBehaviour
    {
        [Header("Weapon Stats")]
        public float damage = 10f; // Пример начального урона
        public float criticalDamage = 20f; // Пример критического урона
        public float criticalChance = 0.1f; // Пример шанса критического удара 10%
        public float attackSpeed = 1f; // Скорость атаки
        public float rotationSpeed; // Скорость вращения снарядов
        public float projectileSpeed; // Скорость снарядов
   
        // Словарь для хранения времени последнего удара по врагам
        private Dictionary<GameObject, float> lastHitTimes = new Dictionary<GameObject, float>();
        public float hitCooldown = 1f; // Время в секундах между ударами по одному врагу

        // Внутренний таймер для контроля атаки
        protected float attackTimer;
    

        protected virtual void Start()
        {  
            attackTimer = 1f; // Устанавливаем таймер атаки в 0          
        }

        // Метод для атаки
        public virtual void Attack()
        {
            if (attackTimer <= 0f) // Проверяем, можно ли атаковать
            {
                // Логика атаки (например, создание снаряда)
                PerformAttack();
                attackTimer = 1f / attackSpeed; // Устанавливаем таймер атаки в зависимости от скорости
            }
        }

        // Метод для фактической реализации атаки
        protected virtual void PerformAttack()
        {
            float finalDamage = CalculateDamage(); // Вычисляем финальный урон
            bool isCriticalHit = finalDamage == criticalDamage; // Проверяем, был ли это критический удар

            // Формируем сообщение о нанесенном уроне
            string damageMessage = isCriticalHit
                ? $"Крит урон {criticalChance * 100}% Урон {damage} Общий нанесенный урон {finalDamage}"
                : $"Урон {finalDamage}";

            Debug.Log(damageMessage); // Выводим сообщение в консоль
        }

        // Метод для обработки попадания снаряда
        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null && CanHitEnemy(enemy.gameObject)) // Проверяем, можно ли нанести урон
                {
                    float finalDamage = CalculateDamage(); // Рассчитываем финальный урон
                    enemy.TakeDamage((int)finalDamage); // Наносим урон врагу
                    lastHitTimes[enemy.gameObject] = Time.time; // Обновляем время последнего удара
                }
            }
        }

        // Проверяем, можно ли нанести урон врагу
        private bool CanHitEnemy(GameObject enemy)
        {
            if (lastHitTimes.TryGetValue(enemy, out float lastHitTime))
            {
                return (Time.time - lastHitTime) >= hitCooldown; // Проверяем, прошло ли достаточно времени
            }

            // Если враг еще не был поражен, разрешаем удар
            return true;
        }

    // Метод для вычисления урона, включая критический удар
    public float CalculateDamage()
    {
        // Проверяем, произошел ли критический удар
        if (Random.value < criticalChance)
        {
            // Возвращаем общий урон при критическом ударе
            return damage + criticalDamage; // Суммируем обычный урон с критическим
        }
        return damage; // Возвращаем обычный урон
    }




    protected virtual void Update()
        {
            // Обновление таймера атаки
            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime; // Уменьшаем таймер
            }
        }
    
    
}
