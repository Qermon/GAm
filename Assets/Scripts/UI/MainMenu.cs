using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup mainMenuPanel; // CanvasGroup ��� ������ ����
    public GameObject canvasHero;

    private void Start()
    {
        // �������� ������ ��� ������� ����
        mainMenuPanel.alpha = 1;
        mainMenuPanel.interactable = true;
        mainMenuPanel.blocksRaycasts = true;
        Time.timeScale = 0f; // ������������� ���� ��� �������� ����
    }

    // ����� ��� ������ "������"
    public void OnPlayButtonPressed()
    {
        // ������ ������ ����
        mainMenuPanel.alpha = 0;
        mainMenuPanel.interactable = false;
        mainMenuPanel.blocksRaycasts = false;

        // ��������� ������ CanvasHero
        canvasHero.SetActive(true);

    }

    // ����� ��� ������ "���������" (���� ������)
    public void OnSettingsButtonPressed()
    {
        Debug.Log("��������� ����� ��������� �����.");
    }

    // ����� ��� ������ "�����"
    public void OnExitButtonPressed()
    {
        Application.Quit(); // ���������� ����
        Debug.Log("���� ���������.");
    }
}
