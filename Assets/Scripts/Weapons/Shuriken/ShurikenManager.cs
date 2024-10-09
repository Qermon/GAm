using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenManager : MonoBehaviour
{
    public GameObject shurikenPrefab; // ������ ��������
    public float rotationSpeed = 100f; // �������� �������� ���������
    public float shurikenDistance = 1.5f; // ���������� �� ������ �� ��������
    public int maxShurikens = 4; // ������������ ���������� ���������

    private List<Shuriken> shurikens = new List<Shuriken>(); // ������ �������� ���������

    private void Start()
    {
        StartCoroutine(SpawnInitialShurikens());
    }

    private void Update()
    {
        RotateShurikens(); // �������� ���������
    }

    private void RotateShurikens()
    {
        float angleOffset = 360f / maxShurikens; // ���� ����� ����������

        for (int i = 0; i < shurikens.Count; i++)
        {
            Shuriken shuriken = shurikens[i];
            if (shuriken != null)
            {
                float angle = Time.time * rotationSpeed + (angleOffset * i);
                Vector3 newPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * shurikenDistance;
                shuriken.transform.localPosition = newPosition; // ������������� ������� ��������
            }
        }
    }

    private IEnumerator SpawnInitialShurikens()
    {
        for (int i = 0; i < maxShurikens; i++)
        {
            yield return StartCoroutine(SpawnShuriken(i)); // �������� ������ � ��������
        }
    }

    private IEnumerator SpawnShuriken(int index)
    {
        if (shurikens.Count >= maxShurikens) // ���������, �� ��������� �� ���������� ��������� ������������ ��������
            yield break; // ����� �� ������, ���� ��������� ������ ���������

        // ������ ������ ��������
        float angle = (360f / maxShurikens) * index; // ���� ��� �������� ��������
        float radians = angle * Mathf.Deg2Rad; // ��������� � �������
        Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * shurikenDistance; // �������� �� ������ ����

        GameObject shurikenObject = Instantiate(shurikenPrefab, transform.position + offset, Quaternion.identity, transform);

        // ����������� ������ ��������
        shurikenObject.transform.localScale = new Vector3(4f, 4f, 1f); // ���������� ������� ��������

        Shuriken shuriken = shurikenObject.GetComponent<Shuriken>();
        if (shuriken != null)
        {
            shurikens.Add(shuriken); // ��������� ������� � ������
        }
    }

    // ����� ��� ���������� ����������� ���������
    public void EnsureShurikens()
    {
        int shurikensNeeded = maxShurikens - shurikens.Count; // ���������� ����������� ���������
        for (int i = 0; i < shurikensNeeded; i++)
        {
            StartCoroutine(SpawnShuriken(shurikens.Count + i)); // ����� ����� ���������
        }
    }

    // �����, ������� ����� ���������� ��� ��������� ������������� ��������
    public void OnShurikenDestroyed(Shuriken destroyedShuriken)
    {
        shurikens.Remove(destroyedShuriken); // ������� ������� �� ������
        EnsureShurikens(); // ��������� � ��������� ����������� ��������
    }
}
