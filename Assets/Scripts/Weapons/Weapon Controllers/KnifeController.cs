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
        spawnedKnife.transform.position = transform.position; // ������������� �������
        spawnedKnife.GetComponent<KnifeBehaviour>().DirectionChecker(pm.lastMovedVector); // ������������� �����������
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with: {other.gameObject.name}"); // ���������� ���������
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Damaging enemy!"); // ���������� ��������� ����� ���������� �����
                enemy.TakeDamage(damage); // ������� ���� �����
                Destroy(gameObject); // ������� �������
            }
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Floor"))
        {
            Destroy(gameObject); // ������� ������� ��� ������������ � ���������
        }
    }

}

