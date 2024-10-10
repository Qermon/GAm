using UnityEngine;
using UnityEngine.UI;

public class ExperienceBarImage : MonoBehaviour
{
    public Image experienceBarImage;      // ������ �� ����������� ������� �����
    public PlayerLevelUp playerLevelUp;   // ������ �� ������ ������

    void Start()
    {
        // �������������� �������� �������
        UpdateExperienceBar();
    }

    void Update()
    {
        // ��������� ������� ������ ����
        UpdateExperienceBar();
    }

    // ����� ��� ���������� ��������� �������
    private void UpdateExperienceBar()
    {
        // ��������� ������� ���������� �������
        float fillAmount = (float)playerLevelUp.currentExperience / playerLevelUp.experienceToNextLevel;
        experienceBarImage.fillAmount = fillAmount;
    }
}
