using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Добавляем для работы с событиями мыши
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

    public VideoClip weaponVideo; // Свойство для хранения видео клипа

    public float damage; // Урон
    public float criticalDamage; // Критический урон
    public float criticalChance; // Критический шанс
    public float attackSpeed; // Скорость атаки
    public float attackRange; // Дальность атаки

    public Sprite skillImage; // Картинка скила для каждого оружия
}
    
public class WeaponSelectionManager : MonoBehaviour
{
    public GameObject weaponSelectionPanel;
    public Image[] weaponIcons;
    public Button[] weaponButtons;
    public Button[] weaponButtons1;
    public TMP_Text[] weaponTexts;
    public TMP_Text[] weaponStats; // Текст для характеристик оружия
    public List<WeaponOption> weaponOptions;
    public TMP_Text[] descriptionTexts; // Массив текстов описания для каждой кнопки
    public Image[] skillImageContainers; // Массив контейнеров для картинок скилов


    private double currentVideoTime = 0; // Переменная для хранения времени видео

    public RawImage videoImage; // Ссылка на RawImage для отображения видео
    public VideoPlayer videoPlayer; // Ссылка на VideoPlayer

    private Player player;
    private WaveManager waveManager;
    private Weapon weapon;
    private CursorManager cursorManager;
    private HashSet<WeaponType> selectedWeapons = new HashSet<WeaponType>(); // Хранение выбранного оружия

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
            description.gameObject.SetActive(false); // Скрываем текст описания в начале
        }
        foreach (Image skillContainer in skillImageContainers)
        {
            skillContainer.gameObject.SetActive(false);
        }

        videoImage.gameObject.SetActive(false); // Скрыть видео в начале
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

            string weaponStatsText = $"Урон: {weapons[i].damage}\n" +
                                     $"Крит. урон: {weapons[i].criticalDamage}%\n" +
                                     $"Крит. шанс: {weapons[i].criticalChance}%\n" +
                                     $"Скорость атаки: {weapons[i].attackSpeed}\n" +
                                     $"Дальность атаки: {weapons[i].attackRange}\n";

            weaponStats[i].text = weaponStatsText;
            weaponStats[i].gameObject.SetActive(false);

            descriptionTexts[i].text = weapons[i].weaponDescription;
            descriptionTexts[i].gameObject.SetActive(true);

            int index = i;
            weaponButtons[i].onClick.RemoveAllListeners();
            weaponButtons[i].onClick.AddListener(() => ChooseWeapon(weapons[index]));
            weaponButtons1[i].onClick.RemoveAllListeners();
            weaponButtons1[i].onClick.AddListener(() => ChooseWeapon(weapons[index]));

            // Добавляем обработчики событий для иконок оружия
            EventTrigger eventTrigger = weaponButtons1[i].gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            pointerEnterEntry.callback.AddListener((data) =>
            {
                ShowWeaponDescription(index);
                PlayWeaponVideo(weapons[index].weaponVideo); // Запускаем видео при наведении
            });
            eventTrigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener((data) =>
            {
                HideWeaponDescription(index);
                StopWeaponVideo(); // Останавливаем видео при выходе
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
            // Устанавливаем клип и время
            videoPlayer.clip = videoClip;
            videoPlayer.time = currentVideoTime; // Устанавливаем сохраненное время

            // Проверяем, активен ли VideoPlayer
            if (!videoPlayer.gameObject.activeSelf)
            {
                videoPlayer.gameObject.SetActive(true);
            }


            StartCoroutine(WaitForHalfSecond());
        }
       
    }

    private IEnumerator WaitForHalfSecond()
    {
        // Ждем 0.5 секунды
     

        // Код, который будет выполнен после задержки
        videoImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
    }

    private void StopWeaponVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            // Сохраняем текущее время видео
            currentVideoTime = videoPlayer.time; // Сохраняем текущее время
            videoPlayer.Stop(); // Останавливаем воспроизведение

            // Скрываем RawImage, когда видео остановлено
            videoImage.gameObject.SetActive(false);
        }
    }




    public void ShowWeaponDescription(int index)
    {
        // Скрываем текст характеристик и показываем текст описания для соответствующего индекса
        weaponStats[index].gameObject.SetActive(true); // Скрываем характеристики
        descriptionTexts[index].gameObject.SetActive(false); // Показываем описание

    }

    public void HideWeaponDescription(int index)
    {
        // Скрываем текст описания и показываем текст характеристик для соответствующего индекса
        descriptionTexts[index].gameObject.SetActive(true); // Скрываем текст описания
        weaponStats[index].gameObject.SetActive(false); // Показываем текст характеристик

    }

    public void ChooseWeapon(WeaponOption weapon)
    {
        if (!selectedWeapons.Contains(weapon.weaponType))
        {
            selectedWeapons.Add(weapon.weaponType); // Добавляем оружие в список выбранных
            ActivateWeaponScript(weapon.weaponType);
            UpdateSkillImageContainers(weapon.skillImage); // Обновляем контейнер с картинками
        }
        CloseWeaponSelection();
    }
    private void UpdateSkillImageContainers(Sprite skillImage)
    {
        // Проходим по массиву контейнеров и находим первый неактивный слот
        for (int i = 0; i < skillImageContainers.Length; i++)
        {
            if (!skillImageContainers[i].gameObject.activeSelf)
            {
                skillImageContainers[i].sprite = skillImage; // Устанавливаем спрайт выбранного оружия
                skillImageContainers[i].gameObject.SetActive(true); // Включаем видимость контейнера
                break; // Прекращаем цикл после вставки в первый свободный слот
            }
        }
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
        cursorManager.HideCursor();
        videoImage.gameObject.SetActive(false);
        weaponSelectionPanel.SetActive(false);
        Time.timeScale = 1;
        // Скрываем все тексты описания при закрытии выбора оружия
        foreach (TMP_Text description in descriptionTexts)
        {
            description.gameObject.SetActive(false);
        }
    }
}
