using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenManager : MonoBehaviour
{
    public GameObject shurikenPrefab; // Префаб шурикена
    public float rotationSpeed = 100f; // Скорость вращения шурикенов
    public float shurikenCooldown = 1.5f; // Время перезарядки
    public float shurikenDistance = 1.5f; // Расстояние от игрока до шурикена

    private List<Shuriken> shurikens = new List<Shuriken>(); // Список активных шурикенов
    private Coroutine spawnCoroutine; // Корутин для спавна шурикенов
    private int destroyedCount = 0; // Счетчик уничтоженных шурикенов


    private void Start()
    {
        // Автоматически создаем 4 шурикена при старте
        StartCoroutine(SpawnInitialShurikens());
    }

    private void Update()
    {
        RotateShurikens(); // Вращение шурикенов
    }

    private void RotateShurikens()
    {
        float angleOffset = 360f / shurikens.Count; // Угол между шурикенами
        for (int i = 0; i < shurikens.Count; i++)
        {
            Shuriken shuriken = shurikens[i];
            if (shuriken != null)
            {
                // Вычисляем новый угол
                float angle = Time.time * rotationSpeed + (angleOffset * i);
                // Вычисляем позицию шурикена относительно игрока
                Vector3 newPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * shurikenDistance;
                // Обновляем позицию шурикена
                shuriken.transform.localPosition = newPosition;
            }
        }
    }

    private IEnumerator SpawnInitialShurikens()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return StartCoroutine(SpawnShuriken()); // Создаем шурикен с задержкой
        }
    }

    private IEnumerator SpawnShuriken()
    {
        if (shurikens.Count >= 4) // Проверяем, не превышает ли количество шурикенов 4
            yield break; // Если 4 и больше, выходим из метода

        Vector3 offset = new Vector3(1, 0, 0); // Смещение для спавна
        GameObject shurikenObject = Instantiate(shurikenPrefab, transform.position + offset, Quaternion.identity, transform);

        // Увеличиваем размер шурикена
        shurikenObject.transform.localScale = new Vector3(2f, 2f, 1f); // Увеличение размера шурикена (можете настроить по своему)

        Shuriken shuriken = shurikenObject.GetComponent<Shuriken>();
        if (shuriken != null)
        {
            shurikens.Add(shuriken); // Добавляем шурикен в список
        }

        yield return new WaitForSeconds(shurikenCooldown); // Ждем 1.5 секунды перед созданием следующего шурикена
    }

    public void OnShurikenDestroyed(Shuriken destroyedShuriken)
    {
        destroyedCount++; // Увеличиваем счетчик уничтоженных шурикенов
        shurikens.Remove(destroyedShuriken); // Удаляем уничтоженный шурикен из списка

        // Если активных шурикенов меньше 4 и корутина еще не запущена
        if (shurikens.Count < 4 && spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnShurikenAfterDelay());
        }
    }

    private IEnumerator SpawnShurikenAfterDelay()
    {
        // Сначала ждем 1.5 секунды
        yield return new WaitForSeconds(shurikenCooldown);

        // Спавним новый шурикен
        yield return StartCoroutine(SpawnShuriken());

        // Если были уничтожены больше 1 шурикена, то продолжаем спавнить новые шурикены
        while (destroyedCount > 0 && shurikens.Count < 4)
        {
            destroyedCount--; // Уменьшаем счетчик уничтоженных шурикенов
            yield return new WaitForSeconds(shurikenCooldown); // Ждем 1.5 секунды перед созданием следующего шурикена
            yield return StartCoroutine(SpawnShuriken()); // Спавн нового шурикена
        }

        spawnCoroutine = null; // Сбрасываем корутину, когда достигли 4 шурикена
    }
}