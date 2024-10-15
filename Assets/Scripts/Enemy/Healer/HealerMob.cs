using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerMob : Enemy
{
    public GameObject healAnimationPrefab; // ������ �������� ����
    public float healRadius = 3f; // ������ ��� ������ ������
    public float healAmountPercent = 0.2f; // 20% �� ������������� ��������
    public float healInterval = 5f; // �������� ���� � ��������
    public float castTime = 2f; // ����� ����������

    private Animator animator;
    private float originalMoveSpeed;
    private float lastHealTime = 0f; // ����� ���������� ����
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

        if (Time.time >= lastHealTime + healInterval && !isCasting)
        {
            StartCoroutine(CastHeal()); // ��������� ��������
        }
    }

    private IEnumerator CastHeal()
    {
        isCasting = true; // ��� �������� ���������
        animator.SetBool("isCasting", true); // ��������� ��������� � Animator

        // ������������� ����
        enemyMoveSpeed = 0f; // ��������� �������� ����

        // ���� 0.5 ������� ����� ��������� �������� ����
        yield return new WaitForSeconds(0.5f);

        // �������� ���������� ������� �������� ����
        GameObject healAnimation = null;
        if (healAnimationPrefab != null)
        {
            healAnimation = Instantiate(healAnimationPrefab, transform.position, Quaternion.identity);
        }

        // ���� ����� �����
        yield return new WaitForSeconds(castTime - 0.5f); // �������� 0.5 ������� �� ������ ������� �����

        // ����� ������ ����� ���������� �����
        HealEnemies();

        // ����� ���������� �������� � ����, ��������������� ��������
        enemyMoveSpeed = originalMoveSpeed; // ��������������� �������� ��������
        lastHealTime = Time.time; // ��������� ����� ���������� ����
        isCasting = false; // ��������� ����
        animator.SetBool("isCasting", false); // ���������� �������� � Animator

        // ������� �������� ���� ����� ����������
        if (healAnimation != null)
        {
            Destroy(healAnimation); // ������� �������� ����
        }
    }


    private void HealEnemies()
    {
        Collider2D[] enemiesToHeal = Physics2D.OverlapCircleAll(transform.position, healRadius);

        foreach (Collider2D enemyCollider in enemiesToHeal)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null && !enemy.IsDead) // �������� �� �������� �����
                {
                    float healAmount = enemy.maxHealth * healAmountPercent; // ������������ ���������� �������

                    // ����� ���������� � �������� �� ����
                    Debug.Log($"{enemy.gameObject.name} - HP �� ����: {enemy.currentHealth}"); // HP �� ����

                    // ��������� �������
                    enemy.Heal(healAmount); // ����� ������ ����

                    // ������, ����� �������� ���������� �������� ��������, ����� ������ ������� ��� �� enemy
                    Debug.Log($"{enemy.gameObject.name} - ���: {healAmount}, HP ������: {enemy.currentHealth}"); // HP ����� ����
                }
            }
        }
    }


    // ����� ��� ��������� ������� � ���������
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRadius);
    }
}
