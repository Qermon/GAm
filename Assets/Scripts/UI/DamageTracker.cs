using System.Collections.Generic;
using UnityEngine;

public class DamageTracker : MonoBehaviour
{
    private Dictionary<string, float> weaponDamage = new Dictionary<string, float>();
    private List<string> activeWeapons = new List<string>(); // ������ ��� �������� ������
    private WeaponSelectionManager weaponSelectionManager;

    private void Start()
    {
        weaponSelectionManager = FindObjectOfType<WeaponSelectionManager>();
    }

    // ����� ��� ���������� ������ ������ � ��������
    public void ActivateWeapon(string weaponName)
    {
        if (!activeWeapons.Contains(weaponName))
        {
            activeWeapons.Add(weaponName);
            weaponDamage[weaponName] = 0; // �������������� ���� ��� ������ ������
        }
    }

    // ����������� ����� ��� ��������� ������
    public void RegisterDamage(string weaponName, float damage)
    {
        if (weaponDamage.ContainsKey(weaponName))
        {
            weaponDamage[weaponName] += damage;
        }
    }

    public float GetTotalDamage()
    {
        float total = 0;
        foreach (var damage in weaponDamage.Values)
        {
            total += damage;
        }
        return total;
    }

    public Dictionary<string, float> GetWeaponDamageData()
    {
        return new Dictionary<string, float>(weaponDamage);
    }

    // ����������� ����������� ������ �����
    public void DisplayDamagePercentages()
    {
        float totalDamage = 0;

        foreach (var damage in weaponDamage.Values)
        {
            totalDamage += damage;
        }

        if (totalDamage == 0) return;

        Debug.Log("Damage Percentages:");
        foreach (var weapon in weaponDamage)
        {
            float percentage = (weapon.Value / totalDamage) * 100;
            Debug.Log($"{weapon.Key}: {percentage}%");
        }
    }

    // ����� ����� ��� ����� �����
    public void ResetDamage()
    {
        foreach (var weapon in activeWeapons)
        {
            weaponDamage[weapon] = 0;
        }
    }
}
