using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // ��� ������������� TMP_Text

public class PlayerGold : MonoBehaviour
{
    public int currentGold = 0; // ������� ���������� ������
    public TMP_Text goldText; // ������ �� UI Text ������� ��� ����������� ������

    void Start()
    {
        UpdateGoldDisplay(); // �������������� ����������� ������ � ������
    }

    public void AddGold(int amount)
    {
        currentGold += amount; // ����������� ������
        Debug.Log("�������� ������: " + amount);
        UpdateGoldDisplay(); // ��������� ����������� ������
    }

    private void UpdateGoldDisplay()
    {
        // ��������� ����� ������ � UI
        if (goldText != null)
        {
            goldText.text = "������: " + currentGold; // ������ ������
        }
    }
}
