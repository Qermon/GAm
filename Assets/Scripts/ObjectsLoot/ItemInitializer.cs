using UnityEngine;

public class ItemInitializer : MonoBehaviour
{
    public Sprite[] bootsIcons;
    public Sprite[] chestplateIcons;
    public Sprite[] glovesIcons;
    public Sprite[] helmetIcons;
    public Sprite[] pantsIcons;
    public Sprite[] weaponIcons;

    public InventoryManager inventoryManager;

    void Start()
    {
        InitializeRandomItems();
        InitializeItems();
    }

    void InitializeItems()
    {
        // ���������� ��������� � ���������
        inventoryManager.AddItem(new Item("Boots 1", ItemType.Boots, bootsIcons[0], ItemStatType.Defense, 5));
        inventoryManager.AddItem(new Item("Chestplate 1", ItemType.Chestplate, chestplateIcons[0], ItemStatType.Defense, 10));
        inventoryManager.AddItem(new Item("Gloves 1", ItemType.Gloves, glovesIcons[0], ItemStatType.Agility, 3));
        inventoryManager.AddItem(new Item("Helmet 1", ItemType.Helmet, helmetIcons[0], ItemStatType.Health, 15));
        inventoryManager.AddItem(new Item("Pants 1", ItemType.Pants, pantsIcons[0], ItemStatType.Defense, 7));
        inventoryManager.AddItem(new Item("Weapon 1", ItemType.Weapon, weaponIcons[0], ItemStatType.Attack, 20));

        // ��������� UI ����� ����, ��� �������� ���������
        inventoryManager.UpdateUI();
    }

    void InitializeRandomItems()
    {
        // ��������� ��������� �������� � ���������

        // ��������� ������� ��� �������
        AddRandomItem(ItemType.Boots, bootsIcons, ItemStatType.Defense);

        // ��������� ������� ��� �����������
        AddRandomItem(ItemType.Chestplate, chestplateIcons, ItemStatType.Defense);

        // ��������� ������� ��� ��������
        AddRandomItem(ItemType.Gloves, glovesIcons, ItemStatType.Agility);

        // ��������� ������� ��� �����
        AddRandomItem(ItemType.Helmet, helmetIcons, ItemStatType.Health);

        // ��������� ������� ��� ������
        AddRandomItem(ItemType.Pants, pantsIcons, ItemStatType.Defense);

        // ��������� ������� ��� ������
        AddRandomItem(ItemType.Weapon, weaponIcons, ItemStatType.Attack);
    }

    // ����� ��� �������� ���������� ��������
    void AddRandomItem(ItemType itemType, Sprite[] itemIcons, ItemStatType statType)
    {
        // �������� ��������� ������ ������
        int randomIndex = Random.Range(0, itemIcons.Length);

        // ������� ����� ��������� ������� � ��������� ���������
        int randomValue = Random.Range(1, 21); // ��������� �������� ��� �������������� (��������, �� 1 �� 20)

        // ��������� ������� � ���������
        inventoryManager.AddItem(new Item($"{itemType} {randomIndex + 1}", itemType, itemIcons[randomIndex], statType, randomValue));
    }
}
