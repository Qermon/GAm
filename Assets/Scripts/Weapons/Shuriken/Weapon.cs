using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 10;

    // ����� ��� ���������� �����
    public void IncreaseDamage(int amount)
    {
        damage += amount;
        Debug.Log("���� ������ �������� �� " + amount + ". ����� ����: " + damage);
    }
}
