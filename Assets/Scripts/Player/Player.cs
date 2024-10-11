using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int health;
    private const int maxHealth = 100;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        health = maxHealth; // ������������� �������� �� ������������ ��������
        Debug.Log("����� ���������������! ��������: " + health);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0; // �������� �� ����� ���� ������ ����
        }
        Debug.Log($"����� ������� {damage} �����. ������� ��������: {health}");
    }

    public bool IsAlive()
    {
        return health > 0; // ���������, ��� �� �����
    }

    // ���������� ������������� ��������
    public void IncreaseMaxHealth(int amount)
    {
        health += amount;
        Debug.Log("����. �������� ��������� �� " + amount + ". ������� ��������: " + health);
    }

    // ���������� ����� ���� ������ ������
    public void IncreaseWeaponDamage(int amount)
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            weapon.IncreaseDamage(amount);
        }
        Debug.Log("���� ���� ������ �������� �� " + amount);
    }
}
