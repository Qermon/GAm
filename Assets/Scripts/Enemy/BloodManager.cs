using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodManager : MonoBehaviour
{
    public GameObject[] bloodObjects; // ������ ��� �������� ���� �������� �����
    public float timeVisible = 7f;    // �����, � ������� �������� ����� ����� �����
    public float fadeDuration = 3f;   // ����� �������� ������������ �����

    // ����� ��� �������� �����
    public void RemoveAllBlood()
    {
        StartCoroutine(FadeAndDestroyAllBlood());
    }

    private IEnumerator FadeAndDestroyAllBlood()
    {
        // ��������� ������ �������� ����� ����� ������� ��������
        UpdateBloodObjects();

        // ���� �������� ����� ����� ������� ��������
        yield return new WaitForSeconds(timeVisible);

        // ������� ������������ ������� ������� �����
        foreach (GameObject blood in bloodObjects)
        {
            if (blood != null)
            {
                SpriteRenderer bloodRenderer = blood.GetComponent<SpriteRenderer>();
                if (bloodRenderer != null)
                {
                    Color originalColor = bloodRenderer.color;
                    float timeElapsed = 0f;

                    while (timeElapsed < fadeDuration)
                    {
                        timeElapsed += Time.deltaTime;
                        float alpha = Mathf.Lerp(1, 0, timeElapsed / fadeDuration);
                        bloodRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                        yield return null;
                    }

                    Destroy(blood); // ���������� ������ ����� ������������
                }
            }
        }

        // ������� ������ ����� ��������
        bloodObjects = new GameObject[0];
    }

    // ����� ��� ���������� ������� �������� �����
    public void UpdateBloodObjects()
    {
        bloodObjects = GameObject.FindGameObjectsWithTag("Blood"); // ������� ��� ������� ����� �� ����
    }
}
