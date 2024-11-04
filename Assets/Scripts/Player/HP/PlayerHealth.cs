using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    public float maxHealth; // ������������ ��������
    public float baseMaxHealth;
    public float currentHealth; // ������� ��������
    public float regen = 0.01f; // ���������� ��������, ������������������ ������ �������
    public float baseRegen = 0.003f;
    private bool isRegenerating = false; // ���� ��� ������������ �����������
    public HealthBar healthBar; // ������ �� ��������� ������� ��������
    [SerializeField] private Image barrierImage; // ������� �������, ����������� � ����������

    public int defense = 10; // ������� ������ ������ (0-200)
    private const int maxDefense = 200; // ������������ ������� ������
    private const float maxDamageReduction = 0.8f; // ������������ ���������� ����� (80%)
    public float investment = 0; // �������� ����������
    public float lifesteal = 0; // �������� ����������
    public float baseLifesteal = 0.001f;
    public float basePickupRadius = 5f;
    public float pickupRadius; // ������ ����� ���������
    public int luck = 0; // ������� �����

    public int shieldAmount = 0;
    private float shieldPercent; // ������� �� ������������� �������� ��� ����
    private int shieldBuffCount = 0; // ��� �������� ���������� �������������� ������
    public float maxShieldAmount = 0; // ��� ������������ ����������� �������� ����

    private bool shieldOnKillBuffActive = false; // ���� ���������� �����
    private const float shieldChance = 0.05f; // 5% ����
    private const float shieldPercentage = 0.1f; // 10% �� ����. ��������

    private bool barrierOnLowHealthBuffActive = false; // ���� ���������� �����
    public bool barrierActivatedThisWave = false;

    private bool healthRegenPerWaveActive = false;

    private CircleCollider2D collectionRadius; // ������ �� �������-��������� ��� ����� ���������
    void Start()
    {

        baseMaxHealth = maxHealth;
        // ������������� �������� ��������
        currentHealth = maxHealth;
        shieldAmount = 0;
        shieldPercent = 0.25f; // 25% �� ������������� ��������
        pickupRadius = basePickupRadius;

        collectionRadius = gameObject.AddComponent<CircleCollider2D>();
        collectionRadius.isTrigger = true;
        collectionRadius.radius = pickupRadius;

        healthBar = FindObjectOfType<HealthBar>();
        if (healthBar == null)
        {
            Debug.LogError("HealthBar �� ������ �� �����!");
        }
        else
        {
            healthBar.SetMaxHealth((int)maxHealth);
            healthBar.SetHealth((int)currentHealth);
        }

        GameObject barrierObj = GameObject.Find("BarrierImage");
        if (barrierObj != null)
        {
            barrierImage = barrierObj.GetComponent<Image>();
        }
        else
        {
            Debug.LogWarning("BarrierImage �� ������ �� �����!");
        }


        UpdateBarrierUI(); // ������������� ����������� �������
    }


    // ����� ��� ���������� ����������� ��������
    public void IncreaseHealthRegen(float percentage)
    {
        float increaseAmount = baseRegen * percentage;
        regen += increaseAmount; // ����������� �������� ����������� ��������
        Debug.Log($"�������� ����������� �������� ��������� �� {percentage}. ����� �������� �����������: {regen}");
    }

    public void IncreaseArmor(int amount)
    {
        defense += amount; // ����������� �����
        Debug.Log($"����� ��������� �� {amount}. ������� �����: {defense}");
    }
    public void IncreaseMaxHealth(float percentage)
    {
        float increaseAmount = baseMaxHealth * percentage; // ������������ ���������� �� ������ �������� �� �������� ������
        maxHealth += increaseAmount; // ����������� ������������ ��������
        currentHealth += increaseAmount; // ����������� ������� �������� �� �� �� ����������, ����� ����� �� ����� ��������
        healthBar.SetMaxHealth((int)maxHealth); // ��������� ������������ �������� �� UI
        UpdateHealthUI(); // ��������� ������� ������� �������� �� UI
        Debug.Log($"������������ �������� ��������� �� {increaseAmount}. ����� ������������ ��������: {maxHealth}. ������� ��������: {currentHealth}");
    }

    // ����� ��� ���������� ����������
    public void IncreaseLifesteal(float percentage)
    {
        float increaseAmount = baseLifesteal * percentage;
        lifesteal += increaseAmount; // ����������� ���������
        Debug.Log($"��������� �������� �� {increaseAmount}. ������� ���������: {lifesteal}");
    }

    // ����� ��� ���������� ����������
    public void IncreaseInvestment(float amount)
    {
        investment += amount; // ����������� ����������
        Debug.Log($"���������� ��������� �� {amount}. ������� ����������: {investment}");
    }

    // ����� ��� ���������� �����
    public void IncreaseLuck(int amount)
    {
        luck += amount; // ����������� ������� �����
        Debug.Log($"����� ��������� �� {amount}. ������� ������� �����: {luck}");
    }

    // ����� ��� ���������� ������� ����� ���������
    public void IncreasePickupRadius(float percentage)
    {
        float increaseAmount = basePickupRadius * percentage;
        pickupRadius += increaseAmount; // ����������� ������ �����

        // ���������, ���������� �� ���������
        if (collectionRadius == null)
        {
            // ������� ����� ���������, ���� ��� ���
            collectionRadius = gameObject.AddComponent<CircleCollider2D>();
            collectionRadius.isTrigger = true; // ����� ��� ��� �������
        }

        // ������������� ����� ������ ����������
        collectionRadius.radius = pickupRadius;

        Debug.Log($"������ ����� �������� �� {increaseAmount}. ����� ������ �����: {pickupRadius}");
    }

    public void AddLifesteal(int amount)
    {
        lifesteal += amount;
    }

    // ����� ��� �������������� �������� ��� �������� �����
    public void HealOnKill(int enemyHealth)
    {
        float healAmount = maxHealth * lifesteal;
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth); // ��������������� ��������
        UpdateHealthUI();

        TryApplyShieldOnKill(); // �������� �������� ��� � 5% ������
    }

    public float CalculateInvestmentBonus(float currentGold)
    {
        // ������������� ���������� ������ �� ������ 10 ����������
        float fixedGoldBonus = Mathf.Floor(investment / 10); // ��������, 1 ������ �� ������ 10 ����������

        // ������� ��������� ������ � ����������� �� ������ ����������
        float investmentPercentage = (Mathf.Floor(investment / 10) * 0.01f); // 1% �� ������ 10 ����������
        float percentageGoldBonus = currentGold * investmentPercentage; // ������������ ���������� �������� ������

        // �������� �������� ������
        float totalBonusGold = fixedGoldBonus + percentageGoldBonus;

        return totalBonusGold;
    }

    // ����� ��� ��������� ���� � ������ ������ �����
    public void ActivateShield()
    {
        // ������������ ��� � ��������� �� ������������� ��������
        float shieldPercent = 0.25f; // 25%
        float shieldFromHealth = maxHealth * shieldPercent;

        // ��������� ��� �� ������ ������ �������
        shieldAmount += Mathf.FloorToInt(shieldFromHealth);
        maxShieldAmount = shieldAmount; // ��������� ������� �������� ����

        // ������� ���������� � ������� ����
        Debug.Log($"��� �����������: ������� ��� = {shieldAmount} (�� ��� {Mathf.FloorToInt(shieldFromHealth)} �� ������������� ��������)");
        UpdateBarrierUI(); // ��������� ���������� ����������� �������
    }

    // ����� ��� ��������� �������
    public void ActivateBarrier()
    {

        UpdateBarrierUI();
        Debug.Log($"������� ������������: {shieldAmount}");
    }

    // ����� ��� ���������� ������� �������
    public void UpdateBarrierUI()
    {
        if (barrierImage != null)
        {
            // ���� shieldAmount ������ ����������� ��������, ��������� ������� ���������
            if (shieldAmount > maxShieldAmount)
            {
                maxShieldAmount = shieldAmount;
                barrierImage.fillAmount = 1f;
            }
            else if (shieldAmount <= 0)
            {
                barrierImage.fillAmount = 0f; // ���� ���� ���, ������� �� ������������

            }
            else
            {
                barrierImage.fillAmount = (float)shieldAmount / maxShieldAmount;
            }


        }
        else
        {
            Debug.LogWarning("������� ������� �� ���������!");
        }

        // ���������� ���������� � ������������ �������� �������
        Debug.Log($"������� �������� �������: {shieldAmount}, ������������ �������� �������: {maxShieldAmount}");
    }

    public void TakeDamage(int damage)
    {
        int damageToTake = damage;
        CheckHealth(); // ��������� ������� ��������

        // ���� ��� �������, ��������� ����
        if (shieldAmount > 0)
        {
            if (damage <= shieldAmount)
            {
                shieldAmount -= damage;
                damageToTake = 0; // ���� ��������� �������� �����
            }
            else
            {
                damageToTake -= (int)shieldAmount; // ����, ������� �������� ����� ���
                shieldAmount = 0; // ��� ������ �������
            }
            UpdateBarrierUI(); // ��������� ���������� ����������� �������
        }

        // ������ ������������ ����� �� ������ ������
        float damageReduction = Mathf.Min(defense / 10 * 0.04f, 0.8f);
        int reducedDamage = Mathf.RoundToInt(damageToTake * (1 - damageReduction));
        currentHealth -= reducedDamage;

        // �������� �� ������ ����������� ����� ��������� �����
        if (currentHealth < maxHealth)
        {
            StartHealthRegen(); // ��������� �����������, ���� �������� ������ �������������
        }

        // ��������� ������� ��� ������ �������� ���� ��� �� �����
        if (barrierOnLowHealthBuffActive && !barrierActivatedThisWave && currentHealth <= maxHealth * 0.5f)
        {
            int barrierAmount = Mathf.FloorToInt(maxHealth * 0.2f);
            AddShield(barrierAmount);
            barrierActivatedThisWave = true; // ���������, ��� ������ �����������
            Debug.Log($"������ ����������� ��� ������ ��������: +{barrierAmount}");
        }

        // ��������, �� ���� �� ����� �� 0 ��������
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die());
        }

        healthBar.SetHealth(currentHealth);
        Debug.Log($"������� ����: {damageToTake}, ����������� ����: {reducedDamage}, ���������� ���: {shieldAmount}");
    }

    public void AddShield(int additionalShield)
    {
        shieldAmount += additionalShield;


        UpdateBarrierUI();
        Debug.Log($"������� ���������: +{additionalShield}, ����� ������: {shieldAmount}");
    }

    private void CheckHealth()
    {

        // ���������, ���� ������� �������� ������ ��� ����� 29% �� �������������
        if (currentHealth <= maxHealth * 0.29f)
        {
            shieldBuffCount = 0; // �������� ���������� ������
            Debug.Log("Shield buff count �������, ������� ��������: " + maxHealth / currentHealth);
        }
    }

    public void AddShieldBuff()
    {
        shieldBuffCount++; // ����������� ���������� �������������� ������
        Debug.Log($"�������� ���� ����. ���������� ������: {shieldBuffCount}");
    }
    public void DecreaseShield(int amount)
    {
        shieldAmount -= amount;
        shieldAmount = Mathf.Max(shieldAmount, 0); // ���������, ��� shieldAmount �� ������ 0

    }
    private IEnumerator Die()
    {
        Debug.Log("Player died!");
        gameObject.SetActive(false);
        yield return null;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20); // ���� 20, ��������
        }
    }

    public void StartHealthRegen()
    {
        Debug.Log("������� ��������� �����������.");
        // ���������, �� ���� �� ��� ����������� � ������ �� ������� �������� �������������
        if (!isRegenerating && currentHealth < maxHealth)
        {
            Debug.Log("������ �����������.");
            isRegenerating = true;
            StartCoroutine(RegenerateHealth());
        }
    }

    private IEnumerator RegenerateHealth()
    {
        while (isRegenerating)
        {
            if (currentHealth < maxHealth)
            {
                float previousHealth = currentHealth; // ��������� ������� ��������

                // ������������ ��������������� �������� ��� ������� �� maxHealth
                float healAmount = maxHealth * regen; // regen ������ ���� � ��������� [0, 1]
                currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth); // ��������������� ��������
                UpdateHealthUI(); // ��������� UI

                Debug.Log($"�����������: ������� ��������: {previousHealth} -> {currentHealth} �� {maxHealth} (������������� {healAmount})");
            }
            else
            {
                Debug.Log("�������� ��������� �������������. ��������� �����������.");
                StopHealthRegen();
                yield break; // ������������� ��������
            }

            yield return new WaitForSeconds(1f); // ����������� ������ �������
        }
    }

    // ���������� �����������
    public void StopHealthRegen()
    {
        isRegenerating = false;
    }

    public void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);

        }
    }

    public void IncreaseDefense(int amount)
    {
        defense = Mathf.Clamp(defense + amount, 0, maxDefense);
        Debug.Log("������ ������ ��������� ��: " + defense);
    }

    public void UpdateShield()
    {
        // ���������� ���
        shieldAmount = 0;

        // ������������ ����� ��� �� ������ ���������� �������������� ������
        float shieldPercent = 0.25f; // 25%
        float shieldFromHealth = maxHealth * shieldPercent * shieldBuffCount; // ����� ��� �� ������ �������������� ������
        shieldAmount += Mathf.FloorToInt(shieldFromHealth);
        maxShieldAmount = shieldAmount;

        Debug.Log($"��� �������: ������� ��� = {shieldAmount} (�� ��� {Mathf.FloorToInt(shieldFromHealth)} �� ������������� �������� �� ������ {shieldBuffCount} ������)");
    }
    public void ActivateShieldOnKillBuff()
    {
        shieldOnKillBuffActive = true;
        Debug.Log("���� '��� ��� ��������' �����������!");
    }

    // ����� ��� ���������� ���� ��� ��������
    public void TryApplyShieldOnKill()
    {
        if (shieldOnKillBuffActive && Random.value <= shieldChance)
        {
            int shieldToAdd = Mathf.FloorToInt(maxHealth * shieldPercentage);
            AddShield(shieldToAdd);
            Debug.Log($"�������� ��� {shieldToAdd} (10% �� ����. ��������) ��� �������� �����.");
        }
    }
    public void ActivateBarrierOnLowHealthBuff()
    {
        barrierOnLowHealthBuffActive = true;
        barrierActivatedThisWave = false; // �������� ��������� �� ����� �����
        Debug.Log("���� '������ ��� ������ ��������' �����������!");
    }
    public void ActivateHealthRegenPerWaveBuff()
    {
        healthRegenPerWaveActive = true;
        Debug.Log("���� HealthRegenPerWave �����������: +2% �������������� �������� ������ �����, �� ������ ����� � 30% ��������.");
    }

    public void ResetBarrierOnLowHealthBuff()
    {
        if (barrierOnLowHealthBuffActive) // ������ ���� ���� ������
        {
            barrierActivatedThisWave = false; // ����������, ����� ������ ��� ��������� �� ����� �����
            Debug.Log("���� '������ ��� ������ ��������' ����������� ��� ����� �����!");
        }
    }

    public void ApplyHealthRegenAtWaveStart()
    {
        if (healthRegenPerWaveActive)
        {
            // ������������� �������� �� 30% �� ���������
            currentHealth = Mathf.FloorToInt(maxHealth * 0.3f);
            healthBar.SetHealth(currentHealth);

            // ��������� �����������, ���� ������� �������� ������ �������������
            StartHealthRegen();

            Debug.Log($"������ �����: �������� �������� �� 30% �� ���������. ������ �����������.");

            // ��������� ������� ��� ������ �������� ���� ��� �� �����
            if (barrierOnLowHealthBuffActive && !barrierActivatedThisWave)
            {
                // �������� �� ��������� �������
                if (currentHealth <= maxHealth * 0.5f || currentHealth == Mathf.FloorToInt(maxHealth * 0.3f))
                {
                    int barrierAmount = Mathf.FloorToInt(maxHealth * 0.2f);
                    AddShield(barrierAmount);
                    barrierActivatedThisWave = true; // ���������, ��� ������ �����������
                    Debug.Log($"������ ����������� ��� ������ ��������: +{barrierAmount}");
                }
            }
        }
    }
}
