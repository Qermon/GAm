using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage; // ������ �� ��������� Image
    private int maxHealth; // ������������ ��������

    // ����� ��� ��������� ������������� ��������
    public void SetMaxHealth(int health)
    {
        maxHealth = health; // ��������� ������������ ��������
        healthBarImage.fillAmount = 1; // ������ ������� ��������
    }

    // ����� ��� ���������� �������� ��������
    public void SetHealth(int health)
    {
        healthBarImage.fillAmount = (float)health / maxHealth; // ��������� ������� ��������
    }
}
