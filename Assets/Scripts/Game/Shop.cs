using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public Button closeButton; // Кнопка закрытия магазина
    public TextMeshProUGUI playerStatsText; // Текст для отображения статов

    private PlayerHealth playerHealth; // Ссылка на скрипт PlayerHealth игрока
    private PlayerMovement playerMovement; // Ссылка на скрипт PlayerMovement игрока

    private void Start()
    {
        // Инициализация
        shopPanel.SetActive(false);

        // Подписка на кнопку закрытия
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }

        // Найдем игрока на сцене и получим ссылки на PlayerHealth и PlayerMovement
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMovement = FindObjectOfType<PlayerMovement>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found in the scene!");
        }
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement not found in the scene!");
        }
    }

    public void OpenShop()
    {
        // Открываем магазин
        shopPanel.SetActive(true);
        Time.timeScale = 0f;  // Ставим игру на паузу
        UpdatePlayerStats(); // Обновляем статы игрока в UI
    }

    public void CloseShop()
    {
        // Закрываем магазин
        shopPanel.SetActive(false);
        Time.timeScale = 1f;  // Возобновляем игру
    }

    private void UpdatePlayerStats()
    {
        if (playerHealth != null && playerStatsText != null && playerMovement != null)
        {
            // Обновляем текст статов на основе текущих параметров игрока, включая скорость
            playerStatsText.text = $"Здоровье: {playerHealth.currentHealth}/{playerHealth.maxHealth}\n" +
                                   $"Защита: {playerHealth.defense}\n" +
                                   $"Вампиризм: {playerHealth.lifesteal}%\n" +
                                   $"Инвестиции: {playerHealth.investment}\n" +
                                   $"Радиус сбора: {playerHealth.pickupRadius}\n" +
                                   $"Удача: {playerHealth.luck}\n" +
                                   $"Скорость движения: {playerMovement.moveSpeed:F1}";
        }
    }
}
