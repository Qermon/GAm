using UnityEngine;

public class PlayerLevelUp : MonoBehaviour
{
    public int currentLevel = 1;      // ������� ������� ������
    public int currentExperience = 0; // ������� ���� ������
    public int experienceToNextLevel = 5; // ���� ��� �������� �� ��������� �������

    void Start()
    {
        // ������������� ������ � �����
        UpdateLevel();
    }

    // ����� ��� ��������� �����
    public void GainExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log("�������� �����: " + amount);

        // �������� �� ��������� ������
        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    // ��������� ������
    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel; // ������� ����, ����������� �� �������

        Debug.Log("�����������! �� �������� ������: " + currentLevel);

        // ���������� ����� ���� ��� ���������� ������
        UpdateLevel();
    }

    // ������ ���������� ����� ��� ���������� ������
    private void UpdateLevel()
    {
        if (currentLevel < 20)
        {
            experienceToNextLevel = 5 + (currentLevel - 1) * 10;
        }
        else if (currentLevel < 40)
        {
            experienceToNextLevel += 13;
        }
        else
        {
            experienceToNextLevel += 16;
        }

        Debug.Log("���������� ����� ��� ���������� ������: " + experienceToNextLevel);
    }
}


