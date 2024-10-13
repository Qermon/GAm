using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : Weapon
{
    public int hitCount = 0; // ������� ���������
    public int maxHits = 5;  // �������� ��������� �� �����������
    public float attackInterval = 1.0f; // �������� ����� �������

    private Dictionary<Collider2D, float> lastAttackTime = new Dictionary<Collider2D, float>(); // ��� ������������ ������� �����

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (CanAttack(collision)) // ���������, ����� �� ���������
            {
               
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage); // ������� ����
                }

                lastAttackTime[collision] = Time.time; // ���������� ����� �����

                if (hitCount >= maxHits) // ���� �������� ������
                {
                    DestroyShuriken();
                }
            }
        }
    }

    private bool CanAttack(Collider2D enemy)
    {
        if (!lastAttackTime.ContainsKey(enemy))
        {
            lastAttackTime[enemy] = Time.time;
            return true;
        }

        return Time.time >= lastAttackTime[enemy] + attackInterval; // ��������� ��������
    }

    private void DestroyShuriken()
    {
        Destroy(gameObject); // ���������� �������
    }
}
