using UnityEngine;

public class ExperienceItem : MonoBehaviour
{
    public int experienceAmount = 20; // Количество опыта, которое получит игрок

    void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, если игрок столкнулся с предметом
        PlayerLevelUp player = other.GetComponent<PlayerLevelUp>();
        if (player != null)
        {
            // Увеличиваем опыт игрока
            player.GainExperience(experienceAmount);

            // Уничтожаем предмет
            Destroy(gameObject);
        }
    }
}
