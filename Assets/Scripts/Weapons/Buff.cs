using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public float lifetime = 2f; // ����� ����� ����
    public float damageMultiplier = 2f; // ��������� ����� (100% ����������)
    private HashSet<Enemy> affectedEnemies = new HashSet<Enemy>(); // ��� �������� ������ � ���� ��������

    private void Start()
    {
        // ��������� �������� ��� �������� ����
        StartCoroutine(DestroyAfterTime(lifetime));
    }

    private void ApplyBuffToEnemies()
    {
        foreach (var enemy in affectedEnemies)
        {
            // ���������� �������� ���� ��� �������
            float originalDamage = enemy.damage;
            // ����������� ���� �����
            enemy.SetDamage(originalDamage * damageMultiplier);
            Debug.Log($"Buff applied: {enemy.gameObject.name} damage increased from {originalDamage} to {enemy.damage}");

            // ��������� �������� ��� �������� ������� ����� ������� �����
            StartCoroutine(RemoveBuffAfterTime(enemy, lifetime, originalDamage));
        }
    }

    private IEnumerator RemoveBuffAfterTime(Enemy enemy, float time, float originalDamage)
    {
        yield return new WaitForSeconds(time); // ���� �������� �����

        // ���������� ���� �����
        enemy.SetDamage(originalDamage);
        Debug.Log($"Buff removed: {enemy.gameObject.name} damage reset to {enemy.damage}");
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
                Debug.Log($"{enemy.gameObject.name} entered buff zone.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ���������, ���� ���� ������� ���� ��������
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                affectedEnemies.Remove(enemy); // ������� ����� �� ���������
                Debug.Log($"{enemy.gameObject.name} exited buff zone.");
            }
        }
    }

    public IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time); // ���� �������� �����
        ApplyBuffToEnemies(); // ��������� ��� ����� ������������
        Destroy(gameObject); // ������� ������ ����
    }
}
