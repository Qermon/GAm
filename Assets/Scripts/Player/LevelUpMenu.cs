using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;


    // ������ ������, ������� ����� ��������
    public Weapon[] weaponsToBuff;


    public ShurikenManager abilityOne;
    public KnifeController abilityTwo;
    public LightningWeapon abilityThree;
    public BoomerangController abilityFour;
    public FireBallController abilityFive;

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        
        abilityOne.enabled = false;
        abilityTwo.enabled = false;
        abilityThree.enabled = false;
        abilityFour.enabled = false;
        abilityFive.enabled = false;
        


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

            case 6: // ��������� ����������� ��������
                ActivateAbility(abilityOne);
                break;

            case 7: // ��������� ����������� ����
                ActivateAbility(abilityTwo);
                break;

            case 8: // ��������� ����������� ������
                ActivateAbility(abilityThree);
                break;

            case 9: // ��������� ����������� ���������
                ActivateAbility(abilityFour);
                break;

            case 10: // ��������� ����������� ��������� ����
                ActivateAbility(abilityFive);
                break;

        }

        CloseLevelUpMenu();
       
    }

    private void ActivateAbility(MonoBehaviour ability)
    {
        if (ability != null)
        {
            ability.enabled = true; // ���������� �����������
            Debug.Log(ability.GetType().Name + " ������������.");
        }
        else
        {
            Debug.LogError("����������� �� ���������.");
        }
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