using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public float regenRate = 5f; // ���������� ��������, ������������������ ������ �������
    private bool isRegenerating = false;
    public HealthBar healthBar; // ������ �� ��������� ������� �������� 
    public Animator animator; // ������ �� ��������� Animator

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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // ������������ ������� ��������, ����� ��� �� ������� ���� 0
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die()); // ��������� �������� ��� ������
        }

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
        if (!isRegenerating)
        {
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
                currentHealth += Mathf.FloorToInt(regenRate);
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }
                UpdateHealthUI();
            }
            yield return new WaitForSeconds(1f);
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
}
