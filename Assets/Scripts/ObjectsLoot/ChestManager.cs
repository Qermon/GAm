using UnityEngine;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    public GameObject chestPanel; // ������ � ������� �������
    public Button openChestButton; // ������ �������� �������
    public InventoryUIManager inventoryUIManager; // ������ �� ��� ����������� ��������
    public WaveManager waveManager; // ������ �� WaveManager

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        openChestButton.onClick.AddListener(OpenChest);
        chestPanel.SetActive(false); // ������ ������ ��������� � ������
    }

    public void ShowChestPanel()
    {
        chestPanel.SetActive(true); // ���������� ������ �������
    }

    private void OpenChest()
    {
        Item newItem = GenerateRandomItem(); // ���������� ����� �������
        inventoryUIManager.AddItemToInventory(newItem); // ��������� ������� � ���������
        InventoryManager.Instance.AddItemToInventory(newItem);
        chestPanel.SetActive(false); // ��������� ������ �������

        GameManager.GetInstance().RestartGameWithDelay(); // ������������� ����
    }


    private Item GenerateRandomItem()
    {
        // ������ ��������� ���������� ��������
        ItemType randomType = (ItemType)Random.Range(0, System.Enum.GetValues(typeof(ItemType)).Length);
        Sprite randomIcon = LoadRandomIcon(randomType); // �������� ��������� ������ ��� ��������

        return new Item
        {
            itemName = randomType.ToString(),
            itemType = randomType,
            icon = randomIcon, // ���������� ��������� ������
            itemValue = Random.Range(1, 100) // ������ ���������� ��������
        };
    }

    private Sprite LoadRandomIcon(ItemType itemType)
    {
        string path = $"Icons/{itemType}"; // ���� � ����� � ��������
        Sprite[] icons = Resources.LoadAll<Sprite>(path); // ��������� ��� ������� �� �����

        if (icons.Length > 0)
        {
            int randomIndex = Random.Range(0, icons.Length); // ����� ���������� �������
            return icons[randomIndex]; // ���������� ��������� ������
        }

        return null; // ���� ������ �� �������, ���������� null
    }
}
