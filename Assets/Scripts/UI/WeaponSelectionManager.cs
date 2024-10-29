using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

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
    FreezeStrike,
    PoisonStrike,
    BleedStrike
}

[System.Serializable]
public class WeaponOption
{
    public string weaponName;
    public Sprite weaponSprite;
    public WeaponType weaponType;
    public string weaponDescription;
}

public class WeaponSelectionManager : MonoBehaviour
{
    public GameObject weaponSelectionPanel;
    public Image[] weaponIcons;
    public Button[] weaponButtons;
    public TMP_Text[] weaponTexts;
    public List<WeaponOption> weaponOptions;

    private Player player;
    private WaveManager waveManager;
    private HashSet<WeaponType> selectedWeapons = new HashSet<WeaponType>(); // Хранение выбранного оружия

    private void Start()
    {
        player = FindObjectOfType<Player>();
        waveManager = FindObjectOfType<WaveManager>();

        weaponSelectionPanel.SetActive(false);
        foreach (Image icon in weaponIcons)
        {
            icon.gameObject.SetActive(false);
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

        // Убираем уже выбранные оружия из доступных
        availableOptions.RemoveAll(option => selectedWeapons.Contains(option.weaponType));

        // Выбираем случайные оружия из оставшихся доступных
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
        weaponSelectionPanel.SetActive(true);
        Time.timeScale = 0;

        int countToDisplay = Mathf.Min(weapons.Count, weaponIcons.Length);

        for (int i = 0; i < countToDisplay; i++)
        {
            weaponIcons[i].sprite = weapons[i].weaponSprite;
            weaponIcons[i].gameObject.SetActive(true);
            weaponTexts[i].text = weapons[i].weaponDescription;
            weaponTexts[i].gameObject.SetActive(true);

            int index = i;
            weaponButtons[i].onClick.RemoveAllListeners();
            weaponButtons[i].onClick.AddListener(() => ChooseWeapon(weapons[index]));
        }

        for (int i = countToDisplay; i < weaponIcons.Length; i++)
        {
            weaponIcons[i].gameObject.SetActive(false);
            weaponTexts[i].gameObject.SetActive(false);
        }
    }

    public void ChooseWeapon(WeaponOption weapon)
    {
        if (!selectedWeapons.Contains(weapon.weaponType))
        {
            selectedWeapons.Add(weapon.weaponType); // Добавляем оружие в список выбранных
            ActivateWeaponScript(weapon.weaponType);
        }
        CloseWeaponSelection();
    }

    private void ActivateWeaponScript(WeaponType weaponType)
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Игрок не найден!");
            return;
        }

        // Активируем только скрипт, соответствующий выбранному типу оружия
        switch (weaponType)
        {
            case WeaponType.Lightning:
                var lightningWeapon = player.GetComponent<LightningWeapon>();
                if (lightningWeapon != null)
                {
                    lightningWeapon.enabled = true;
                }
                break;

            case WeaponType.FireBall:
                var fireBallController = player.GetComponent<FireBallController>();
                if (fireBallController != null)
                {
                    fireBallController.enabled = true;
                }
                break;

            case WeaponType.Boomerang:
                var boomerangController = player.GetComponent<BoomerangController>();
                if (boomerangController != null)
                {
                    boomerangController.enabled = true;
                }
                break;

            case WeaponType.Shuriken:
                var shuriken = player.GetComponent<Shuriken>();
                if (shuriken != null)
                {
                    shuriken.enabled = true;
                }
                break;

            case WeaponType.KillerFog:
                var killerFog = player.GetComponent<KillerFog>();
                if (killerFog != null)
                {
                    killerFog.enabled = true;
                }
                break;

            case WeaponType.Knife:
                var knifeController = player.GetComponent<KnifeController>();
                if (knifeController != null)
                {
                    knifeController.enabled = true;
                }
                break;

            case WeaponType.ZeusLight:
                var zeusLight = player.GetComponent<ZeusLight>();
                if (zeusLight != null)
                {
                    zeusLight.enabled = true;
                }
                break;

            case WeaponType.FireStrike:
                var fireStrike = player.GetComponent<FireStrike>();
                if (fireStrike != null)
                {
                    fireStrike.enabled = true;
                }
                break;

            case WeaponType.FreezeStrike:
                var freezeStrike = player.GetComponent<FreezeStrike>();
                if (freezeStrike != null)
                {
                    freezeStrike.enabled = true;
                }
                break;

            case WeaponType.PoisonStrike:
                var poisonStrike = player.GetComponent<PoisonStrike>();
                if (poisonStrike != null)
                {
                    poisonStrike.enabled = true;
                }
                break;

            case WeaponType.BleedStrike:
                var bleedStrike = player.GetComponent<BleedStrike>();
                if (bleedStrike != null)
                {
                    bleedStrike.enabled = true; 
                }
                break;

            default:
                Debug.LogWarning("Неизвестный тип оружия: " + weaponType);
                break;
        }
    }

    public void CloseWeaponSelection()
    {
        weaponSelectionPanel.SetActive(false);
        Time.timeScale = 1;
    }
}
