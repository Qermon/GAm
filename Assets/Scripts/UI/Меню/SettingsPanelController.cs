using UnityEngine;
using UnityEngine.UI; // ��� ������ � UI ����������

public class SettingsPanelController : MonoBehaviour
{
    // UI �������� ��� ���������
    public Button hardButton;
    public Button normalButton;
    public Button easyButton;

    public Color activeColor = new Color(1f, 1f, 1f, 1f); // ������ ��������������
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.5f); // ����������������
    private Button selectedButton; // ������, ������� ���� �������

    // ������ ��������
    public GameObject settingsPanel; // ������ �� ������ ������ ��������

    private WaveManager waveManager;

    // ���������� ���������
    private void Start()
    {
        // ������������� WaveManager
        waveManager = FindObjectOfType<WaveManager>();

        // ��������� ����������� ���������
        LoadDifficulty();

        // ������������� ����������� ������ ���������
        hardButton.onClick.AddListener(() => SelectButton(hardButton, Hard, 1));
        normalButton.onClick.AddListener(() => SelectButton(normalButton, Normal, 2));
        easyButton.onClick.AddListener(() => SelectButton(easyButton, Easy, 3));
    }

    // ����� ���������
    void SelectButton(Button button, System.Action difficultyAction, int difficultyId)
    {
        // ���� ��������� ������ ��� �� ��, ������ �� ������
        if (selectedButton == button) return;

        // �������� ����� �� ����� ������
        selectedButton = button;
        SetButtonTransparency();
        difficultyAction.Invoke(); // ��������� ��������� ���������

        // ��������� ��������� ���������
        PlayerPrefs.SetInt("Difficulty", difficultyId);
        PlayerPrefs.Save(); // ��������� ���������
    }

    // ������������� ������������ ��� ������
    void SetButtonTransparency()
    {
        hardButton.GetComponent<Image>().color = (selectedButton == hardButton) ? activeColor : inactiveColor;
        normalButton.GetComponent<Image>().color = (selectedButton == normalButton) ? activeColor : inactiveColor;
        easyButton.GetComponent<Image>().color = (selectedButton == easyButton) ? activeColor : inactiveColor;
    }

    // ���������� ��������� "Hard"
    public void Hard()
    {
        waveManager.damageMultiplier = 1.2f;
        waveManager.healthMultiplier = 1.4f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 1.25f;
    }

    // ���������� ��������� "Normal"
    public void Normal()
    {
        waveManager.damageMultiplier = 1.15f;
        waveManager.healthMultiplier = 1.25f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 1.2f;
    }

    // ���������� ��������� "Easy"
    public void Easy()
    {
        waveManager.damageMultiplier = 1.1f;
        waveManager.healthMultiplier = 1.15f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 1.13f;
    }

    // ����� ��� �������� ������ ��������
    public void Exit()
    {
        settingsPanel.SetActive(false); // �������� ������
    }

    // ��������� ����������� ���������
    void LoadDifficulty()
    {
        int difficultyId = PlayerPrefs.GetInt("Difficulty", 2); // ���� ��������� �� ���� ���������, �� ��������� - Normal (2)

        // � ����������� �� ����������� ���������, ���������� ������ ������
        switch (difficultyId)
        {
            case 1:
                SelectButton(hardButton, Hard, 1);
                break;
            case 2:
                SelectButton(normalButton, Normal, 2);
                break;
            case 3:
                SelectButton(easyButton, Easy, 3);
                break;
        }
    }
}
