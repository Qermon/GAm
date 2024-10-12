using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel; // ������ ��� ������
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth; // ������ �� ����� PlayerHealth

    private void Start()
    {
        // ������� ���������� PlayerMovement � PlayerHealth �� ������� ������
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    public void OpenLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true); // ���������� ������
            Debug.Log("������ Level Up �������.");
            Time.timeScale = 0; // ������������� �����
        }
        else
        {
            Debug.LogError("������ Level Up �� ��������� � ����������.");
        }
    }

    public void ChooseUpgrade(int choice)
    {
        switch (choice)
        {
            case 1: // ���������� ��������
                if (playerMovement != null)
                {
                    playerMovement.moveSpeed += 2; // ��������, ����������� �������� �� 2
                    Debug.Log("�������� ��������� ��: " + playerMovement.moveSpeed);
                }
                break;

            case 2: // ���������� ��������
                if (playerHealth != null)
                {
                    playerHealth.maxHealth += 20; // ����������� ������������ �������� �� 20
                    playerHealth.currentHealth = playerHealth.maxHealth; // ��������������� ������� �������� �� ���������
                    playerHealth.UpdateHealthUI(); // ��������� UI ������� ��������
                    Debug.Log("������������ �������� ��������� ��: " + playerHealth.maxHealth);
                }
                break;

            case 3: // ���������� �����
                // ������ ���������� ����� (���������� �������� ���� ������ �����)
                break;
        }

        CloseLevelUpMenu(); // ��������� ���� ����� ������
    }

    public void CloseLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false); // ������������ ������
            Debug.Log("������ Level Up �������.");
            Time.timeScale = 1; // ���������� �����
        }
    }
}