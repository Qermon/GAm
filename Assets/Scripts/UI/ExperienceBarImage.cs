using UnityEngine;
using UnityEngine.UI;

public class ExperienceBarImage : MonoBehaviour
{
    public Image experienceBarImage;      // ������ �� ����������� ������� �����
    public PlayerLevelUp playerLevelUp;   // ������ �� ������ ������

    void Start()
    {
        StartCoroutine(FindPlayerCoroutine()); // ��������� �������� ��� ������ ������
    }

    public void RestartSkript()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        playerLevelUp = playerObject.GetComponent<PlayerLevelUp>();
        UpdateExperienceBar();
    }

    private System.Collections.IEnumerator FindPlayerCoroutine()
    {
        // ���, ���� ����� �������� �� �����
        while (playerLevelUp == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerLevelUp = playerObject.GetComponent<PlayerLevelUp>();
            }

            yield return new WaitForSeconds(0.5f); // ��������� �������� ������ ����������
        }

        experienceBarImage.fillAmount = 0f; // ������������� ��������� �������� ������� � 0
        UpdateExperienceBar(); // ��������� ������� ����� ���������� ������
    }

    void Update()
    {
      
            UpdateExperienceBar(); // ��������� ������� ������ ����, ���� ����� ������
      
    }

    // ����� ��� ���������� ��������� �������
    private void UpdateExperienceBar()
    {
        if (playerLevelUp != null && playerLevelUp.experienceToNextLevel > 0)
        {
            float fillAmount = (float)playerLevelUp.currentExperience / playerLevelUp.experienceToNextLevel;
            fillAmount = Mathf.Clamp01(fillAmount); // ������������ �������� ����� 0 � 1
            experienceBarImage.fillAmount = fillAmount;
        }
    }
}
