using UnityEngine;

public class ExperienceItem : MonoBehaviour
{
    public int experienceAmount = 3; // Количество опыта, которое получит игрок
    public int goldAmount = 1;
    public float baseMoveSpeed = 1f; // Начальная скорость
    public float maxMoveSpeed = 5f; // Максимальная скорость
    public float acceleration = 0.5f; // Ускорение
    private float currentSpeed; // Текущая скорость
    private bool isAttracted = false; // Флаг, притягивается ли опыт к игроку
    private Transform player; // Ссылка на игрока

    void Start()
    {
        currentSpeed = baseMoveSpeed; // Устанавливаем начальную скорость
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Если попал в радиус игрока, начинаем притягивать
            isAttracted = true;
            player = other.transform;
        }
    }

    void Update()
    {
        // Если предмет опыта притягивается к игроку, двигаем его к игроку
        if (isAttracted && player != null)
        {
            // Увеличиваем скорость
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxMoveSpeed);

            // Плавно двигаем опыт к игроку
            transform.position = Vector3.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);

            // Проверяем, достиг ли опыт игрока
            if (Vector3.Distance(transform.position, player.position) < 0.1f)
            {
                // Начисляем золото игроку
                PlayerGold playerGold = player.GetComponent<PlayerGold>();
                if (playerGold != null)
                {
                    playerGold.AddGold(goldAmount);
                }

                PlayerLevelUp playerScript = player.GetComponent<PlayerLevelUp>();
                if (playerScript != null)
                {
                    playerScript.GainExperience(experienceAmount);
                    Destroy(gameObject); // Уничтожаем предмет после того, как опыт собран
                }
            }
        }
    }
}
