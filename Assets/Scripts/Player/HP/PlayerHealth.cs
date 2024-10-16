using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // ������������ ��������
    public int currentHealth; // ������� ��������
    private int lifestealPercent = 0;
    private const int regenRate = 5; // ���������� ��������, ������������������ ������ �������
    private bool isRegenerating = false; // ���� ��� ������������ �����������
    public HealthBar healthBar; // ������ �� ��������� ������� ��������
    public Animator animator; // ������ �� ��������� Animator
    public int defense = 0; // ������� ������ ������ (0-200)
    private const int maxDefense = 200; // ������������ ������� ������
    private const float maxDamageReduction = 0.8f; // ������������ ���������� ����� (80%)

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth); // ���������� ������������ �������� �� �������
        }
        else
        {
            Debug.LogError("HealthBar reference is missing on PlayerHealth!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator reference is missing on PlayerHealth!");
        }
    }
    public void AddLifesteal(int amount)
    {
        lifestealPercent += amount;
        Debug.Log("��������� �������� �� " + amount + "%. ������� ������� ����������: " + lifestealPercent + "%");
    }

    // ����� ��� �������������� �������� ��� �������� �����
    public void HealOnKill(int enemyHealth)
    {
        int healAmount = Mathf.FloorToInt(enemyHealth * (lifestealPercent / 100f));
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
        Debug.Log("������������� " + healAmount + " �������� �� �������� �����. ������� ��������: " + currentHealth);
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        // ������������ ���������� ����� �� ������ ������ ������
        float damageReduction = Mathf.Min(defense / 10 * 0.04f, maxDamageReduction);
        int reducedDamage = Mathf.RoundToInt(damage * (1 - damageReduction));

        currentHealth -= reducedDamage;

        // ������������ ������� ��������, ����� ��� �� ������� ���� 0
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die()); // ��������� �������� ��� ������
        }

        Debug.Log("����� ������� ����: " + reducedDamage + ", ������ ��������� ���� ��: " + (damage - reducedDamage));

        UpdateHealthUI(); // ��������� UI ��������
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
        if (!isRegenerating && currentHealth < maxHealth)
        {
            Debug.Log("������ �����������.");
            isRegenerating = true;
            StartCoroutine(RegenerateHealth());
        }
        else
        {
            Debug.Log("����������� ��� ������� ��� �������� �� ���������.");
        }
    }

    // ��������� ����������� ��������
    private IEnumerator RegenerateHealth()
    {
        while (isRegenerating)
        {
            if (currentHealth < maxHealth)
            {
                int previousHealth = currentHealth; // ��������� ���������� ��������
                currentHealth += Mathf.FloorToInt(regenRate);
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                Debug.Log($"�����������: ������� ��������: {previousHealth} -> {currentHealth} �� {maxHealth}");
                UpdateHealthUI();
            }
            else
            {
                Debug.Log("�������� ��������� �������������. ��������� �����������.");
                StopHealthRegen();
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
}