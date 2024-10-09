using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenManager : MonoBehaviour
{
    public GameObject shurikenPrefab; // ������ ��������
    public float rotationSpeed = 100f; // �������� �������� ���������
    public float shurikenCooldown = 1.5f; // ����� �����������
    public float shurikenDistance = 1.5f; // ���������� �� ������ �� ��������

    private List<Shuriken> shurikens = new List<Shuriken>(); // ������ �������� ���������
    private Coroutine spawnCoroutine; // ������� ��� ������ ���������
    private int destroyedCount = 0; // ������� ������������ ���������


    private void Start()
    {
        // ������������� ������� 4 �������� ��� ������
        StartCoroutine(SpawnInitialShurikens());
    }

    private void Update()
    {
        RotateShurikens(); // �������� ���������
    }

    private void RotateShurikens()
    {
        float angleOffset = 360f / shurikens.Count; // ���� ����� ����������
        for (int i = 0; i < shurikens.Count; i++)
        {
            Shuriken shuriken = shurikens[i];
            if (shuriken != null)
            {
                // ��������� ����� ����
                float angle = Time.time * rotationSpeed + (angleOffset * i);
                // ��������� ������� �������� ������������ ������
                Vector3 newPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * shurikenDistance;
                // ��������� ������� ��������
                shuriken.transform.localPosition = newPosition;
            }
        }
    }

    private IEnumerator SpawnInitialShurikens()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return StartCoroutine(SpawnShuriken()); // ������� ������� � ���������
        }
    }

    private IEnumerator SpawnShuriken()
    {
        if (shurikens.Count >= 4) // ���������, �� ��������� �� ���������� ��������� 4
            yield break; // ���� 4 � ������, ������� �� ������

        Vector3 offset = new Vector3(1, 0, 0); // �������� ��� ������
        GameObject shurikenObject = Instantiate(shurikenPrefab, transform.position + offset, Quaternion.identity, transform);

        // ����������� ������ ��������
        shurikenObject.transform.localScale = new Vector3(2f, 2f, 1f); // ���������� ������� �������� (������ ��������� �� ������)

        Shuriken shuriken = shurikenObject.GetComponent<Shuriken>();
        if (shuriken != null)
        {
            shurikens.Add(shuriken); // ��������� ������� � ������
        }

        yield return new WaitForSeconds(shurikenCooldown); // ���� 1.5 ������� ����� ��������� ���������� ��������
    }

    public void OnShurikenDestroyed(Shuriken destroyedShuriken)
    {
        destroyedCount++; // ����������� ������� ������������ ���������
        shurikens.Remove(destroyedShuriken); // ������� ������������ ������� �� ������

        // ���� �������� ��������� ������ 4 � �������� ��� �� ��������
        if (shurikens.Count < 4 && spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnShurikenAfterDelay());
        }
    }

    private IEnumerator SpawnShurikenAfterDelay()
    {
        // ������� ���� 1.5 �������
        yield return new WaitForSeconds(shurikenCooldown);

        // ������� ����� �������
        yield return StartCoroutine(SpawnShuriken());

        // ���� ���� ���������� ������ 1 ��������, �� ���������� �������� ����� ��������
        while (destroyedCount > 0 && shurikens.Count < 4)
        {
            destroyedCount--; // ��������� ������� ������������ ���������
            yield return new WaitForSeconds(shurikenCooldown); // ���� 1.5 ������� ����� ��������� ���������� ��������
            yield return StartCoroutine(SpawnShuriken()); // ����� ������ ��������
        }

        spawnCoroutine = null; // ���������� ��������, ����� �������� 4 ��������
    }
}