using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DamageDisplayManager : MonoBehaviour
{
    [Header("Text Objects for Weapon Stats")]
    public TMP_Text totalDamageText;
    public TMP_Text[] weaponDamageTexts; // ������ ��� ������� ����� ������� ������

    private Dictionary<WeaponType, float> weaponDamage = new Dictionary<WeaponType, float>(); // ���� ������� ������ �� �����
    private float totalDamage;

    private void Start()
    {
        // ���������� �������� ��� ������ ��� ����� ������
        foreach (var text in weaponDamageTexts)
        {
            text.gameObject.SetActive(false);
        }
    }

    public void UpdateWeaponDamage(WeaponType weaponType, float damage)
    {
        // ��������� ���� ��� ����������� ������
        if (!weaponDamage.ContainsKey(weaponType))
        {
            weaponDamage[weaponType] = 0;
        }
        weaponDamage[weaponType] += damage;

        // ��������� ����� ����
        UpdateTotalDamage();
    }

    private void UpdateTotalDamage()
    {
        totalDamage = 0;

        foreach (var dmg in weaponDamage.Values)
        {
            totalDamage += dmg;
        }

        // ��������� ����� ����� �����
        totalDamageText.text = $"����� ����: {totalDamage:F2}";

        // ��������� ����� ����� ��� ������� ��������� ������
        int i = 0;
        foreach (var weaponEntry in weaponDamage)
        {
            if (i >= weaponDamageTexts.Length) break;

            float damagePercentage = (totalDamage > 0) ? (weaponEntry.Value / totalDamage) * 100 : 0;
            weaponDamageTexts[i].text = $"{weaponEntry.Key}: {damagePercentage:F1}% | ����: {weaponEntry.Value:F2}";
            weaponDamageTexts[i].gameObject.SetActive(true);
            i++;
        }
    }

    public void ActivateWeaponText(int index)
    {
        if (index >= 0 && index < weaponDamageTexts.Length)
        {
            weaponDamageTexts[index].gameObject.SetActive(true);
        }
    }

    public void ResetWaveDamage()
    {
        weaponDamage.Clear();
        UpdateTotalDamage();
    }
}
