using System.Collections;
using UnityEngine;

public class Boss : Enemy
{
    public GameObject mobPrefab;
    public GameObject samuraiPrefab;

    private float phase1Threshold;
    private float phase2Threshold;
    private bool isPhase1 = true;
    private bool isPhase2 = false;
    private bool isPhase3 = false;
    private float summonCooldown;
    private float samuraiSummonCooldown;
    public new float enemyMoveSpeed = 0.8f;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        phase1Threshold = maxHealth * 0.6f;
        phase2Threshold = maxHealth * 0.3f;
        summonCooldown = 10f;
        samuraiSummonCooldown = 12f;

        StartCoroutine(PhaseBehavior());
    }

    private IEnumerator PhaseBehavior()
    {
        while (!isDead)
        {
            if (currentHealth <= phase2Threshold && !isPhase3)
            {
                EnterPhase3();
            }
            else if (currentHealth <= phase1Threshold && !isPhase2)
            {
                EnterPhase2();
            }

            if (Vector2.Distance(transform.position, player.position) <= attackRange && attackTimer <= 0f)
            {
                AttackPlayer();
            }

            MoveTowardsPlayer();

            yield return null;
        }
    }

    private void EnterPhase1()
    {
        isPhase1 = true;
        isPhase2 = false;
        isPhase3 = false;

        // ��������� ������� ������� ���������
        animator.SetBool("isPhase1", true);
        animator.SetBool("isPhase2", false);
        animator.SetBool("isPhase3", false);

        StartCoroutine(SummonMobs(10f, 5));
    }

    private void EnterPhase2()
    {
        isPhase1 = false;
        isPhase2 = true;
        isPhase3 = false;

        // �������� �������� �������� � ���� 2
        animator.SetTrigger("transitionToPhase2");

        // ��������� ������� ������� ���������
        animator.SetBool("isPhase1", false);
        animator.SetBool("isPhase2", true);
        animator.SetBool("isPhase3", false);

        summonCooldown = 13f;
        StartCoroutine(SummonMobs(summonCooldown, 7));
        StartCoroutine(SummonSamurai(samuraiSummonCooldown, 3));
    }

    private void EnterPhase3()
    {
        if (isPhase3) return; // ���������, ���� ��� � ���� 3, �� ��������� �������� �������� �����

        isPhase1 = false;
        isPhase2 = false;
        isPhase3 = true;

        // �������� �������� �������� � ���� 3 ������ ���� ���
        animator.SetTrigger("transitionToPhase3");

        // ��������� ������� ������� ���������
        animator.SetBool("isPhase1", false);
        animator.SetBool("isPhase2", false);
        animator.SetBool("isPhase3", true);

        // ����������� �������� ��������
        enemyMoveSpeed *= 1.3f;

        summonCooldown = 18f;
        samuraiSummonCooldown = 10f;

        // ��������� ����� ����� � ��������
        StartCoroutine(SummonMobs(summonCooldown, 7));
        StartCoroutine(SummonSamurai(samuraiSummonCooldown, 4));
    }





    private IEnumerator SummonMobs(float cooldown, int count)
    {
        while (!isDead && ((isPhase1 && cooldown == 10f) || (isPhase2 && cooldown == 8f) || (isPhase3 && cooldown == 6f)))
        {
            yield return new WaitForSeconds(cooldown);
            PlaySummonMobsAnimation();
        }
    }

    private IEnumerator SummonSamurai(float cooldown, int count)
    {
        while (!isDead && ((isPhase2 && cooldown == 12f) || (isPhase3 && cooldown == 10f)))
        {
            yield return new WaitForSeconds(cooldown);
            PlaySummonSamuraiAnimation();
        }
    }

    protected override void MoveTowardsPlayer()
    {
        // ���� �� � ���� 3 � ������ ������
        if (isPhase3 && Vector2.Distance(transform.position, player.position) > attackRange)
        {
            animator.SetBool("isRunning", true);  // �������� ���
        }
        else
        {
            animator.SetBool("isRunning", false); // �������� ������ ��� �����������
        }

        base.MoveTowardsPlayer(); // ����� ������������� ������ ��� �����������
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
            animator.SetTrigger("isAttacking"); // ����� �������� ����� (skill_1)
        }
    }

    private void PlaySummonSamuraiAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("isSummoningSamurai"); // ����� �������� ������� �������� (skill_2)
        }
    }

    private void PlaySummonMobsAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("isSummoningMobs"); // ����� �������� ������� ����� (idle_2)
        }
    }

    private void PlayAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }

    private float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 1f; // ������������ �� ���������
    }

    public void ApplyAttackDamage()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);  // ������� ���� ������
            }
        }
    }
    public IEnumerator SpawnMobs()
    {
        int count = isPhase3 ? 7 : isPhase2 ? 7 : 5;

        for (int i = 0; i < count; i++)
        {
            // ������� ���� � ��������� ������� � �������� �������
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 4;
            Instantiate(mobPrefab, spawnPosition, Quaternion.identity);

            // ��������� �������� ����� �������� �����
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
        Destroy(gameObject);  // ������� ������ ����� ����� ��������� �������� ������
    }

    protected override void Die()
    {
        isDead = true;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        animator.SetBool("isDead", true);
    }
}
