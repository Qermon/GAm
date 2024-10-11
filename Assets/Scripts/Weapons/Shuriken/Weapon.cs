using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 10;

    // Метод для увеличения урона
    public void IncreaseDamage(int amount)
    {
        damage += amount;
        Debug.Log("Урон оружия увеличен на " + amount + ". Новый урон: " + damage);
    }
}
