using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ChestManager : MonoBehaviour
{
    public GameObject chestPanel;
    public Button openChestButton;
    public InventoryUIManager inventoryUIManager;
    public WaveManager waveManager;

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        openChestButton.onClick.AddListener(OpenChest);
        chestPanel.SetActive(false);
    }

    public void ShowChestPanel()
    {
        chestPanel.SetActive(true);
    }

    private void OpenChest()
    {
        Item newItem = GenerateRandomItem();
        chestPanel.SetActive(false);

        GameManager.GetInstance().RestartGameWithDelay();
    }

    private Item GenerateRandomItem()
    {
        ItemType randomType = (ItemType)Random.Range(0, System.Enum.GetValues(typeof(ItemType)).Length);
        Sprite randomIcon = LoadRandomIcon(randomType);

        // Случайная характеристика и её значение в зависимости от волны
        ItemStatType statType = (ItemStatType)Random.Range(0, System.Enum.GetValues(typeof(ItemStatType)).Length);
        int wave = waveManager.waveNumber;
        int statValue = GetStatValueForWave(statType, wave);

        return new Item(randomType.ToString(), randomType, randomIcon, statType, statValue);
    }

    private int GetStatValueForWave(ItemStatType statType, int wave)
    {
        // Определяем значение баффа в зависимости от волны (например, от 3 до 7 для волны 5)
        int minValue = wave / 2;
        int maxValue = wave + 2;
        return Random.Range(minValue, maxValue);
    }

    private Sprite LoadRandomIcon(ItemType itemType)
    {
        string path = $"Icons/{itemType}";
        Sprite[] icons = Resources.LoadAll<Sprite>(path);

        if (icons.Length > 0)
        {
            int randomIndex = Random.Range(0, icons.Length);
            return icons[randomIndex];
        }

        return null;
    }
}
