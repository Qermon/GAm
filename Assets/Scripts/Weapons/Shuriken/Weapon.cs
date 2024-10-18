using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f; // ������ ���������� �����
    public float criticalDamage = 20f; // ���� ��� ����������� �����
    public float criticalChance = 0.1f; // ���� ������������ ����� (10%)
    public float attackSpeed = 1f; // �������� �����
    public float rotationSpeed; // �������� �������� ��������
    public float projectileSpeed; // �������� ��������

    // ����� ����� ������� �� ������ �����
    private Dictionary<GameObject, float> lastHitTimes = new Dictionary<GameObject, float>();
    public float hitCooldown = 1f; // ����� � �������� ����� ������� �� ������ �����

    protected float attackTimer; // ���������� ������ ��� �������� �����

    protected virtual void Start()
    {
        attackTimer = 1f; // ������������� ������ �����
    }

    public virtual void Attack()
    {
        if (attackTimer <= 0f) // ���������, ����� �� ���������
        {
            PerformAttack();
            attackTimer = 1f / attackSpeed; // ������������� ������ �����
        }
    }

    protected virtual void PerformAttack()
    {
        float finalDamage = CalculateDamage();
        bool isCriticalHit = finalDamage == criticalDamage;

        string damageMessage = isCriticalHit
            ? $"����������� ����! ����: {finalDamage}"
            : $"������� ����: {finalDamage}";

        Debug.Log(damageMessage);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && CanHitEnemy(enemy.gameObject))
            {
                float finalDamage = CalculateDamage();
                enemy.TakeDamage((int)finalDamage);
                lastHitTimes[enemy.gameObject] = Time.time;
            }
        }
    }

    private bool CanHitEnemy(GameObject enemy)
    {
        if (lastHitTimes.TryGetValue(enemy, out float lastHitTime))
        {
            return (Time.time - lastHitTime) >= hitCooldown;
        }
        return true;
    }
    public float CalculateDamage()
    {
        // ���������� ��������� ��������
        float randomValue = Random.value;

        // �������� ���� ������������ ����� � ��������� ��������
        Debug.Log($"���� ������������ �����: {criticalChance * 100}% | ��������� ��������: {randomValue}");

        if (randomValue < criticalChance)
        {
            Debug.Log($"����������� ����! ����: {criticalDamage}");
            return criticalDamage; // ����������� ����
        }

        Debug.Log($"������� ����. ����: {damage}");
        return damage; // ������� ����
    }


    protected virtual void Update()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }
}
