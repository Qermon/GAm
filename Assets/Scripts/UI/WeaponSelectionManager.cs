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

    public VideoClip weaponVideo; // �������� ��� �������� ����� �����

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


    private double currentVideoTime = 0; // ���������� ��� �������� ������� �����

    public RawImage videoImage; // ������ �� RawImage ��� ����������� �����
    public VideoPlayer videoPlayer; // ������ �� VideoPlayer

    private Player player;
    private WaveManager waveManager;
    private Weapon weapon;
    private CursorManager cursorManager;
    private HashSet<WeaponType> selectedWeapons = new HashSet<WeaponType>(); // �������� ���������� ������

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

        videoImage.gameObject.SetActive(false); // ������ ����� � ������
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
                PlayWeaponVideo(weapons[index].weaponVideo); // ��������� ����� ��� ���������
            });
            eventTrigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener((data) =>
            {
                HideWeaponDescription(index);
                StopWeaponVideo(); // ������������� ����� ��� ������
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


    private void PlayWeaponVideo(VideoClip videoClip)
    {
        if (videoClip != null && videoPlayer != null)
        {
            // ������������� ���� � �����
            videoPlayer.clip = videoClip;
            videoPlayer.time = currentVideoTime; // ������������� ����������� �����

            // ���������, ������� �� VideoPlayer
            if (!videoPlayer.gameObject.activeSelf)
            {
                videoPlayer.gameObject.SetActive(true);
            }


            StartCoroutine(WaitForHalfSecond());
        }
       
    }

    private IEnumerator WaitForHalfSecond()
    {
        // ���� 0.5 �������
     

        // ���, ������� ����� �������� ����� ��������
        videoImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
    }

    private void StopWeaponVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            // ��������� ������� ����� �����
            currentVideoTime = videoPlayer.time; // ��������� ������� �����
            videoPlayer.Stop(); // ������������� ���������������

            // �������� RawImage, ����� ����� �����������
            videoImage.gameObject.SetActive(false);
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
                Debug.LogWarning("����������� ��� ������: " + weaponType);
                break;
        }
    }

    public void CloseWeaponSelection()
    {
        cursorManager.HideCursor();
        videoImage.gameObject.SetActive(false);
        weaponSelectionPanel.SetActive(false);
        Time.timeScale = 1;
        // �������� ��� ������ �������� ��� �������� ������ ������
        foreach (TMP_Text description in descriptionTexts)
        {
            description.gameObject.SetActive(false);
        }
    }
}
