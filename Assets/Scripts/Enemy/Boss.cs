using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    public GameObject mobPrefab;
    public GameObject samuraiPrefab;

    private float phase2Threshold;
    private float phase3Threshold;
    private bool isPhase1 = true;
    private bool isPhase2 = false;
    private bool isPhase3 = false;
    private float summonCooldown;
    private float samuraiSummonCooldown;
    private bool isCasting = false;
    private Coroutine summonMobsCoroutine;
    private Coroutine summonSamuraiCoroutine;
    public new Rigidbody2D rigidbody2D;
    public Image hpBarImage; // ������ �� Image ��� ����������� HP
    public float extendedAttackRange = 1.1f;  // ����������� ��������� �����

    private Animator animator;

    public AudioSource hit_1;  
    public AudioSource death;  
    public AudioSource idle_2;  
    public AudioSource run; 
    public AudioSource skill_1;  
    public AudioSource skill_2;
    public AudioSource skill_2Roar;  
    public AudioSource walk;  

    protected override void Start()
    {

        base.Start();

        GameObject hit_1SoundObject = GameObject.Find("hit_1");
        GameObject deathSoundObject = GameObject.Find("death");
        GameObject idle_2SoundObject = GameObject.Find("idle_2");
        GameObject runSoundObject = GameObject.Find("run");
        GameObject skill_1SoundObject = GameObject.Find("skill_1");
        GameObject skill_2SoundObject = GameObject.Find("skill_2");
        GameObject skill_2RoarSoundObject = GameObject.Find("skill_2Roar");
        GameObject walkSoundObject = GameObject.Find("walk");

        if (hit_1SoundObject != null || deathSoundObject != null || idle_2SoundObject != null || runSoundObject != null || skill_1SoundObject != null || skill_2SoundObject != null || skill_2RoarSoundObject != null || walkSoundObject != null)
        {
            if (hit_1SoundObject != null)
                hit_1 = hit_1SoundObject.GetComponent<AudioSource>();
            else
                Debug.LogWarning("������ 'hit_1' �� ������ �� �����!");

            if (deathSoundObject != null)
                death = deathSoundObject.GetComponent<AudioSource>();
            else
                Debug.LogWarning("������ 'death' �� ������ �� �����!");

            if (idle_2SoundObject != null)
                idle_2 = idle_2SoundObject.GetComponent<AudioSource>();
            else
                Debug.LogWarning("������ 'idle_2' �� ������ �� �����!");

            if (runSoundObject != null)
                run = runSoundObject.GetComponent<AudioSource>();
            else
                Debug.LogWarning("������ 'run' �� ������ �� �����!");

            if (skill_1SoundObject != null)
                skill_1 = skill_1SoundObject.GetComponent<AudioSource>();
            else
                Debug.LogWarning("������ 'skill_1' �� ������ �� �����!");

            if (skill_2SoundObject != null)
                skill_2 = skill_2SoundObject.GetComponent<AudioSource>();
            else
                Debug.LogWarning("������ 'skill_2' �� ������ �� �����!");

            if (skill_2RoarSoundObject != null)
                skill_2Roar = skill_2RoarSoundObject.GetComponent<AudioSource>();
            else
                Debug.LogWarning("������ 'skill_2Roar' �� ������ �� �����!");

            if (walkSoundObject != null)
                walk = walkSoundObject.GetComponent<AudioSource>();
            else
                Debug.LogWarning("������ 'walk' �� ������ �� �����!");
        }

            GameObject hpBarObject = GameObject.Find("foregroundBoss"); // ����� "HPBarName" - ��� ��� ������ ������� Image

        if (hpBarObject != null)
        {
            // ����������� ��� � ��������� Image
            hpBarImage = hpBarObject.GetComponent<Image>();

            if (hpBarImage != null)
            {
                Debug.Log("HP-��� ������!");
            }
            else
            {
                Debug.LogError("������ ������, �� ��������� Image �����������!");
            }

        }

        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        phase2Threshold = maxHealth * 0.7f;
        phase3Threshold = maxHealth * 0.35f;
        summonCooldown = 10f;
        samuraiSummonCooldown = 12f;
        

       
        // �������� � ���� 1
        EnterPhase(1);

        StartCoroutine(PhaseBehavior());
        // ������������� ��������� �������� ��� HP-����
        UpdateHealthBar();
    }

    protected override void Update()
    {
        base.Update();


        if (hpBarImage == null) 
        {
            GameObject hpBarObject = GameObject.Find("foregroundBoss"); // ����� "HPBarName" - ��� ��� ������ ������� Image

            if (hpBarObject != null)
            {
                // ����������� ��� � ��������� Image
                hpBarImage = hpBarObject.GetComponent<Image>();

                if (hpBarImage != null)
                {
                    Debug.Log("HP-��� ������!");
                }
                else
                {
                    Debug.LogError("������ ������, �� ��������� Image �����������!");
                }

            }
        }
    }

    private IEnumerator PhaseBehavior()
    {
        while (!isDead)
        {
            // �������� ��� �������� � ���� 3
            if (currentHealth <= phase3Threshold && !isPhase3)
            {
                EnterPhase(3);
            }
            // �������� ��� �������� � ���� 2, ������ ���� ���� 3 ��� �� ��������������
            else if (currentHealth <= phase2Threshold && !isPhase2 && !isPhase3)
            {
                EnterPhase(2);
            }

            // �����, ���� ����� � �������� ������������
            if (Vector2.Distance(transform.position, player.position) <= attackRange && attackTimer <= 0f)
            {
                AttackPlayer();
            }

            MoveTowardsPlayer();

            yield return null;
        }
    }


    private void EnterPhase(int phase)
    {
        // ������������� ����� ��� ���� ���
        isPhase1 = phase == 1;
        isPhase2 = phase == 2;
        isPhase3 = phase == 3;

        // ������������� ��� �������� ������� ��� ����� ����
        StopSummonCoroutines();

        // ������������� ��������� ��������, ����� ������� ���� ������ ���� ����
        animator.SetBool("isPhase1", isPhase1);
        animator.SetBool("isPhase2", isPhase2);
        animator.SetBool("isPhase3", isPhase3);

        switch (phase)
        {
            case 1:
                StartPhase1();
                break;
            case 2:
                StartPhase2();
                break;
            case 3:
                StartPhase3();
                break;
        }
    }

    private void StartPhase1()
    {
        summonCooldown = 10f;
        summonMobsCoroutine = StartCoroutine(SummonMobs(summonCooldown, 5));
    }

    private void StartPhase2()
    {
        summonCooldown = 8f;
        summonMobsCoroutine = StartCoroutine(SummonMobs(summonCooldown, 7));
        summonSamuraiCoroutine = StartCoroutine(SummonSamurai(samuraiSummonCooldown, 3));
    }

    private void StartPhase3()
    {
        summonCooldown = 6f;
        enemyMoveSpeed *= 1.3f;
        summonMobsCoroutine = StartCoroutine(SummonMobs(summonCooldown, 7));
        summonSamuraiCoroutine = StartCoroutine(SummonSamurai(samuraiSummonCooldown, 4));
    }

    private void StopSummonCoroutines()
    {
        if (summonMobsCoroutine != null)
        {
            StopCoroutine(summonMobsCoroutine);
            summonMobsCoroutine = null;
        }
        if (summonSamuraiCoroutine != null)
        {
            StopCoroutine(summonSamuraiCoroutine);
            summonSamuraiCoroutine = null;
        }
    }

    private IEnumerator SummonMobs(float cooldown, int count)
    {
        if (isDead) yield break;

        while (!isDead)
        {
            yield return new WaitForSeconds(cooldown);
            PlaySummonMobsAnimation();
        }
    }

    private IEnumerator SummonSamurai(float cooldown, int count)
    {
        if (isDead) yield break;

        while (!isDead)
        {
            yield return new WaitForSeconds(cooldown);
            PlaySummonSamuraiAnimation();
        }
    }

    protected override void MoveTowardsPlayer()
    {
        if (isCasting)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        if (isPhase3 && Vector2.Distance(transform.position, player.position) > attackRange)
        {
            animator.SetBool("isRunning", true);
            rigidbody2D.velocity = direction * enemyMoveSpeed;  // ���������� Rigidbody2D ��� �����������
        }
        else
        {
            animator.SetBool("isRunning", false);
            rigidbody2D.velocity = Vector2.zero;  // ������������� ��������, ���� � �����
        }

        base.MoveTowardsPlayer();
    }
    public override void TakeDamage(int damage, bool isCriticalHit, bool isDoubleDamage = false)
    {
        base.TakeDamage(damage, isCriticalHit, isDoubleDamage);  // ����� ������ �������� ������

        // �������������� ������ ��� ����� (��������, ���������� �������, ������� � ���� � �. �.)
        UpdateHealthBar();  // ��������� HP-���
    }


    private void UpdateHealthBar()
    {
        // ������������ ����� fillAmount � ����������� �� �������� ��������
        float healthPercentage = currentHealth / maxHealth;
        hpBarImage.fillAmount = healthPercentage; // ��������� �������� "�������������" �����������
    }


    protected override void AttackPlayer()
    {
        attackTimer = attackCooldown;
        PlayAttackAnimation();
    }

    private void PlayAttackAnimation()
    {
        if (animator != null)
        {
            isCasting = true;
            animator.SetTrigger("isAttacking");
        }
    }

    private void PlaySummonSamuraiAnimation()
    {
        if (animator != null)
        {
            isCasting = true;
            animator.SetTrigger("isSummoningSamurai");
        }
    }

    private void PlaySummonMobsAnimation()
    {
        if (animator != null)
        {
            isCasting = true;
            animator.SetTrigger("isSummoningMobs");
        }
    }

    public void EndCasting()
    {
        isCasting = false;
    }

    public IEnumerator SpawnMobs()
    {
        int count = isPhase3 ? 7 : isPhase2 ? 7 : 5;

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPosition;
            int maxAttempts = 10;
            int attempts = 0;

            do
            {
                // ��������� ��������� ������� ������ �����
                spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 4;
                attempts++;

                // �������� �� ��������� � ����� Wall
                Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 0.5f);
                bool collidesWithWall = false;
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Wall"))
                    {
                        collidesWithWall = true;
                        break;
                    }
                }

                // ���� �� ������� ����������� � Wall, ������� �� �����
                if (!collidesWithWall)
                    break;

            } while (attempts < maxAttempts);

            // ����� ���� ������ ���� ������� ���������� �������
            Instantiate(mobPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
        }
    }


    public void SpawnSamurai()
    {
        int count = isPhase3 ? 4 : 3;
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 4;
            Instantiate(samuraiPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void OnDeathComplete()
    {
        Destroy(gameObject);
    }

    protected override void Die()
    {
        isDead = true;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        animator.SetBool("isDead", true);
    }

    // ������ ��� ����, ����� ���� ������� ���� �� ����������� ���������
    public void ApplyAttackDamage()
    {
        // ���������, �� ���������� �� ���������, ����� ������� ����
        if (Vector2.Distance(transform.position, player.position) <= extendedAttackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);  // ������� ���� ������
            }
        }
    }


    public void Hit_1()
    {
        if (hit_1 != null)
        {
            hit_1.Play();
        }
    }
    public void Death()
    {
        if (death != null)
        {
            death.Play();
        }
    }
    public void Idle_2()
    {
        if (idle_2 != null)
        {
            idle_2.Play();
        }
    }
    public void Run()
    {
        if (run != null)
        {
            run.Play();
        }
    }
    public void Skill_1()
    {
        if (skill_1 != null)
        {
            skill_1.Play();
        }
    }
    public void Skill_2()
    {
        if (skill_2 != null)
        {
            skill_2.Play();
        }
    }
    public void Skill_2Roar()
    {
        if (skill_2Roar != null)
        {
            skill_2Roar.Play();
        }
    }
    public void Walk()
    {
        if (walk != null)
        {
            walk.Play();
        }
    }
}
