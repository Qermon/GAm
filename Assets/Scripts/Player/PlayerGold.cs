using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // ��� ������������� TMP_Text

public class PlayerGold : MonoBehaviour
{
    public int currentGold = 0; // ������� ���������� ������
    public TMP_Text goldText; // ������ �� UI Text ������� ��� ����������� ������
    private PlayerHealth playerHealth;
    private bool bonusGivenForWave = false; // ����, ������������� ���������� ������ �� ������� �����

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>(); // �������� ������ �� PlayerHealth
        UpdateGoldDisplay(); // �������������� ����������� ������ � ������
    }

    // ��������� ������
    public void AddGold(int amount)
    {
        currentGold += amount; // ����������� ������
        UpdateGoldDisplay(); // ��������� ����������� ������
    }

    // ���������� ������ ����� �������� ��������
    public void AddWaveInvestmentBonus()
    {
        // ���������, ���� �� ������ ��� ��������� �� ��� �����
        if (!bonusGivenForWave)
        {
            if (playerHealth != null)
            {
                float bonusGold = playerHealth.CalculateInvestmentBonus(currentGold); // ������������ �������� ������
                AddGold(Mathf.FloorToInt(bonusGold)); // ��������� ��� � �������� ������
                Debug.Log($"�������� ������ �� ����������: {bonusGold}");
            }
            else
            {
                Debug.LogError("PlayerHealth �� ������!");
            }

            // ������������� ����, ����� �� ��������� ������ �����
            bonusGivenForWave = true;
        }
    }

    // ����� ��� �������� ��������
    public void OnShopClosed()
    {
        AddWaveInvestmentBonus(); // ��������� ����� �� ���������� ����� �������� ��������
    }

    // ����� ��� ������ ����� �����
    public void OnNewWaveStarted()
    {
        bonusGivenForWave = false; // ���������� ���� ��� ����� �����
    }

    // ����� ��� ������� � ��������
    public bool PurchaseItem(int price)
    {
        if (currentGold >= price)
        {
            currentGold -= price; // ��������� ������
            UpdateGoldDisplay(); // ��������� ����������� ������
            Debug.Log($"������� ������ �� {price} ������.");
            return true; // ������� �������
        }
        else
        {
            Debug.Log("������������ ������ ��� �������.");
            return false; // ������������ ������
        }
    }

    private void UpdateGoldDisplay()
    {
        // ��������� ����� ������ � UI ������ ���� ������ �� ����� ����������
        if (goldText != null)
        {
            goldText.text ="" + currentGold;
        }
        else
        {
            Debug.LogError("������ �� goldText �����������!");
        }
    }
}
