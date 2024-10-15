using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerMob : Enemy
{
    public GameObject buffPrefab; // ������ ����, ������� ����� ����������
    public float attackAnimationDuration = 2f; // ������������ �������� �����
    public float buffSpawnCooldown = 5f; // ����� ����� �������
    private Animator animator; // ��������� Animator

    private bool isCasting = false; // ���������� ��� �������� ��������� ��������

    protected override void Start()
    {
        base.Start(); // ����� ������ Start() �� ������ Enemy
        animator = GetComponent<Animator>(); // �������� ��������� Animator
        StartCoroutine(AttackRoutine()); // ��������� �������� ��� �����
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // ������ � ������
            MoveTowardsPlayer();

            // ����� ����� ������
            yield return new WaitForSeconds(buffSpawnCooldown);

            // ����������� �������� �����
            Attack();

            // ��������������� �� ����� �������� �����
            yield return new WaitForSeconds(attackAnimationDuration);

            // �������� ��� ������ ���� ��� ����� �����
            SpawnBuff();
        }
    }

    private void Attack()
    {
        if (!isCasting) // ���������, ��� ������ �� ���� ����
        {
            isCasting = true; // ������������� ���� ��������
            animator.SetBool("isCasting", true); // ��������� ������� ���������� ��� ��������
            StopMoving(); // ������������� ��������

            // �������������� ��������� ����� ���������� �����
            StartCoroutine(ResetCastingState());
        }
    }

    private void StopMoving()
    {
        enemyMoveSpeed = 0; // ������������� ��������
    }

    private IEnumerator ResetCastingState()
    {
        yield return new WaitForSeconds(attackAnimationDuration); // ���� ���������� ��������
        animator.SetBool("isCasting", false); // ���������� ������� ����������
        isCasting = false; // ���������� ���� ��������
        enemyMoveSpeed = 1f; // ��������������� �������� �������� (�������� �� ���� ����������� ��������)
    }

    private void SpawnBuff()
    {
        // ����� ����
        GameObject buff = Instantiate(buffPrefab, transform.position, Quaternion.identity);
        Buff buffScript = buff.GetComponent<Buff>(); // �������� ��������� ������� ����
        if (buffScript != null)
        {
            StartCoroutine(buffScript.DestroyAfterTime(2f)); // ������� ��� ����� 5 ������
        }
    }

    // ����� ��� �������� � ������ (����������� �� Enemy)
    protected override void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyMoveSpeed * Time.deltaTime);
            FlipSprite(direction); // ����� ��� �������� �������
        }
    }
}
