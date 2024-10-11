using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;
    public int maxHealth = 100; // ������ maxHealth ����� ����������

    // ������������� ������
    public void Initialize()
    {
        health = maxHealth; // ������������� ������� �������� �� ������������
        Debug.Log("����� ���������������! ��������: " + health);
    }

    // ����� ��� ��������� �����
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0; // �������� �� ����� ���� ���� ����
        }
        Debug.Log($"����� ������� {damage} �����. ������� ��������: {health}");
    }

    // �������� �� ��������� ������
    public bool IsAlive()
    {
        return health > 0;
    }

    // ���������� ������������� ��������
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        health = maxHealth; // ��������������� ������� �������� �� ������ ���������
        Debug.Log($"������������ �������� ��������� ��: {maxHealth}");
    }
}
