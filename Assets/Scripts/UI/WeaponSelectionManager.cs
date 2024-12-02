using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // ��������� ��� ������ � ��������� ����
using UnityEngine.Video;
using System.Collections;
using Unity.VisualScripting.FullSerializer.Internal;


public enum WeaponType
{
    Lightning,
    FireBall,
    Boomerang,
    Shuriken,
    KillerFog,
    Knife,
    ZeusLight,
    FireStrike,
    Frezzy,
    Peezzy,
    BleedStrike
}

[System.Serializable]
public class WeaponOption
{
    public string weaponName;
    public Sprite weaponSprite;
    public WeaponType weaponType;
    public string weaponDescription;

    public float damage; // ����
    public float criticalDamage; // ����������� ����
    public float criticalChance; // ����������� ����
    public float attackSpeed; // �������� �����
    public float attackRange; // ��������� �����

    public Sprite skillImage; // �������� ����� ��� ������� ������
}

public class WeaponSelectionManager : MonoBehaviour
{
    public GameObject weaponSelectionPanel;
    public Image[] weaponIcons;
    public Button[] weaponButtons;
    public Button[] weaponButtons1;
    public TMP_Text[] weaponTexts;
    public TMP_Text[] weaponStats; // ����� ��� ������������� ������
    public List<WeaponOption> weaponOptions;
    public TMP_Text[] descriptionTexts; // ������ ������� �������� ��� ������ ������
    public Image[] skillImageContainers; // ������ ����������� ��� �������� ������

    private Player player;
    private WaveManager waveManager;
    private Weapon weapon;
    private CursorManager cursorManager;
    private HashSet<WeaponType> selectedWeapons = new HashSet<WeaponType>(); // �������� ���������� ������

    public bool isLightningActive = false;
    public bool isFireBallActive = false;
    public bool isBoomerangActive = false;
    public bool isShurikenActive = false;
    public bool isKnifeActive = false;
    public bool isZeusLightActive = false;
    public bool isFireStrikeActive = false;
    public bool isBleedStrikeActive = false;

    private void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>();
        player = FindObjectOfType<Player>();

