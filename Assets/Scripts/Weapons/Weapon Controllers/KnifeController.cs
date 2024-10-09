using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KnifeController : WeaponController
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedKnife = Instantiate(prefab);
        spawnedKnife.transform.position = transform.position; // Устанавливаем позицию
        spawnedKnife.GetComponent<KnifeBehaviour>().DirectionChecker(pm.lastMovedVector); // Устанавливаем направление
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with: {other.gameObject.name}"); // Отладочное сообщение
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Damaging enemy!"); // Отладочное сообщение перед нанесением урона
                enemy.TakeDamage(damage); // Наносим урон врагу
                Destroy(gameObject); // Удаляем сюрикен
            }
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Floor"))
        {
            Destroy(gameObject); // Удаляем сюрикен при столкновении с текстурой
        }
    }

}

