using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBuff : MonoBehaviour
{
    public float lifetime = 2f; // ����� ����� ����
    public float healPercentage = 0.2f; // ������� �������������� (20%)
    private HashSet<Enemy> affectedEnemies = new HashSet<Enemy>(); // ��� �������� ������, �������� � �������

    private void Start()
    {
        // ��������� �������� ��� �������� ����
        StartCoroutine(DestroyAfterTime(lifetime));
    }

    private void ApplyHealToEnemies()
    {
        // ��������� ������ ���� � ������, ����������� � ��������
        foreach (var enemy in affectedEnemies)
        {
            float healAmount = enemy.maxHealth * healPercentage; // ������������ ���������� ���������������� ��������
            enemy.Heal(healAmount); // ��������������� �������� �����
            Debug.Log($"Buff applied: {enemy.gameObject.name} healed for {healAmount}. Current Health: {enemy.currentHealth}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���������, ���� ������������ � �������� �������� ������
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                affectedEnemies.Add(enemy); // ��������� ����� � ���������
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ������� ����� �� ���������, ����� �� �������� �������
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                affectedEnemies.Remove(enemy); // ������� ����� �� ���������
            }
        }
    }

    public IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time); // ���� �������� �����
        ApplyHealToEnemies(); // ��������� �������������� ����� ������������
        Destroy(gameObject); // ������� ������ ����
    }
}