using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int health;
    private const int maxHealth = 100;

    public void Initialize()
    {
        health = maxHealth; // ���������� �������� �� ������������ ��������
        Debug.Log("����� ���������������! ��������: " + health);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0; // ������'� �� ���� ���� ����� ����
        }
        Console.WriteLine($"������� ������� {damage} ���������. ������'�: {health}");
    }

    public bool IsAlive()   
    {
        return health > 0; // ��������, �� ����� �������
    }
}
