using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffMob : Enemy
{
    public GameObject healAnimationPrefab; // ������ �������� �����
    public float buffRadius = 3f; // ������ ��� ������ ������
    public float damageBonusPercent = 0.5f; // 50% ����� � �����
    public float buffInterval = 5f; // �������� ���� � ��������
    public float castTime = 2f; // ����� ����������
    public float buffDuration = 10f; // ������������ ���� � ��������

    private Animator animator;
    private float originalMoveSpeed;
    private float lastBuffTime = 0f; // ����� ���������� ����
    private bool isCasting = false; // ������ ����������

    protected override void Start()
    {
        base.Start(); // ����� ������ Start() �� Enemy
        animator = GetComponent<Animator>();
        originalMoveSpeed = enemyMoveSpeed; // ������������� �������� ��������
    }

    protected override void Update()
    {
        base.Update();

        if (Time.time >= lastBuffTime + buffInterval && !isCasting)
        {
            StartCoroutine(CastBuff()); // ��������� ��������
        }
    }

    private IEnumerator CastBuff()
    {
        isCasting = true; // ��� �������� ���������
        animator.SetBool("isCasting", true); // ��������� ��������� � Animator

        // ������������� ����
        enemyMoveSpeed = 0f; // ��������� �������� ����

        // ���� 0.5 ������� ����� ��������� �������� ����
        yield return new WaitForSeconds(0.5f);

        

        GameObject healAnimation = null;
        if (healAnimationPrefab != null)
        {
            healAnimation = Instantiate(healAnimationPrefab, transform.position, Quaternion.identity);
        }

        // ���� ����� �����
        yield return new WaitForSeconds(castTime - 0.5f); // �������� 0.5 ������� �� ������ ������� �����

        // ������ ������ ����� ���������� �����
        BuffEnemies();

        // ����� ���������� �������� � ����, ��������������� ��������
        enemyMoveSpeed = originalMoveSpeed; // ��������������� �������� ��������
        lastBuffTime = Time.time; // ��������� ����� ���������� ����
        isCasting = false; // ��������� ����
        animator.SetBool("isCasting", false); // ���������� �������� � Animator

        // ������� �������� ���� ����� ����������
        if (healAnimation != null)
        {
            Destroy(healAnimation); // ������� �������� ����
        }
        // Destroy(buffAnimation); // Uncomment if you have a buff animation prefab
    }

    private void BuffEnemies()
    {
        Collider2D[] alliesToBuff = Physics2D.OverlapCircleAll(transform.position, buffRadius);

        foreach (Collider2D allyCollider in alliesToBuff)
        {
            if (allyCollider.CompareTag("Enemy")) // �������� �� ��� ��� ���������
            {
                Enemy enemy = allyCollider.GetComponent<Enemy>();
                if (enemy != null && !enemy.IsDead) // �������� �� �������� �����
                {
                    float originalDamage = enemy.damage; // ��������� ������������ ����
                    float buffedDamage = originalDamage * (1 + damageBonusPercent); // ������������ ����������� ����

                    enemy.SetDamage(buffedDamage); // ������������� ����� ���� (�������� ����� SetDamage � Enemy)

                    // ����� ���������� � �����
                    Debug.Log($"{enemy.gameObject.name} - ����� � �����: {buffedDamage}, ������������ ����: {originalDamage}");

                    // ��������� �������� ��� �������������� ������������� �����
                    StartCoroutine(RestoreOriginalDamage(enemy, originalDamage, buffDuration));
                }
            }
        }
    }

    private IEnumerator RestoreOriginalDamage(Enemy enemy, float originalDamage, float duration)
    {
        yield return new WaitForSeconds(duration); // ���� 10 ������

        enemy.SetDamage(originalDamage); // ��������������� ������������ ����
        Debug.Log($"{enemy.gameObject.name} - ���� ������������: {originalDamage}");
    }

    // ����� ��� ��������� ������� � ���������
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue; // ���� �������
        Gizmos.DrawWireSphere(transform.position, buffRadius);
    }
}
