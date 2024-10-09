using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenManager : MonoBehaviour
{
    public GameObject shurikenPrefab; // Префаб шурикена
    public float rotationSpeed = 100f; // Скорость вращения шурикенов
    public float shurikenDistance = 1.5f; // Расстояние от игрока до шурикена
    public int maxShurikens = 4; // Максимальное количество шурикенов

    private List<Shuriken> shurikens = new List<Shuriken>(); // Список активных шурикенов

    private void Start()
    {
        StartCoroutine(SpawnInitialShurikens());
    }

    private void Update()
    {
        RotateShurikens(); // Вращение шурикенов
    }

    private void RotateShurikens()
    {
        float angleOffset = 360f / maxShurikens; // Угол между шурикенами

        for (int i = 0; i < shurikens.Count; i++)
        {
            Shuriken shuriken = shurikens[i];
            if (shuriken != null)
            {
                float angle = Time.time * rotationSpeed + (angleOffset * i);
                Vector3 newPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * shurikenDistance;
                shuriken.transform.localPosition = newPosition; // Устанавливаем позицию шурикена
            }
        }
    }

    private IEnumerator SpawnInitialShurikens()
    {
        for (int i = 0; i < maxShurikens; i++)
        {
            yield return StartCoroutine(SpawnShuriken(i)); // Передаем индекс в корутину
        }
    }

    private IEnumerator SpawnShuriken(int index)
    {
        if (shurikens.Count >= maxShurikens) // Проверяем, не превышает ли количество шурикенов максимальное значение
            yield break; // Выход из метода, если шурикенов больше максимума

        // Логика спавна шурикена
        float angle = (360f / maxShurikens) * index; // Угол для текущего шурикена
        float radians = angle * Mathf.Deg2Rad; // Переводим в радианы
        Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * shurikenDistance; // Смещение на основе угла

        GameObject shurikenObject = Instantiate(shurikenPrefab, transform.position + offset, Quaternion.identity, transform);

        // Увеличиваем размер шурикена
        shurikenObject.transform.localScale = new Vector3(4f, 4f, 1f); // Увеличение размера шурикена

        Shuriken shuriken = shurikenObject.GetComponent<Shuriken>();
        if (shuriken != null)
        {
            shurikens.Add(shuriken); // Добавляем шурикен в список
        }
    }

    // Метод для добавления недостающих шурикенов
    public void EnsureShurikens()
    {
        int shurikensNeeded = maxShurikens - shurikens.Count; // Количество недостающих шурикенов
        for (int i = 0; i < shurikensNeeded; i++)
        {
            StartCoroutine(SpawnShuriken(shurikens.Count + i)); // Спавн новых шурикенов
        }
    }

    // Метод, который может вызываться для обработки уничтоженного шурикена
    public void OnShurikenDestroyed(Shuriken destroyedShuriken)
    {
        shurikens.Remove(destroyedShuriken); // Удаляем шурикен из списка
        EnsureShurikens(); // Проверяем и добавляем недостающие шурикены
    }
}