        weaponSelectionPanel.SetActive(false);
        foreach (Image icon in weaponIcons)
        {
            icon.gameObject.SetActive(false);
        }
        foreach (TMP_Text description in descriptionTexts)
        {
            description.gameObject.SetActive(false); // �������� ����� �������� � ������
        }
        foreach (Image skillContainer in skillImageContainers)
        {
            skillContainer.gameObject.SetActive(false);
        }
    }

    public void RestartScript()
    {
        // ������� ������ ��������� ������
        selectedWeapons.Clear();

        // ������� ���������� ��� ����������� �������
        foreach (Image skillContainer in skillImageContainers)
        {
            skillContainer.sprite = null; // ������� �����������
            skillContainer.gameObject.SetActive(false); // ������ ��������� ���������
        }

    }



    public void OpenWeaponSelection()
    {
        List<WeaponOption> availableWeapons = GetRandomWeapons(3);
        DisplayWeaponOptions(availableWeapons);
    }

    private List<WeaponOption> GetRandomWeapons(int count)
    {
        List<WeaponOption> availableOptions = new List<WeaponOption>(weaponOptions);
        List<WeaponOption> selectedWeaponsList = new List<WeaponOption>();

        // ������� ��� ��������� ������ �� ���������
        availableOptions.RemoveAll(option => selectedWeapons.Contains(option.weaponType));

        // �������� ��������� ������ �� ���������� ���������
        while (selectedWeaponsList.Count < count && availableOptions.Count > 0)
        {
            WeaponOption selectedWeapon = availableOptions[Random.Range(0, availableOptions.Count)];
            selectedWeaponsList.Add(selectedWeapon);
            availableOptions.Remove(selectedWeapon);
        }

        return selectedWeaponsList;
    }


    private void DisplayWeaponOptions(List<WeaponOption> weapons)
    {
        cursorManager.ShowCursor();
        weaponSelectionPanel.SetActive(true);
        Time.timeScale = 0;

        int countToDisplay = Mathf.Min(weapons.Count, weaponIcons.Length);

        for (int i = 0; i < countToDisplay; i++)
        {
            weaponIcons[i].sprite = weapons[i].weaponSprite;
            weaponIcons[i].gameObject.SetActive(true);
            weaponTexts[i].text = weapons[i].weaponName;
            weaponTexts[i].gameObject.SetActive(true);

            string weaponStatsText = $"����: {weapons[i].damage}\n" +
                                     $"����. ����: {weapons[i].criticalDamage}%\n" +
                                     $"����. ����: {weapons[i].criticalChance}%\n" +
                                     $"�������� �����: {weapons[i].attackSpeed}\n" +
                                     $"��������� �����: {weapons[i].attackRange}\n";

            weaponStats[i].text = weaponStatsText;
            weaponStats[i].gameObject.SetActive(false);

            descriptionTexts[i].text = weapons[i].weaponDescription;
            descriptionTexts[i].gameObject.SetActive(true);

            int index = i;
            weaponButtons[i].onClick.RemoveAllListeners();
            weaponButtons[i].onClick.AddListener(() => ChooseWeapon(weapons[index]));
            weaponButtons1[i].onClick.RemoveAllListeners();
            weaponButtons1[i].onClick.AddListener(() => ChooseWeapon(weapons[index]));

            // ��������� ����������� ������� ��� ������ ������
            EventTrigger eventTrigger = weaponButtons1[i].gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            pointerEnterEntry.callback.AddListener((data) =>
            {
                ShowWeaponDescription(index);
            });
            eventTrigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener((data) =>
            {
                HideWeaponDescription(index);
            });
            eventTrigger.triggers.Add(pointerExitEntry);
        }

        for (int i = countToDisplay; i < weaponIcons.Length; i++)
        {
            weaponIcons[i].gameObject.SetActive(false);
            weaponTexts[i].gameObject.SetActive(false);
            weaponStats[i].gameObject.SetActive(false);
            descriptionTexts[i].gameObject.SetActive(false);
        }
    }

    public void ShowWeaponDescription(int index)
    {
        // �������� ����� ������������� � ���������� ����� �������� ��� ���������������� �������
        weaponStats[index].gameObject.SetActive(true); // �������� ��������������
        descriptionTexts[index].gameObject.SetActive(false); // ���������� ��������

    }

    public void HideWeaponDescription(int index)
    {
        // �������� ����� �������� � ���������� ����� ������������� ��� ���������������� �������
        descriptionTexts[index].gameObject.SetActive(true); // �������� ����� ��������
        weaponStats[index].gameObject.SetActive(false); // ���������� ����� �������������

    }

    public void ChooseWeapon(WeaponOption weapon)
    {
        if (!selectedWeapons.Contains(weapon.weaponType))
        {
            selectedWeapons.Add(weapon.weaponType); // ��������� ������ � ������ ���������
            ActivateWeaponScript(weapon.weaponType);
            UpdateSkillImageContainers(weapon.skillImage); // ��������� ��������� � ����������
        }
        CloseWeaponSelection();
    }
    private void UpdateSkillImageContainers(Sprite skillImage)
    {
        // �������� �� ������� ����������� � ������� ������ ���������� ����
        for (int i = 0; i < skillImageContainers.Length; i++)
        {
            if (!skillImageContainers[i].gameObject.activeSelf)
            {
                skillImageContainers[i].sprite = skillImage; // ������������� ������ ���������� ������
                skillImageContainers[i].gameObject.SetActive(true); // �������� ��������� ����������
                break; // ���������� ���� ����� ������� � ������ ��������� ����
            }
        }
    }

    private void ActivateWeaponScript(WeaponType weaponType)
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("����� �� ������!");
            return;
        }

        // ���������� ������ ������, ��������������� ���������� ���� ������
        switch (weaponType)
        {
            case WeaponType.Lightning:
                var lightningWeapon = player.GetComponent<LightningWeapon>();
                if (lightningWeapon != null)
                {
                    lightningWeapon.enabled = true;
                    isLightningActive = true; // ������������� ����������
                }
                break;

            case WeaponType.FireBall:
                var fireBallController = player.GetComponent<FireBallController>();
                if (fireBallController != null)
                {
                    fireBallController.enabled = true;
                    isFireBallActive = true; // ������������� ����������
                }
                break;

            case WeaponType.Boomerang:
                var boomerangController = player.GetComponent<BoomerangController>();
                if (boomerangController != null)
                {
                    boomerangController.enabled = true;
                    isBoomerangActive = true; // ������������� ����������
                }
                break;

            case WeaponType.Shuriken:
                var shuriken = player.GetComponent<Shuriken>();
                if (shuriken != null)
                {
                    shuriken.enabled = true;
                    isShurikenActive = true; // ������������� ����������
                }
                break;

            case WeaponType.Knife:
                var knifeController = player.GetComponent<KnifeController>();
                if (knifeController != null)
                {
                    knifeController.enabled = true;
                    isKnifeActive = true; // ������������� ����������
                }
                break;

            case WeaponType.ZeusLight:
                var zeusLight = player.GetComponent<ZeusLight>();
                if (zeusLight != null)
                {
                    zeusLight.enabled = true;
                    isZeusLightActive = true; // ������������� ����������
                }
                break;

            case WeaponType.FireStrike:
                var fireStrike = player.GetComponent<FireStrike>();
                if (fireStrike != null)
                {
                    fireStrike.enabled = true;
                    isFireStrikeActive = true; // ������������� ����������
                }
                break;

            case WeaponType.BleedStrike:
                var bleedStrike = player.GetComponent<BleedStrike>();
                if (bleedStrike != null)
                {
                    bleedStrike.enabled = true;
                    isBleedStrikeActive = true; // ������������� ����������
                }
                break;

            default:
                Debug.LogWarning("����������� ��� ������: " + weaponType);
                break;

        }
    }
        public void CloseWeaponSelection()
    {
        cursorManager.HideCursor();
        weaponSelectionPanel.SetActive(false);
        Time.timeScale = 1;
        // �������� ��� ������ �������� ��� �������� ������ ������
        foreach (TMP_Text description in descriptionTexts)
        {
            description.gameObject.SetActive(false);
        }
    }
}