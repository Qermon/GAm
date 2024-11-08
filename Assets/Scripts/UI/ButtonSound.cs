using UnityEngine;
using UnityEngine.UI; // ��� ������ � UI ����������

public class ButtonClickSound : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // ������� ������ AudioSource, ������� ����� �������������� ����
        audioSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();

        // ��������� ���������� ������� ������� ��� ���� ������
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    // �������, ������� ����� ���������� ��� ������� �� ������
    void PlaySound()
    {
        audioSource.Play();
    }
}
