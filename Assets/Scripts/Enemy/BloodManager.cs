using System.Collections;
using UnityEngine;

public class BloodManager : MonoBehaviour
{
    public float timeVisible = 5f;    // �����, � ������� �������� ����� ����� �����
    public float fadeDuration = 3f;    // ����� �������� ������������ �����

    // ����� ��� �������� ���� �������� �����
    public void RemoveAllBlood()
    {
        StartCoroutine(FadeAndDestroyAllBlood());
    }

    // ����� ����� ��� �������� ����������� ������� �����
    public IEnumerator RemoveBlood(GameObject blood)
    {
        // ���� �������� ����� ����� ������� ��������
        yield return new WaitForSeconds(timeVisible);

        // ������� ������������ ������� �����
        yield return StartCoroutine(FadeAndDestroyBlood(blood));
    }

    private IEnumerator FadeAndDestroyAllBlood()
    {
        yield return new WaitForSeconds(timeVisible);

        GameObject[] bloodObjects = GameObject.FindGameObjectsWithTag("Blood");

        if (bloodObjects.Length == 0)
        {
            Debug.Log("��� �������� ����� ��� ��������.");
            yield break;
        }

        foreach (GameObject blood in bloodObjects)
        {
            if (blood != null)
            {
                yield return StartCoroutine(FadeAndDestroyBlood(blood));
            }
            else
            {
                Debug.LogWarning("������ ����� ����� null.");
            }
        }
    }

    public IEnumerator FadeAndDestroyBlood(GameObject blood)
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
        else
        {
            Debug.LogWarning($"SpriteRenderer �� ������ ��� �������: {blood.name}");
        }
    }
}
