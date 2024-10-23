using UnityEngine;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel; // Префаб магазина

    private void Start()
    {
        // Скрыть магазин в начале
        shopPanel.SetActive(false);
    }

    public void ShowShop()
    {
        shopPanel.SetActive(true);
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
    }

    // Добавьте методы для покупок
    public void BuyItem(int itemId)
    {
        // Логика покупки
        Debug.Log("Покупка предмета с ID: " + itemId);
        // Уменьшите золото и другие действия
    }
}
