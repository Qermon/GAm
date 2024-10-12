using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KnifeController : WeaponController
{

    protected override void Start()
    {
        base.Start();
    }

    public void Activate()
    {
        Debug.Log("������������ ����������� 1");
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedKnife = Instantiate(prefab);
        spawnedKnife.transform.position = transform.position; // ������������� �������
        spawnedKnife.GetComponent<KnifeBehaviour>().DirectionChecker(pm.lastMovedVector); // ������������� �����������
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision with: " + collision.name);

        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage((int)damage);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Destructible"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

}

