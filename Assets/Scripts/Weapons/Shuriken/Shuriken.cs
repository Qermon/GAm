using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public int hitCount = 0; // Счетчик попаданий
    public int maxHits = 5; // Максимальное количество попаданий до уничтожения

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            hitCount++;
            Debug.Log($"Hit enemy: {hitCount}");
            if (hitCount >= maxHits)
            {
                Destroy(collision.gameObject); // Уничтожить врага
                Debug.Log("Enemy destroyed");
                DestroyShuriken(); // Уведомляем менеджер о разрушении шурикена
                Destroy(gameObject); // Уничтожить шурикен
            }
        }
    }


    private void DestroyShuriken()
    {
        // Уведомляем менеджер о разрушении шурикена
        ShurikenManager manager = FindObjectOfType<ShurikenManager>();
        if (manager != null)
        {
            manager.OnShurikenDestroyed(this);
        }
    }



}

