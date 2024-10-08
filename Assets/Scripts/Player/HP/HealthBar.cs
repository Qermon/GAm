using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage; // ������ �� ��������� Image

    public void SetMaxHealth(int health)
    {
        // ���������� ������������ ��������
        healthBarImage.fillAmount = 1; // ������ ������� ��������
    }

    public void SetHealth(int health)
    {
        // �������� �������� ������� ��������
        healthBarImage.fillAmount = (float)health / 100; // ��������������, ��� maxHealth = 100
    }
}

