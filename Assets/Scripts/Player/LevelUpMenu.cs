using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel; // ������ � ������� ���������

    private Player player;
    private PlayerMovement playerMovement;

    void Start()
    {
        // ������� ������ � ����������� ����������
        player = FindObjectOfType<Player>();
        playerMovement = player.GetComponent<PlayerMovement>();

        // �������� ������ � ����������� ��� ������
        levelUpPanel.SetActive(false);
    }

    // ��������� ���� ������ ���������
    public void OpenLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);  // ���������� ������
            Debug.Log("������ Level Up �������.");
            Time.timeScale = 0;  // ������������� �����
        }
        else
        {
            Debug.LogError("������ Level Up �� ��������� � ����������.");
        }
    }


    // ��������� ����
    public void CloseLevelUpMenu()
    {
        Time.timeScale = 1f; // ���������� ���� � ���������� ���������
        levelUpPanel.SetActive(false);
    }

    // �������� ���������
    public void ChooseUpgrade(int choice)
    {
        switch (choice)
        {
            case 1:
                playerMovement.moveSpeed += 1f;
                Debug.Log("�������� ������ ��������� ��: " + playerMovement.moveSpeed);
                break;
            case 2:
                player.IncreaseMaxHealth(20); // ����������� ��������
                break;
            case 3:
                player.IncreaseWeaponDamage(5); // ����������� ���� ������
                break;
            default:
                Debug.Log("�������� �����!");
                break;
        }

        CloseLevelUpMenu(); // ��������� ����
    }
}
