using System.Collections;
using UnityEngine;

public class BloodManager : MonoBehaviour
{
    public float timeVisible = 5f;    // Время, в течение которого кровь будет видна
    public float fadeDuration = 3f;    // Время плавного исчезновения крови

    // Метод для удаления всех объектов крови
    public void RemoveAllBlood()
    {
        StartCoroutine(FadeAndDestroyAllBlood());
    }

    // Новый метод для удаления конкретного объекта крови
    public IEnumerator RemoveBlood(GameObject blood)
    {
        // Ждем заданное время перед началом удаления
        yield return new WaitForSeconds(timeVisible);

        // Плавное исчезновение объекта крови
        yield return StartCoroutine(FadeAndDestroyBlood(blood));
    }

    private IEnumerator FadeAndDestroyAllBlood()
    {
        yield return new WaitForSeconds(timeVisible);

        GameObject[] bloodObjects = GameObject.FindGameObjectsWithTag("Blood");

        if (bloodObjects.Length == 0)
        {
            Debug.Log("Нет объектов крови для удаления.");
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
                Debug.LogWarning("Объект крови равен null.");
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

            Destroy(blood); // Уничтожаем объект после исчезновения
        }
        else
        {
            Debug.LogWarning($"SpriteRenderer не найден для объекта: {blood.name}");
        }
    }
}
