using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f; // ������ ���������� �����
    public float criticalDamage = 20f; // ���� ��� ����������� �����
    public float criticalChance = 0.1f; // ���� ������������ ����� (10%)
    public float attackSpeed = 1f; // �������� �����
    public float attackRange; // ��������� �����
    public float rotationSpeed; // �������� �������� ��������
    public float projectileSpeed; // �������� ��������

    protected float attackTimer; // ���������� ������ ��� �������� �����

    protected virtual void Start()
    {
        attackTimer = 0f; // ������������� ������ �����
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

    public void IncreaseDamage(float percentage)
    {
        float increaseAmount = damage * percentage; // ��������� ���������� ����� �� ������ ��������
        damage += increaseAmount; // ����������� ������� ����
        Debug.Log($"���� �������� �� {percentage * 100}%. ����� ����: {damage}");
    }

    public void IncreaseCritDamage(float percentage)
    {
        // ����������� ����������� ���� �� �������� ������� �� �������� ��������
        criticalDamage += percentage; // ����� percentage - ��� ���� ��������, ������� ����������� � ������������ �����
        Debug.Log($"����������� ���� �������� �� {percentage}%. ����� ����������� ����: {criticalDamage}%");
    }

    public void IncreaseCritChance(float percentage)
    {
        criticalChance += percentage; // ����������� ���� ������������ �����
        Debug.Log($"���� ������������ ����� �������� �� {percentage}%. ����� ���� ������������ �����: {criticalChance}%");
    }

    public void IncreaseAttackSpeed(float percentage)
    {
        float increaseAmount = attackSpeed * percentage; // ��������� ���������� �������� ����� �� ������ ��������
        attackSpeed += increaseAmount; // ����������� �������� �����
        Debug.Log($"�������� ����� ��������� �� {percentage * 100}%. ����� �������� �����: {attackSpeed}");
    }

    public void IncreaseAttackRange(float percentage)
    {
        float increaseAmount = attackRange * percentage; // ��������� ���������� ��������� ����� �� ������ ��������
        attackRange += increaseAmount; // ����������� ��������� �����
        Debug.Log($"��������� ����� ��������� �� {percentage * 100}%. ����� ��������� �����: {attackRange}");
    }

    protected virtual void Update()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime; // ��������� ������
        }
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
}
