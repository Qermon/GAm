using System.Collections;
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

    private bool isCritChanceBuffPurchased = false; // ����, ����������� ��� �� ������ ����
    private bool isCritChanceBuffActive = false; // ����, ����������� ������� �� ����
    private int critChanceBuffCount = 0; // ���������� ���������� ����� ������������ �����

    private bool isCritDamageBuffPurchased = false; // ����, ����������� ��� �� ������ ����
    private bool isCritDamageBuffActive = false; // ����, ����������� ������� �� ����
    private float critDamageBuffCount = 0f; // ������� ��������� ������������� ����������� ����



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

        if (randomValue < criticalChance)
        {
            return criticalDamage; // ����������� ����
        }
        return damage; // ������� ����
    }

    public void PurchaseCritChanceBuff()
    {
        isCritChanceBuffPurchased = true; // ������������� ����, ��� ���� ������
        ActivateCritChanceBuff(); // ���������� ����
    }

    public void ActivateCritChanceBuff()
    {
        if (isCritChanceBuffPurchased && !isCritChanceBuffActive)
        {
            isCritChanceBuffActive = true;
            critChanceBuffCount = 0; // �������� ������� ����������
            StartCoroutine(CritChanceBuffRoutine());
        }
    }

    private IEnumerator CritChanceBuffRoutine()
    {
        // ����������� ���� ������������ ����� ������ �������
        while (isCritChanceBuffActive)
        {
            IncreaseCritChance(0.005f); // ����������� ���� �� 0.5%
            critChanceBuffCount++; // ����������� ������� ����������
            yield return new WaitForSeconds(1f); // ���� 1 �������
        }
    }

    public void DecreaseCritChance(float percentage)
    {
        criticalChance -= percentage; // ��������� ���� ������������ �����
        Debug.Log($"���� ������������ ����� �������� �� {percentage * 100}%. ����� ���� ������������ �����: {criticalChance * 100}%");
    }
    public void CritChanceWave()
    {
        // ��������� ���� ������������ ����� �� ���������� ����������
        DecreaseCritChance(critChanceBuffCount * 0.005f); // ��������� �� ����� ���������� ����������
        isCritChanceBuffActive = false; // ������������ ����
    }

    public void PurchaseCritDamageBuff()
    {
        isCritDamageBuffPurchased = true; // ������������� ����, ��� ���� ������
        ActivateCritDamageBuff(); // ���������� ����
    }

    public void ActivateCritDamageBuff()
    {
        if (isCritDamageBuffPurchased && !isCritDamageBuffActive)
        {
            isCritDamageBuffActive = true;
            critDamageBuffCount = 0; // �������� ������� ����������
            StartCoroutine(CritDamageBuffRoutine());
        }
    }

    private IEnumerator CritDamageBuffRoutine()
    {
        // ����������� ���� ������������ ����� ������ �������
        while (isCritDamageBuffActive)
        {
            IncreaseCritDamage(1f); // ����������� ���� �� 1
            critDamageBuffCount++; // ����������� ������� ����������
            yield return new WaitForSeconds(1f); // ���� 1 �������
        }
    }

    public void DecreaseCritDamage(float amount)
    {
        criticalDamage -= amount; // ��������� ���� ������������ �����
        Debug.Log($" ����������� ���� �������� �� {amount}%. ����� ����������� ����: {criticalDamage}%");
    }

    public void CritDamageWave()
    {
        DecreaseCritDamage(critDamageBuffCount); // ��������� �� ����� ���������� ����������
        isCritDamageBuffActive = false; // ������������ ����
        critDamageBuffCount = 0f;
    }

}
