using System.Collections; // �������� ���
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image damageImage; // ������ �� ��������� �����
    public float flashDuration = 0.2f; // ������������ �������
    private Coroutine flashCoroutine;

    // ����� ��� ������, ����� ����� �������� ����
    public void ShowDamageIndicator()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashDamageIndicator());
    }

    private IEnumerator FlashDamageIndicator()
    {
        damageImage.enabled = true;
        Color originalColor = damageImage.color;

        // ��������� ���������� ����� � �����������������
        damageImage.color = new Color(1, 0, 0, 0.05f); // �������������� �������

        yield return new WaitForSeconds(flashDuration);

        // ���������� ������������ ���� (������� �����-�����)
        damageImage.color = originalColor;
        damageImage.enabled = false; // �������� ���������
    }

}
