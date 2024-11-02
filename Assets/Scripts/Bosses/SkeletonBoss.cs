using System.Collections;
using UnityEngine;

public class Skeletonboss : MonoBehaviour
{
    // �������������� �����
    public int goldAmount = 50;
    public int maxHealth = 500;
    public float currentHealth; // ������� public ��� �������
    public float bossMoveSpeed = 1f;
    public int meleeDamage = 20;
    public float meleeAttackRange = 5f;
    public float meleeAttackCooldown = 2f;

    private bool isDead = false;
    private Transform player;
    private float meleeAttackTimer = 0f;

    // ����� ��� �������� ������� �����
    private bool hasSummonedAt70 = false;
    private bool hasSummonedAt50 = false;
    private bool hasSummonedAt30 = false;

    // ��������� ���� ��� ����� � �����
    public GameObject experienceItemPrefab;
    public int experienceAmount = 100;

    void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player �� ������!");
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        meleeAttackTimer -= Time.deltaTime;

        // �������� �� ������ �������� � ������ �����
        CheckHealthForSummoning();

        if (Vector2.Distance(transform.position, player.position) <= meleeAttackRange && meleeAttackTimer <= 0f)
        {
            MeleeAttack();
        }

        MoveTowardsPlayer();
    }

    // �������� �������� ��� ������� �����
    void CheckHealthForSummoning()
    {
        if (currentHealth <= maxHealth * 0.7f && !hasSummonedAt70)
        {
            SummonMobs();
            hasSummonedAt70 = true;
        }

        if (currentHealth <= maxHealth * 0.5f && !hasSummonedAt50)
        {
            SummonMobs();
            hasSummonedAt50 = true;
        }

        if (currentHealth <= maxHealth * 0.3f && !hasSummonedAt30)
        {
            SummonMobs();
            hasSummonedAt30 = true;
        }
    }

    void MeleeAttack()
    {
        meleeAttackTimer = meleeAttackCooldown;

        // ��������� �������� �����
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("skill_1"); // ������������� ������� ��� �������� �����
        }

        // ������� ���� ������
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(meleeDamage);
        }
    }

    // ������ �����
    void SummonMobs()
    {
        // ��������� �������� �������
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("skill_2"); // ������������� ������� ��� �������� �������
        }

        // ������ ������� �����
        Debug.Log("���� ������� �����!");
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, bossMoveSpeed * Time.deltaTime);

            // ������������� ������� �������� ����
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("run"); // ������������� ������� ��� �������� ����
            }

            FlipSprite(direction);
        }
    }

    void FlipSprite(Vector2 direction)
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("���� ��������!");

        // ���������� ������ � �����
        PlayerGold playerGold = player.GetComponent<PlayerGold>();
        if (playerGold != null)
        {
            playerGold.AddGold(goldAmount);
        }

        SpawnExperience();
        Destroy(gameObject);
    }

    void SpawnExperience()
    {
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
        }
    }
}
