using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;


    // ������ ������, ������� ����� ��������
    public Weapon[] weaponsToBuff;


    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();

    }

    public void OpenLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
            Debug.Log("������ Level Up �������.");
            Time.timeScale = 0;
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
                    playerMovement.moveSpeed += 2;
                    Debug.Log("�������� ��������� ��: " + playerMovement.moveSpeed);
                }
                break;

            case 2: // ���������� ������������� ��������
                if (playerHealth != null)
                {
                    playerHealth.maxHealth += 20; // ����������� ������������ �������� �� 20
                    playerHealth.UpdateHealthUI();
                    Debug.Log("������������ �������� ��������� ��: " + playerHealth.maxHealth);
                }
                break;

            case 3: // ��������� ����������� ��������
                if (playerHealth != null)
                {
                    playerHealth.StartHealthRegen(); // ���������� �����������
                    Debug.Log("��������� ����������� ������������.");
                }
                break;

            case 4: // ��� ���� ������
                foreach (Weapon weapon in weaponsToBuff)
                {
                    if (weapon != null)
                    {
                        weapon.damage += 10; // ����������� ���� �� 10
                        Debug.Log(weapon.name + " ���� �������� ��: " + weapon.damage);
                    }
                }
                break;

            case 5: // ���������� ������
                if (playerHealth != null)
                {
                    playerHealth.IncreaseDefense(10); // ����������� ������ �� 10
                    Debug.Log("������ ������ ��������� ��: " + playerHealth.defense);
                }
                break;

        }

        CloseLevelUpMenu();
    }

    public void CloseLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
            Debug.Log("������ Level Up �������.");
            Time.timeScale = 1;
        }
    }
}