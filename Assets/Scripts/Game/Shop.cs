using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public Button closeButton; // ������ �������� ��������
    public TextMeshProUGUI playerStatsText; // ����� ��� ����������� ������

    private PlayerHealth playerHealth; // ������ �� ������ PlayerHealth ������
    private PlayerMovement playerMovement; // ������ �� ������ PlayerMovement ������
    private PlayerGold playerGold; // ������ �� ������ PlayerGold

    private void Start()
    {
        // �������������
        shopPanel.SetActive(false);

        // �������� �� ������ ��������
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }

        // ������ ������ �� ����� � ������� ������ �� PlayerHealth, PlayerMovement � PlayerGold
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerGold = FindObjectOfType<PlayerGold>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found in the scene!");
        }
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement not found in the scene!");
        }
        if (playerGold == null)
        {
            Debug.LogError("PlayerGold not found in the scene!");
        }
    }

    public void OpenShop()
    {
        // ��������� �������
        shopPanel.SetActive(true);
        Time.timeScale = 0f;  // ������ ���� �� �����
        UpdatePlayerStats(); // ��������� ����� ������ � UI
    }

    public void CloseShop()
    {
        // ��������� �������
        shopPanel.SetActive(false);
        Time.timeScale = 1f;  // ������������ ����
        // ��������� ����� ������
        playerGold.OnShopClosed();
    }


    private void UpdatePlayerStats()
    {
        if (playerHealth != null && playerStatsText != null && playerMovement != null)
        {
            // ��������� ����� ������ �� ������ ������� ���������� ������, ������� ��������
            playerStatsText.text = $"��������: {playerHealth.currentHealth}/{playerHealth.maxHealth}\n" +
                                   $"������: {playerHealth.defense}\n" +
                                   $"���������: {playerHealth.lifesteal}%\n" +
                                   $"����������: {playerHealth.investment}\n" +
                                   $"������ �����: {playerHealth.pickupRadius}\n" +
                                   $"�����: {playerHealth.luck}\n" +
                                   $"�������� ��������: {playerMovement.moveSpeed:F1}";
        }
    }
}
