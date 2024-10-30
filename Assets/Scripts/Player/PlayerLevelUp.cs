using UnityEngine;
using TMPro; // ��� ������������� TMP_Text

public class PlayerLevelUp : MonoBehaviour
{
    public int currentLevel = 1;      // ������� ������� ������
    public int currentExperience = 0; // ������� ���� ������
    public int experienceToNextLevel = 100; // ���� ��� �������� �� ��������� �������

    public TMP_Text levelText; // ������ �� UI Text �������

    void Start()
    {
        UpdateLevel(); // �������������� ������� � ������
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount; // ����������� ����

        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp(); // ��������� � �������� ������� �����
        }
    }

    private void LevelUp()
    {
        currentLevel++; // ����������� �������
        currentExperience -= experienceToNextLevel; // ��������� ���� �� ����������� ���������� ��� ���������� ������

        Debug.Log("�����������! �� �������� ������: " + currentLevel);
        FindObjectOfType<LevelUpMenu>().OpenLevelUpMenu(); // ��������� ���� ���������

        UpdateLevel(); // ��������� ����� ������
    }

    private void UpdateLevel()
    {
        experienceToNextLevel = 100 + (currentLevel - 1) * 100; // ��������� ���� ��� ���������� ������

        // ��������� ����� ������ � UI
        if (levelText != null)
        {
            levelText.text = currentLevel.ToString();
        }
    }
}