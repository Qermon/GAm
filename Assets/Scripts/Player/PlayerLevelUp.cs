using UnityEngine;

public class PlayerLevelUp : MonoBehaviour
{
    public int currentLevel = 1;      // ������� ������� ������
    public int currentExperience = 0; // ������� ���� ������
    public int experienceToNextLevel = 5; // ���� ��� �������� �� ��������� �������

    void Start()
    {
        UpdateLevel();
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log("�������� �����: " + amount);

        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel;

        Debug.Log("�����������! �� �������� ������: " + currentLevel);

        FindObjectOfType<LevelUpMenu>().OpenLevelUpMenu(); // ��������� ���� ���������

        UpdateLevel();
    }

    private void UpdateLevel()
    {
        experienceToNextLevel = 5 + (currentLevel - 1) * 10;
        Debug.Log("���������� ����� ��� ���������� ������: " + experienceToNextLevel);
    }
}