using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public int hitCount = 0; // ������� ���������
    public int maxHits = 5; // ������������ ���������� ��������� �� �����������

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            hitCount++;
            Debug.Log($"Hit enemy: {hitCount}");
            if (hitCount >= maxHits)
            {
                Destroy(collision.gameObject); // ���������� �����
                Debug.Log("Enemy destroyed");
                DestroyShuriken(); // ���������� �������� � ���������� ��������
                Destroy(gameObject); // ���������� �������
            }
        }
    }


    private void DestroyShuriken()
    {
        // ���������� �������� � ���������� ��������
        ShurikenManager manager = FindObjectOfType<ShurikenManager>();
        if (manager != null)
        {
            manager.OnShurikenDestroyed(this);
        }
    }



}

