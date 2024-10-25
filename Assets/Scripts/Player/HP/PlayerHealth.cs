using System.Collections;
using System.Threading;
using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 1000; // ������������ ��������
    public int currentHealth; // ������� ��������
    public float regen = 0.01f; // ���������� ��������, ������������������ ������ �������
    private bool isRegenerating = false; // ���� ��� ������������ �����������
    public HealthBar healthBar; // ������ �� ��������� ������� ��������
    

    public int defense = 10; // ������� ������ ������ (0-200)
    private const int maxDefense = 200; // ������������ ������� ������
    private const float maxDamageReduction = 0.8f; // ������������ ���������� ����� (80%)
    public float investment = 0; // �������� ����������
    public float lifesteal = 0; // �������� ����������
    public float pickupRadius = 1f; // ������ ����� ���������
    public int luck = 0; // ������� �����

    public int shieldAmount = 0; // ������ ���� � ������ ������ �����
    private float shieldPercent; // ������� �� ������������� �������� ��� ����
    private int shieldBuffCount = 0; // ��� �������� ���������� �������������� ������



    private CircleCollider2D collectionRadius; // ������ �� �������-��������� ��� ����� ���������
    void Start()
    {
        // ������������� �������� ��������
        currentHealth = maxHealth;
        shieldAmount = 0;
        shieldPercent = 0.25f; // 25% �� ������������� ��������
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
        collectionRadius = gameObject.AddComponent<CircleCollider2D>();
        collectionRadius.isTrigger = true;
        collectionRadius.radius = pickupRadius;

        if (healthBar == null)
        {
            Debug.LogError("HealthBar reference is missing on PlayerHealth!");
        }
    }

    // ����� ��� ���������� ����������� ��������
    public void IncreaseHealthRegen(float percentage)
    {
        float increaseAmount = regen * percentage;
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
        int increaseAmount = Mathf.FloorToInt(maxHealth * percentage); // ������������ ���������� �� ������ �������� �� �������� maxHealth
        maxHealth += increaseAmount; // ����������� ������������ ��������
        currentHealth += increaseAmount; // ����������� ������� �������� �� �� �� ����������, ����� ����� �� ����� ��������
        healthBar.SetMaxHealth(maxHealth); // ��������� ������������ �������� �� UI
        UpdateHealthUI(); // ��������� ������� ������� �������� �� UI
        Debug.Log($"������������ �������� ��������� �� {increaseAmount}. ����� ������������ ��������: {maxHealth}. ������� ��������: {currentHealth}");
    }



    // ����� ��� ���������� ����������
    public void IncreaseLifesteal(float percentage)
    {
        float increaseAmount = lifesteal * percentage;
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
        float increaseAmount = pickupRadius * percentage;
        pickupRadius += increaseAmount; // ����������� ������ �����

        // ������� ������ ���������, ���� �� ����������
        if (collectionRadius != null)
        {
            Destroy(collectionRadius);
        }

        // ������� ����� ���������
        collectionRadius = gameObject.AddComponent<CircleCollider2D>();
        collectionRadius.isTrigger = true; // ����� ��� ��� �������
        collectionRadius.radius = pickupRadius; // ������������� ����� ������

        Debug.Log($"������ ����� �������� �� {increaseAmount}. ����� ������ �����: {pickupRadius}");
    }



    public void AddLifesteal(int amount)
    {
        lifesteal += amount;
        Debug.Log("��������� �������� �� " + amount + "%. ������� ������� ����������: " + lifesteal + "%");
    }

    // ����� ��� �������������� �������� ��� �������� �����
    public void HealOnKill(int enemyHealth)
    {
        int healAmount = Mathf.FloorToInt(enemyHealth * (lifesteal / 100f));
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
        Debug.Log("������������� " + healAmount + " �������� �� �������� �����. ������� ��������: " + currentHealth);
        UpdateHealthUI();
    }

    public float CalculateInvestmentBonus(float currentGold)
    {
        // ������������� ���������� ������ �� ������ 10 ����������
        float fixedGoldBonus = Mathf.Floor(investment / 10) * 10; // ��������, 10 ������ �� ������ 10 ����������

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

        // ������� ���������� � ������� ����
        Debug.Log($"��� �����������: ������� ��� = {shieldAmount} (�� ��� {Mathf.FloorToInt(shieldFromHealth)} �� ������������� ��������)");
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
        }

        // ������ ������������ ����� �� ������ ������
        float damageReduction = Mathf.Min(defense / 10 * 0.04f, 0.8f);
        int reducedDamage = Mathf.RoundToInt(damageToTake * (1 - damageReduction));
        currentHealth -= reducedDamage;

        // ��������, �� ���� �� ����� �� 0 ��������
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die());
        }

        healthBar.SetHealth(currentHealth);
        Debug.Log($"������� ����: {damageToTake}, ����������� ����: {reducedDamage}, ���������� ���: {shieldAmount}");
    }

    private void CheckHealth()
    {
        // ���������, ���� ������� �������� ������ ��� ����� 29% �� �������������
        if (currentHealth <= maxHealth * 0.29f)
        {
            shieldBuffCount = 0; // �������� ���������� ������
            Debug.Log("Shield buff count �������, ������� ��������: " + maxHealth/currentHealth);
        }
    }


    public void AddShield(int additionalShield)
    {
        // ������������ ��� � ��������� �� ������������� ��������
        float shieldFromHealth = maxHealth * shieldPercent;

        // ���������� shieldFromHealth � int ����� ����������� � shieldAmount
        shieldAmount += additionalShield + Mathf.FloorToInt(shieldFromHealth);

        // ������� ���������� � ������� ����
        Debug.Log($"������� ���: {shieldAmount}");
    }

    public void AddShieldBuff()
    {
        shieldBuffCount++; // ����������� ���������� �������������� ������
        Debug.Log($"�������� ���� ����. ���������� ������: {shieldBuffCount}");
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

    // ����������� �����������
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


    // ��������� ����������� ��������
    private IEnumerator RegenerateHealth()
    {
        while (isRegenerating)
        {
            if (currentHealth < maxHealth)
            {
                int previousHealth = currentHealth; // ��������� ������� ��������

                // ������������ ��������������� �������� ��� ������� �� maxHealth
                int healAmount = Mathf.FloorToInt(maxHealth * regen);
                currentHealth += healAmount;

                // ��������, ��� ������� �������� �� ��������� ���������
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                Debug.Log($"�����������: ������� ��������: {previousHealth} -> {currentHealth} �� {maxHealth} (������������� {healAmount})");
                UpdateHealthUI(); // ��������� UI ��������
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

        Debug.Log($"��� �������: ������� ��� = {shieldAmount} (�� ��� {Mathf.FloorToInt(shieldFromHealth)} �� ������������� �������� �� ������ {shieldBuffCount} ������)");
    }


}