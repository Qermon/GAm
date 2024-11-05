using UnityEngine;

public class PlayerSelectionManager : MonoBehaviour
{
    public GameObject playerPrefab1;
    public GameObject playerPrefab2;
    public GameObject playerPrefab3;
    public Transform spawnPoint;
    public GameObject playerSelectionPanel;
    private ExperienceBarImage experienceBarImage;
    private PlayerLevelUp playerLevelUp;
    private Shop shop;
    private LevelUpMenu levelUpMenu;
    private PlayerGold playerGold;
    private WaveManager waveManager;
    private WeaponSelectionManager weaponSelectionManager;

    void Start()
    {
        weaponSelectionManager = FindObjectOfType<WeaponSelectionManager>();
        playerGold = FindObjectOfType <PlayerGold>();
        levelUpMenu = FindAnyObjectByType<LevelUpMenu>();
        shop = FindAnyObjectByType <Shop>();
        playerLevelUp = FindObjectOfType<PlayerLevelUp>();
        experienceBarImage = FindObjectOfType<ExperienceBarImage>();
        waveManager = FindObjectOfType<WaveManager>();
    }

    public void SpawnPlayer1()
    {
        SpawnPlayer(playerPrefab1);
        playerSelectionPanel.SetActive(false);
    }

    public void SpawnPlayer2()
    {
        SpawnPlayer(playerPrefab2);
        playerSelectionPanel.SetActive(false);
    }

    public void SpawnPlayer3()
    {
        SpawnPlayer(playerPrefab3);
        playerSelectionPanel.SetActive(false);
    }

    private void SpawnPlayer(GameObject playerPrefab)
    {
        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Спавн игрока: " + playerPrefab.name);

        StartGame();
    }

    private void StartGame()
    {   
        if (weaponSelectionManager.enabled == false && waveManager.enabled == false && shop.enabled == false && levelUpMenu.enabled == false)
        {
            experienceBarImage.enabled = true;
            weaponSelectionManager.enabled = true;
            waveManager.enabled = true;
            shop.enabled = true;
            levelUpMenu.enabled = true;
        }
        else
        {
            experienceBarImage.RestartSkript();
            weaponSelectionManager.RestartScript();
            waveManager.RestartScript();
            shop.RestartScript();
            levelUpMenu.RestartScript();
        }
    }
}