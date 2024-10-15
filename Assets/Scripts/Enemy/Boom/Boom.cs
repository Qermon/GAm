using System.Collections;
using UnityEngine;

public class Boom : Enemy
{
    public float explosionRadius = 0.5f;   // ������ ������������ ������
    public float detectionRadius = 2f;     // ������, � ������� ��� ����������� ����� �������
    public float explosionDelay = 2f;      // ����� �������� ����� �������
    public int explosionDamage = 50;       // ���� �� ������

    private bool isExploding = false;      // ��������, ������� �� �����
    private Animator animator;             // ������ �� Animator

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();  // �������� ��������� Animator
    }

    protected override void Update()
    {
        if (isDead || isExploding || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ���� ��� ��������� � ������� ������, ���������� ������� ������
        if (distanceToPlayer <= detectionRadius)
        {
            StartCoroutine(Explode());
        }
        else
        {
            // �������� � ������, ���� �� ��� ������� ������
            animator.SetBool("isMoving", true);  // ���������� �������� ��������
            base.MoveTowardsPlayer();
        }
    }

    // ����� ��� ������
    private IEnumerator Explode()
    {
        isExploding = true;

        // ������������� �������� � �������� �������� Idle
        animator.SetBool("isMoving", false);
        animator.SetTrigger("Idle");

        // �������� ����� �������
        yield return new WaitForSeconds(explosionDelay);

        // ���������� �������� ������
        animator.SetTrigger("ExplodeTrigger");

        // �������, ���� �������� ������ ���������� (������: 0.5 ���, ������� �� ����� ��������)
        yield return new WaitForSeconds(0.5f); // �������������� ������������ �������� ��� ���� ��������

        // ��������� ����� � ������������ ������
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Enemy") || obj.CompareTag("Player"))
            {
                // ��������� �����
                obj.GetComponent<Enemy>()?.TakeDamage(explosionDamage);
                obj.GetComponent<PlayerHealth>()?.TakeDamage(explosionDamage);

                // ������������ �������
                Vector2 direction = (obj.transform.position - transform.position).normalized;
                obj.GetComponent<Rigidbody2D>()?.AddForce(direction * 100f); // ������������
            }
        }

        // ������ ���� ����� ������
        Die();
    }

    // ����� ��� ������������ ������� ������ � ��������� Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // ������ ������
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // ������ ��������
    }
}
