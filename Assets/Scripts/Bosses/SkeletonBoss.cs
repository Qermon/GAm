using UnityEngine;

public class SkeletonBoss : Enemy
{
    public float attackRange = 1.0f; // ��������� �����
    public float moveSpeed = 1.0f; // �������� ��������
    public int goldAmount = 50; // ���������� ������, ���������� � �����
    private Transform player; // ������ �� ������
    private float attackCooldown = 1.0f; // ����� ����� �������
    private float skeletonAttackTimer; // ������ ����� (������������)
    private Animator animator; // ������ �� Animator

    // ����� ���������� ��� ������ �����
    public GameObject lightningPrefab; // ������ ������
    public float lightningRadius = 1.0f; // ������ ������ ������
    public float lightningDuration = 3.0f; // ������������ ����� ������
    private bool isLightningAttacking = false; // ���������, ��������� �� ���� � ����� ������
    private float[] healthThresholds = { 0.75f, 0.50f, 0.25f, 0.10f }; // ����� �������� ��� ������

    // ����� ���������� ��� ������ �����
    public GameObject mobPrefab; // ������ ����
    public int mobsToSpawn = 10; // ���������� ����� ��� �������
    private bool isInvulnerable = false; // ���������, ����� �� ���� �������� ����
    private bool hasSpawnedMobs = false; // ���� ��� ������������ ��������� �����
    private bool isHitAnimating = false; // ���������, ��������� �� ���� � ��������

    protected override void Start()
    {
        base.Start();
        skeletonAttackTimer = 0f; // ������������� ������� �����
        animator = GetComponent<Animator>(); // �������� ��������� Animator

        // ������� ������ � �����
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }

    protected override void Update()
    {
        if (isDead || player == null) return;

        skeletonAttackTimer -= Time.deltaTime; // ��������� ������ �����

        // ��������� ���������� �� ������
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isInvulnerable)
        {
            // � ������, ���� ���� ��������, ��������� ���������� ��������
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("hit_2"))
            {
                isInvulnerable = false; // ������� ������������
            }
            return; // �� ���������� ����������, ���� ���� ��������
        }

        if (isHitAnimating) // ��������� ��������� ��������
        {
            return; // �� ��������� ��������, ���� �������� �� ���������
        }

        if (distanceToPlayer <= attackRange && skeletonAttackTimer <= 0f)
        {
            AttackPlayer(); // ����� ������
        }
        else if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer(); // �������� � ������
        }
        else
        {
            animator.SetBool("run", false); // ������������� �������� ����
        }

        // ��������� �������� ����� ��� ��������� ����� ������ � ������ �����
        CheckHealthForLightningAttack();
    }

    protected override void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            FlipSprite(direction); // ������������ ������ � ������� ��������

            animator.SetBool("run", true); // ������������� �������� ����
        }
    }

    protected override void AttackPlayer()
    {
        skeletonAttackTimer = attackCooldown; // ���������� ������ �����
        animator.SetBool("run", false); // ������������� �������� ����
        animator.SetTrigger("skill_1"); // ��������� ������ �����

        // ������ �������� ����� ���������� �����
        Invoke("DealDamage", 0.5f); // ���������, ��� ����� �������� ��������� � ������������������ �������� �����

        Debug.Log("������ ������� ���� ������!");
    }

    private void CheckHealthForLightningAttack()
    {
        float healthPercentage = (float)currentHealth / maxHealth; // ������� �������� ��������

        foreach (float threshold in healthThresholds)
        {
            if (healthPercentage <= threshold && !isLightningAttacking)
            {
                if (healthPercentage <= 0.50f && !hasSpawnedMobs)
                {
                    // ���� �������� ���� 50%, �������� ���� �������� hit_2 � ����� �����
                    StartCoroutine(StartInvulnerabilityAndSpawnMobs());
                    hasSpawnedMobs = true; // ������������� ����, ����� �� �������� ����� �����
                }
                else if (healthPercentage > 0.50f)
                {
                    StartLightningAttack(); // ����� ������
                }
                break; // ������������� �������� ����� ������ ���������
            }
        }
    }

    private System.Collections.IEnumerator StartInvulnerabilityAndSpawnMobs()
    {
        isInvulnerable = true; // ���� ���������� ����������
        isHitAnimating = true; // ������������� ���� ��������
        animator.SetTrigger("hit_2"); // ��������� �������� hit_2

        for (int i = 0; i < mobsToSpawn; i++)
        {
            // ����� ���� � ��������� ������� ������ �����
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 1.0f; // ������ �� ��������� ���������� �� �����
            Instantiate(mobPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.5f); // �������� ����� ������� �����
        }

        // ���� ��������� �������� hit_2
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isInvulnerable = false; // ���� ������ �� ��������
        isHitAnimating = false; // ���������� ���� ��������
    }

    private void StartLightningAttack()
    {
        isLightningAttacking = true;
        animator.SetTrigger("skill_2"); // ��������� �������� ������

        // ��������� �������� ��� ������ ������
        StartCoroutine(SpawnLightning());
    }

    private System.Collections.IEnumerator SpawnLightning()
    {
        float endTime = Time.time + lightningDuration;

        while (Time.time < endTime)
        {
            // ����� ������ � ��������� ������� ������ �����
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * lightningRadius;
            GameObject lightningInstance = Instantiate(lightningPrefab, spawnPosition, Quaternion.identity);

            // ���������� ��������� ������ ����� ��������
            Destroy(lightningInstance, 1.0f); // ��������� ����� �����, ��� ����� �� ������, ����� ��� ����������

            yield return new WaitForSeconds(0.5f); // �������� ����� �������� ������
        }

        isLightningAttacking = false; // ���������� ���� ����� ���������� �����
    }

    private void DealDamage()
    {
        if (isInvulnerable) return; // �� ������� ����, ���� ���� ��������

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage((int)damage); // ������� ���� ������
        }
    }

    public override void TakeDamage(int damage)
    {
        if (isInvulnerable) return; // �� ������� ����, ���� ���� ��������

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    protected override void Die()
    {
        isDead = true;
        Debug.Log("������ ��������!");
        // ������ ��� ������, ��������, ��������� ������
        PlayerGold playerGold = player.GetComponent<PlayerGold>();
        if (playerGold != null)
        {
            playerGold.AddGold(goldAmount);
        }
        Destroy(gameObject);
    }

    protected void FlipSprite(Vector2 direction)
    {
        Vector3 localScale = transform.localScale;
        if (direction.x > 0)
        {
            localScale.x = Mathf.Abs(localScale.x);
        }
        else if (direction.x < 0)
        {
            localScale.x = -Mathf.Abs(localScale.x);
        }
        transform.localScale = localScale;
    }
}
