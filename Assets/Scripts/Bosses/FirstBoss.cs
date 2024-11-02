using System.Collections;
using UnityEngine;

public class FirstBoss : Enemy
{
    public float bossAttackRange = 1.0f;
    public float bossMoveSpeed = 0.5f;
    public int bossGoldAmount = 50;
    public Animator animator;
    public GameObject lightningPrefab;
    public float lightningSpawnRadius = 3.0f;
    public float invulnerabilityDuration = 10.0f;

    private bool isRegenerating = false;
    private bool isInvulnerable = false;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        attackRange = bossAttackRange;
        enemyMoveSpeed = bossMoveSpeed;
        goldAmount = bossGoldAmount;

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

        if (!isRegenerating && currentHealth <= maxHealth * 0.5f)
        {
            Debug.Log("Boss starts regenerating.");
            StartCoroutine(Regeneration());
        }

        if (isInvulnerable) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && attackTimer <= 0f)
        {
            animator.SetTrigger("meleeAttack");
            DealDamageToPlayer();
            attackTimer = attackCooldown;
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    private IEnumerator Regeneration()
    {
        isRegenerating = true;
        isInvulnerable = true;
        animator.SetTrigger("regeneration");

        yield return new WaitForSeconds(1.0f);
        currentHealth += 20;

        for (int i = 0; i < 5; i++)
        {
            SpawnLightning();
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(invulnerabilityDuration);

        isRegenerating = false;
        isInvulnerable = false;
        Debug.Log("Boss is no longer invulnerable.");
    }

    private void SpawnLightning()
    {
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * lightningSpawnRadius;
        Instantiate(lightningPrefab, randomPosition, Quaternion.identity);
    }

    protected override void MoveTowardsPlayer()
    {
        if (player != null && !isInvulnerable)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition((Vector2)transform.position + direction * enemyMoveSpeed * Time.deltaTime);
            FlipSprite(direction);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    public void DealDamageToPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage((int)damage);
            Debug.Log("Boss deals damage to player!");
        }
    }

    public override void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        base.TakeDamage(damage);
        Debug.Log("Boss takes damage!");
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Boss dies!");
    }
}
