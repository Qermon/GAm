using UnityEngine;
using UnityEngine.UI;

public class ExperienceBarImage : MonoBehaviour
{
    public Image experienceBarImage;      // Ссылка на изображение полоски опыта
    public PlayerLevelUp playerLevelUp;   // Ссылка на скрипт игрока

    void Start()
    {
        StartCoroutine(FindPlayerCoroutine()); // Запускаем корутину для поиска игрока
    }

    public void RestartSkript()
    {
        GameObject playerObject = GameObject.FindWithTag("Player"); 
        playerLevelUp = playerObject.GetComponent<PlayerLevelUp>();
        UpdateExperienceBar();
    }

    private System.Collections.IEnumerator FindPlayerCoroutine()
    {
        // Ждём, пока игрок появится на сцене
        while (playerLevelUp == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerLevelUp = playerObject.GetComponent<PlayerLevelUp>();
            }

            yield return new WaitForSeconds(0.5f); // Повторяем проверку каждые полсекунды
        }

        UpdateExperienceBar(); // Обновляем полоску после нахождения игрока
    }

    void Update()
    {
        if (playerLevelUp != null)
        {
            UpdateExperienceBar(); // Обновляем полоску каждый кадр, если игрок найден
        }
    }

    // Метод для обновления состояния полоски
    private void UpdateExperienceBar()
    {
        // Проверка, чтобы избежать ошибок, если playerLevelUp ещё не найден
        if (playerLevelUp != null)
        {
            float fillAmount = (float)playerLevelUp.currentExperience / playerLevelUp.experienceToNextLevel;
            experienceBarImage.fillAmount = fillAmount;
        }
    }
}
