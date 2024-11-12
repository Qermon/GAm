using UnityEngine;
using TMPro; // ��� ������������� TMP_Text

public class PlayerLevelUp : MonoBehaviour
{
    public int currentLevel = 1;      // ������� ������� ������
    public int currentExperience = 0; // ������� ���� ������
    public int experienceToNextLevel = 100; // ���� ��� �������� �� ��������� �������

    public TMP_Text levelText; // ������ �� UI Text �������

    public AudioSource lvlUpSound;

    void Start()
    {

        GameObject lvlUpSoundObject = GameObject.Find("LvlUpSound");

        if (lvlUpSoundObject != null)
        {
            lvlUpSound = lvlUpSoundObject.GetComponent<AudioSource>();
        }

        // ����� ������� � ������ "levelText" � ��������� TMP_Text ����������
        GameObject levelTextObject = GameObject.Find("levelText");
        if (levelTextObject != null)
        {
            levelText = levelTextObject.GetComponent<TMP_Text>();
        }
        else
        {
            Debug.LogError("������ � ������ 'levelText' �� ������ �� �����! ���������, ��� �� ����������.");
        }

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

        if (lvlUpSound != null)
        {
            lvlUpSound.Play();
        }

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
