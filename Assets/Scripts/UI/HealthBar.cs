using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage; // ������ �� ��������� Image ��� ��������
    public Image barrierBarImage; // ������ �� ��������� Image ��� �������
    private int maxHealth; // ������������ ��������
    private float maxBarrier; // ������������ ������

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        healthBarImage.fillAmount = 1f; // ��������� ��� �� 100%
    }

    public void SetHealth(int health)
    {
        healthBarImage.fillAmount = (float)health / maxHealth; // ��������� ��� � ����������� �� �������� ��������
    }

    public void SetMaxBarrier(int barrier)
    {
        maxBarrier = barrier;
        barrierBarImage.fillAmount = 1f; // ��������� ������ �� 100%
    }

    public void AddBarrier(float barrier)
    {
        maxBarrier += barrier; // ��������� ����� ������
        // ��������� UI �������
        barrierBarImage.fillAmount = maxBarrier / maxHealth; // ��� ������ ������ ��� ������������
    }

    public void SetBarrier(int barrier)
    {
        barrierBarImage.fillAmount = (float)barrier / maxBarrier; // ��������� ������ � ����������� �� �������� �������� �������
    }
}
