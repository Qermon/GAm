using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // �������� �������������� �����
    public int goldAmount = 1;
    public float maxHealth;
    public float currentHealth;
    public float enemyMoveSpeed;
    
    public float damage;
    public float attackRange = 0.1f;
    public float attackCooldown = 1f;
    protected bool isDead = false;


    protected Transform player;
    protected float attackTimer = 0f;

    private float originalMass;
    private Rigidbody2D rb;

    // ��������� ���� ��� ����� � �����
    public GameObject experienceItemPrefab;
    public int experienceAmount = 20;

    public float baseMaxHealth;
    public float baseEnemyMoveSpeed;
    public float baseDamage;

    public GameObject damageTextPrefab;   // ������ DamageText
    private Camera mainCamera;             // ������ �� ������
    public Canvas canvas; // ���� ������ ���� ���� Canvas, �� GameObject � �� Transform

    public static List<Enemy> allEnemies = new List<Enemy>();
    public GameObject bloodEffectPrefab; // �������� ��� ����
    private WaveManager waveManager; 
    private float currentSlowEffect = 0f; // ������� ����������
  

    public bool IsDead
    {
        get { return isDead; }
    }

    protected virtual void Start()
    {
        
        mainCamera = Camera.main;
        enemyMoveSpeed = baseEnemyMoveSpeed; // ������������� ������� ��������
        rb = GetComponent<Rigidbody2D>();
        originalMass = rb.mass;

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

    public void UpdateStats(float damageMultiplier, float healthMultiplier, float speedMultiplier)
    {
       
            damage += baseDamage / 100 * damageMultiplier;
            maxHealth += baseMaxHealth / 100 * healthMultiplier;
            enemyMoveSpeed += baseEnemyMoveSpeed / 100 * speedMultiplier;
    }

    public void RefreshStats()
    {
        damage = baseDamage;
        maxHealth = baseMaxHealth;
        enemyMoveSpeed = baseEnemyMoveSpeed;
    }



    void OnEnable()
    {
        allEnemies.Add(this); // ��������� ��� � ����������� ������ ��� ��� ���������
    }

    void OnDisable()
    {
        allEnemies.Remove(this); // ������� ��� �� ������ ��� ��� �����������
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
            rb.mass = originalMass + 100;
        }
        else
        {
            rb.mass = originalMass;
        }

        MoveTowardsPlayer();
    }

    public float GetCurrentHP()
    {
        return currentHealth;
    }

    public float GetCurrentSlowEffect()
    {
        return currentSlowEffect; // ���������� ������� ����������
    }


    public void ModifySpeed(float speedMultiplier, float duration)
    {
        float newSlowEffect = 1f - speedMultiplier; // ����������� ��������� � ����������

        // ���������, ����� ����� ���������� �� ��������� 10%
        if (currentSlowEffect + newSlowEffect > 0.1f)
        {
            newSlowEffect = 0.1f - currentSlowEffect; // ������������ ���������� �� 10%
        }

        if (newSlowEffect > 0)
        {
            currentSlowEffect += newSlowEffect; // ��������� ������� ����������
            StartCoroutine(ApplySpeedChange(speedMultiplier, duration));
            StartCoroutine(ResetSlowEffect(duration)); // ��������� �������� ��� ������ ����������
        }
    }
    private IEnumerator ResetSlowEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (!isDead)
        {
            currentSlowEffect = 0; 
            enemyMoveSpeed = baseEnemyMoveSpeed + baseEnemyMoveSpeed / 100 * waveManager.waveNumber;
        }

    }

    private IEnumerator ApplySpeedChange(float speedMultiplier, float duration)
    {
        float originalSpeed = enemyMoveSpeed;
        enemyMoveSpeed *= speedMultiplier;

        yield return new WaitForSeconds(duration);

        enemyMoveSpeed = originalSpeed;
    }

    protected virtual void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyMoveSpeed * Time.deltaTime);
            FlipSprite(direction);
        }
    }

    protected virtual void AttackPlayer()
    {
        attackTimer = attackCooldown;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage((int)damage);
        }
    }

    public virtual void TakeDamage(int damage, bool isCriticalHit)
    {
        Debug.Log($"Taking damage: {damage}"); // ���������� ���������
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);


      

        ShowDamageText(damage, isCriticalHit);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
        else
        {
            if (Random.Range(0f, 1f) <= 0.2f)
            {
                GameObject bloodEffectInstance = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
                BloodEffect bloodEffect = bloodEffectInstance.GetComponent<BloodEffect>();
                if (bloodEffect != null)
                {
                    bloodEffect.SpawnBlood(transform.position);
                }
            }
        }
    }

    private void ShowDamageText(int damage, bool isCriticalHit)
    {
        if (damageTextPrefab == null)
        {
            Debug.LogError("Damage Text Prefab is null!");
            return;
        }

        if (canvas == null)
        {
            Debug.LogError("CanvasDamage �� �����!");
            return;
        }

        // ������ ����� ����� ��� �������� ������� CanvasDamage
        GameObject damageTextInstance = Instantiate(damageTextPrefab, canvas.transform);
        damageTextInstance.transform.position = transform.position;

        DamageTextController damageText = damageTextInstance.GetComponent<DamageTextController>();
        if (damageText != null)
        {
            damageText.SetDamage(damage, isCriticalHit);
        }

        // ������������ ����� � ������� ������
        damageTextInstance.transform.LookAt(damageTextInstance.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                             Camera.main.transform.rotation * Vector3.up);
    }


    private void Awake()
    {
        // ���� Canvas �� �����, ������� CanvasDamage �� �����
        if (canvas == null)
        {
            canvas = GameObject.Find("CanvasDamage")?.GetComponent<Canvas>();
        }

        // ���������, ������ �� ������
        if (canvas == null)
        {
            Debug.LogError("CanvasDamage �� ������ �� �����!");
        }
    }

    protected virtual void Die()
    {
        isDead = true;

        // ����� ����� ��� ������ ����
        if (bloodEffectPrefab != null) // ���������, ��� ���������� bloodEffectPrefab ��������� � �����
        {
            GameObject bloodEffectInstance = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            BloodEffect bloodEffect = bloodEffectInstance.GetComponent<BloodEffect>();
            if (bloodEffect != null)
            {
                bloodEffect.SpawnBlood(transform.position);
            }
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HealOnKill((int)maxHealth); // ��������������� �������� ������
        }

      

        SpawnExperience();

        Destroy(gameObject);
    }


    protected virtual void SpawnExperience()
    {
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
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