using System.Collections;
using UnityEngine;

public class FirstBoss : Enemy
{
    public float bossAttackRange = 1.0f; // ����������� ��������� �����
    public float bossMoveSpeed = 0.5f; // ����������� �������� ��������
    public int bossGoldAmount = 50; // ���������� ������, ���������� � �����
    public Animator animator; // ������ �� ��������
    public GameObject lightningPrefab; // ������ ������
    public float lightningSpawnRadius = 3.0f; // ������ ������ ������
    public float invulnerabilityDuration = 10.0f; // ������������ ������������

    private Rigidbody2D rb; // ��������� Rigidbody2D ��� ������
    private bool isRegenerating = false; // ���� ��� �����������
    private bool isInvulnerable = false; // ���� ��� ������������
    private Weapon weapon;

    protected override void Start()
    {
        weapon = GetComponent<Weapon>();
        base.Start();
        attackRange = bossAttackRange;
        enemyMoveSpeed = bossMoveSpeed;
        goldAmount = bossGoldAmount;

        // ������������� Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    protected override void Update()
    {
        if (isDead || player == null) return;

        attackTimer -= Time.deltaTime;

        // ���������, ����� �� ������ �����������
        if (!isRegenerating && currentHealth <= maxHealth * 0.5f)
        {
            Debug.Log("���� �������� �����������."); // ���������� ���������
            StartCoroutine(Regeneration());
        }

        // ���� ���� ��������, �� �� ������ ���������
        if (isInvulnerable) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && attackTimer <= 0f)
        {
            // ��������� ������� �����
            animator.SetTrigger("meleeAttack");
            DealDamageToPlayer();
            attackTimer = attackCooldown; // ���������� ������ ��� ������� �����
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    private IEnumerator Regeneration()
    {
        isRegenerating = true;
        isInvulnerable = true; // ������������� ������������

        animator.SetTrigger("regeneration"); // ��������� �������� �����������

        // �������� ��������� ��������
        yield return new WaitForSeconds(1.0f); // ������������ �������� �����������

        // ��������������� ��������
        currentHealth += 20; // ����� ������������ ���������� ������������������ ��������
        Debug.Log("���� ��������������� ��������."); // ���������� ���������

        // ����� ������
        for (int i = 0; i < 5; i++) // ����� 5 ������
        {
            SpawnLightning();
            yield return new WaitForSeconds(0.5f); // �������� ����� ������� ������
        }

        // ���������� ������������
        yield return new WaitForSeconds(invulnerabilityDuration); // ������������ ������������

        isRegenerating = false;
        isInvulnerable = false; // ���� ������ �� ��������
        Debug.Log("���� ������ �� ��������."); // ���������� ���������
    }

    private void SpawnLightning()
    {
        // ��������� ��������� ������� � �������
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * lightningSpawnRadius;
        Instantiate(lightningPrefab, randomPosition, Quaternion.identity); // ������� ������
    }

    protected override void MoveTowardsPlayer()
    {
        if (player != null && !isInvulnerable) // ���� ��������, �� ���������
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.MovePosition((Vector2)transform.position + direction * enemyMoveSpeed * Time.deltaTime);
                FlipSprite(direction);

                // ��������� �������� idle
                animator.SetBool("IsMoving", true);
            }
            else
            {
                // ���� ���� ������ � ������, ������������� ��������
                animator.SetBool("IsMoving", false);
            }
        }
    }

    // �����, ���������� � ����� �������� �����
    public void DealDamageToPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage((int)damage);
            Debug.Log("���� ������� ���� ������!");
        }
    }

    public override void TakeDamage(int damage, bool isCriticalHit, bool isDoubleDamage = false) // ���������, ��� �������� ������������
    {
        if (isDead) return;

        // ��������� ������������ �����
        if (isCriticalHit)
        {
            damage = (int)(damage + damage / 100 * weapon.criticalDamage); // ����������� ����, ���� ��� ����������� ����
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
    }


    protected override void Die()
    {
        base.Die();
        Debug.Log("���� ��������!");
    }
}
