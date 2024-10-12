using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodManager : MonoBehaviour
{
    public GameObject[] bloodObjects; // Массив для хранения всех объектов крови
    public float timeVisible = 7f;    // Время, в течение которого кровь будет видна
    public float fadeDuration = 3f;   // Время плавного исчезновения крови

    // Метод для удаления крови
    public void RemoveAllBlood()
    {
        StartCoroutine(FadeAndDestroyAllBlood());
    }

    private IEnumerator FadeAndDestroyAllBlood()
    {
        // Обновляем массив объектов крови перед началом удаления
        UpdateBloodObjects();

        // Ждем заданное время перед началом удаления
        yield return new WaitForSeconds(timeVisible);

        // Плавное исчезновение каждого объекта крови
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

                    Destroy(blood); // Уничтожаем объект после исчезновения
                }
            }
        }

        // Очищаем массив после удаления
        bloodObjects = new GameObject[0];
    }

    // Метод для обновления массива объектов крови
    public void UpdateBloodObjects()
    {
        bloodObjects = GameObject.FindGameObjectsWithTag("Blood"); // Найдите все объекты крови по тегу
    }
}
