using UnityEngine;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel; // ������ ��������

    private void Start()
    {
        // ������ ������� � ������
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

    // �������� ������ ��� �������
    public void BuyItem(int itemId)
    {
        // ������ �������
        Debug.Log("������� �������� � ID: " + itemId);
        // ��������� ������ � ������ ��������
    }
}
