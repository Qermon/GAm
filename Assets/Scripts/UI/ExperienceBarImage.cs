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

        UpdateExperienceBar(); // ��������� ������� ����� ���������� ������
    }

    void Update()
    {
        if (playerLevelUp != null)
        {
            UpdateExperienceBar(); // ��������� ������� ������ ����, ���� ����� ������
        }
    }

    // ����� ��� ���������� ��������� �������
    private void UpdateExperienceBar()
    {
        // ��������, ����� �������� ������, ���� playerLevelUp ��� �� ������
        if (playerLevelUp != null)
        {
            float fillAmount = (float)playerLevelUp.currentExperience / playerLevelUp.experienceToNextLevel;
            experienceBarImage.fillAmount = fillAmount;
        }
    }
}
