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
    public Image hpBarImage; // Ссылка на Image для отображения HP

    private Animator animator;

    protected override void Start()
    {
       
        base.Start();

        GameObject hpBarObject = GameObject.Find("foregroundBoss"); // Здесь "HPBarName" - это имя вашего объекта Image

        if (hpBarObject != null)
        {
            // Преобразуем его в компонент Image
            hpBarImage = hpBarObject.GetComponent<Image>();

            if (hpBarImage != null)
            {
                Debug.Log("HP-бар найден!");
            }
            else
            {
                Debug.LogError("Объект найден, но компонент Image отсутствует!");
            }

        }

        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        phase2Threshold = maxHealth * 0.7f;
        phase3Threshold = maxHealth * 0.35f;
        summonCooldown = 10f;
        samuraiSummonCooldown = 12f;
        
       
        // Начинаем с фазы 1
        EnterPhase(1);

        StartCoroutine(PhaseBehavior());
        // Устанавливаем начальное значение для HP-бара
        UpdateHealthBar();
    }

    protected override void Update()
    {
        base.Update();

        if (hpBarImage == null) 
        {
            GameObject hpBarObject = GameObject.Find("foregroundBoss"); // Здесь "HPBarName" - это имя вашего объекта Image

            if (hpBarObject != null)
            {
                // Преобразуем его в компонент Image
                hpBarImage = hpBarObject.GetComponent<Image>();

                if (hpBarImage != null)
                {
                    Debug.Log("HP-бар найден!");
                }
                else
                {
                    Debug.LogError("Объект найден, но компонент Image отсутствует!");
                }

            }
        }
    }

    private IEnumerator PhaseBehavior()
    {
        while (!isDead)
        {
            // Проверка для перехода в фазу 3
            if (currentHealth <= phase3Threshold && !isPhase3)
            {
                EnterPhase(3);
            }
            // Проверка для перехода в фазу 2, только если фаза 3 еще не активировалась
            else if (currentHealth <= phase2Threshold && !isPhase2 && !isPhase3)
            {
                EnterPhase(2);
            }

            // Атака, если игрок в пределах досягаемости
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
        // Устанавливаем флаги для всех фаз
        isPhase1 = phase == 1;
        isPhase2 = phase == 2;
        isPhase3 = phase == 3;

        // Останавливаем все корутины призыва при смене фазы
        StopSummonCoroutines();

        // Устанавливаем параметры анимации, чтобы активна была только одна фаза
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
            rigidbody2D.velocity = direction * enemyMoveSpeed;  // Используем Rigidbody2D для перемещения
        }
        else
        {
            animator.SetBool("isRunning", false);
            rigidbody2D.velocity = Vector2.zero;  // Останавливаем движение, если в атаке
        }

        base.MoveTowardsPlayer();
    }
    public override void TakeDamage(int damage, bool isCriticalHit, bool isDoubleDamage = false)
    {
        base.TakeDamage(damage, isCriticalHit, isDoubleDamage);  // Вызов метода базового класса

        // Дополнительная логика для босса (например, визуальные эффекты, переход в фазу и т. д.)
        UpdateHealthBar();  // Обновляем HP-бар
    }


    private void UpdateHealthBar()
    {
        // Рассчитываем новый fillAmount в зависимости от текущего здоровья
        float healthPercentage = currentHealth / maxHealth;
        hpBarImage.fillAmount = healthPercentage; // Обновляем значение "заполненности" изображения
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
                // Генерация случайной позиции вокруг босса
                spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 4;
                attempts++;

                // Проверка на коллайдер с тегом Wall
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

                // Если не найдено пересечение с Wall, выходим из цикла
                if (!collidesWithWall)
                    break;

            } while (attempts < maxAttempts);

            // Спавн моба только если найдена подходящая позиция
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

    public void ApplyAttackDamage()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);  // Наносим урон игроку
            }
        }
    }

}
