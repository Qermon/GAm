using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // �������� �������������� �����
    public int goldAmount = 10;
    public int maxHealth = 100;
    public float currentHealth; // ������� public
    public float enemyMoveSpeed = 0f;
    public int damage = 0;
    public float attackRange = 0.1f;
    public float attackCooldown = 1f;
    protected bool isDead = false;

    protected Transform player; // ������ �� ������
    private float attackTimer = 0f; // ���������� ������ ��� �������� ����

    private float originalMass; // �������� �����
    private Rigidbody2D rb; // Rigidbody ��� ��������� �����

    // ��������� ���� ��� ����� � �����
    public GameObject experienceItemPrefab;
    public int experienceAmount = 20;
    public GameObject[] bloodPrefabs; // ������ ������� �����

    public bool IsDead
    {
        get { return isDead; }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalMass = rb.mass; // ��������� �������� �����

        currentHealth = maxHealth;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("������ Player �� ������ � �����!");
        }
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        attackTimer -= Time.deltaTime;
        if (Vector2.Distance(transform.position, player.position) <= attackRange && attackTimer <= 0f)
        {
            AttackPlayer();
        }

        if (enemyMoveSpeed <= 0)
        {
            rb.mass = originalMass + 100; // ����������� �����
        }
        else
        {
            rb.mass = originalMass; // ���������� �������� �����
        }

        MoveTowardsPlayer();
    }

    public float GetCurrentHP()
    {
        return currentHealth;
    }

    // ����� ��� ��������� ��������
    public void ModifySpeed(float speedMultiplier, float duration)
    {
        StartCoroutine(ApplySpeedChange(speedMultiplier, duration));
    }

    private IEnumerator ApplySpeedChange(float speedMultiplier, float duration)
    {
        float originalSpeed = enemyMoveSpeed;
        enemyMoveSpeed *= speedMultiplier; // ��������� ��������� ��������

        yield return new WaitForSeconds(duration); // �������� ����� ��������

        enemyMoveSpeed = originalSpeed; // ��������������� �������� ��������
    }


    // ����� ��� �������� � ������
    protected virtual void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyMoveSpeed * Time.deltaTime);
            FlipSprite(direction); // ����� ��� �������� �������
        }
    }

    protected virtual void AttackPlayer()
    {
        attackTimer = attackCooldown;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HealOnKill(maxHealth); // ��������������� �������� ������
        }

        // ��������� ������ ������
        PlayerGold playerGold = player.GetComponent<PlayerGold>();
        if (playerGold != null)
        {
            playerGold.AddGold(goldAmount);
        }

        SpawnExperience();
        SpawnBlood();
        Destroy(gameObject);
    }

    protected virtual void SpawnExperience()
    {
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
        }
    }

    protected virtual void SpawnBlood()
    {
        if (bloodPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodPrefabs.Length);
            GameObject blood = Instantiate(bloodPrefabs[randomIndex], transform.position, Quaternion.identity);
            blood.tag = "Blood";
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += (int)amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log(gameObject.name + " ��� ������� �� " + amount + " ������.");
    }

    protected virtual void FlipSprite(Vector2 direction)
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

    public void SetDamage(float newDamage)
    {
        damage = (int)newDamage;
    }
}
