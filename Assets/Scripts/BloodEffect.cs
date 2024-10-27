using System.Collections;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    public GameObject[] bloodTextures; // Массив префабов текстур крови

    // Метод для спавна крови
    public void SpawnBlood(Vector3 position)
    {
        // Выбор случайного префаба крови из массива
        int randomIndex = Random.Range(0, bloodTextures.Length);
        GameObject bloodEffectInstance = Instantiate(bloodTextures[randomIndex], position, Quaternion.identity);

        // Запускаем корутину для удаления крови
        StartCoroutine(RemoveBloodEffect(bloodEffectInstance));
    }

    // Коррутина для удаления эффекта крови
    private IEnumerator RemoveBloodEffect(GameObject bloodEffect)
    {
        // Ждем 4 секунды
        yield return new WaitForSeconds(4f);

        // Плавное исчезновение
        SpriteRenderer spriteRenderer = bloodEffect.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            float fadeDuration = 3f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                spriteRenderer.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // Удаляем объект со сцены
        Destroy(bloodEffect);
    }
}
