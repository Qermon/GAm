using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WaveManager : MonoBehaviour
{
    public Shop shop;
    public GameObject[] deathMobPrefabs;  // Префабы мобов Смерти
    public GameObject[] deathPrefabs;
    public GameObject[] batPrefabs;       // Префабы мобов Летучих мышей
    public GameObject[] wizardPrefabs;    // Префабы мобов магов
    public GameObject[] samuraiPrefabs;  // Префабы скелетов
    public GameObject[] archerPrefabs;    // Префабы лучников
    public GameObject[] boomPrefabs;      // Префабы бомбардировщиков
    public GameObject[] healerPrefabs;    // Префабы лекарей
    public GameObject[] buffMobPrefabs;   // Префабы бафферов
    public GameObject[] bossPrefabs;
    public GameObject[] ArchersArrow;
    public GameObject[] WitchsProjectile;
    public Transform[] spawnPoints;       // Точки спауна
    public float spawnRadius = 1f; // Радиус для разброса спавна вокруг точки
    public float timeBetweenWaves = 5f;

    public int waveNumber = 0;
    public int maxWaves = 30;
    public int killCount; // Счетчик убийств для волны на убийства
    public int killThreshold; // Порог убийств для завершения волны
    public int killCountAll;

    private bool spawningWave = false;
    private bool isSpawningActive = true; // Флаг для контроля спавна врагов

    private float timeStartedWave;
    private float waveDuration;

    public float damageMultiplier;
    public float healthMultiplier;
    public float speedMultiplier ;
    public float projectile;

    public Transform player; // Ссылка на объект игрока
    public Vector2 centerOfMap = new Vector2(18.5f, -12.5f); // Координаты центра карты

    public GameObject crossPrefab; // Префаб крестика для обозначения места появления мобов


    private bool isWaveInProgress = false;

    private PlayerHealth playerHealth; // Добавьте ссылку на PlayerHealth
    public WeaponSelectionManager weaponSelectionManager; // Ссылка на ваш скрипт выбора оружия
    private List<GameObject> activeEnemies = new List<GameObject>();
    private WaveConfig currentWaveConfig;

    private Dictionary<int, WaveConfig> waveConfigs;



    void Start()
    {

        if (waveConfigs != null && waveConfigs.TryGetValue(waveNumber, out currentWaveConfig))
        {
            // Обновляем killThreshold для текущей волны
            UpdateKillThreshold(currentWaveConfig);
        }
        // Найти объект игрока на сцене и сохранить его ссылку
        PlayerHealth playerObject = FindObjectOfType<PlayerHealth>(); // Предполагается, что ваш класс игрока называется Player
        if (playerObject != null)
        {
            player = playerObject.transform; // Сохраняем трансформ игрока
        }
        else
        {
            Debug.LogWarning("Игрок не найден на сцене!");
        }

        weaponSelectionManager = FindObjectOfType<WeaponSelectionManager>(); // Присваиваем weaponSelectionManager
        playerHealth = FindObjectOfType<PlayerHealth>(); // Инициализируйте ссылку

        InitializeWaves(); // Убедимся, что инициализация происходит до начала игры

        if (waveConfigs == null || waveConfigs.Count == 0)
        {
            Debug.LogError("Ошибка инициализации волн: waveConfigs не инициализирован или пустой.");
            return;
        }

        EndWave();
        StartWave();
    }

    // Метод для инициализации или обновления значения killThreshold для текущей волны
    public void UpdateKillThreshold(WaveConfig waveConfig)
    {
        int totalEnemies = 0;

        // Считаем количество мобов, которые будут заспавнены в текущей волне
        foreach (var spawn in waveConfig.enemiesToSpawn)
        {
            totalEnemies += spawn.count;
        }

        // Логируем, сколько мобов посчитано для текущей волны
        Debug.Log($"Total enemies for wave: {totalEnemies}");

        // Обновляем killThreshold для текущей волны
        killThreshold = totalEnemies - 20;
    }   


    public void RestartScript()
    {
        isSpawningActive = false;

        PlayerHealth playerObject = FindObjectOfType<PlayerHealth>(); // Получаем объект игрока
        if (playerObject != null)
        {
            player = playerObject.transform; // Сохраняем трансформ игрока
        }
        else
        {
            Debug.LogWarning("Игрок не найден на сцене!");
        }

        playerHealth = FindObjectOfType<PlayerHealth>(); // Инициализируем ссылку на здоровье игрока

        // Уничтожаем всех активных врагов и очищаем список
        foreach (GameObject enemy in activeEnemies)
        {
            Destroy(enemy); // Уничтожаем каждого активного врага
        }
        activeEnemies.Clear(); // Очищаем список активных врагов

        // Проверяем, идет ли волна и останавливаем корутины спавна, если это так
        if (isWaveInProgress)
        {
            StopAllCoroutines(); // Остановите все корутины, связанные со спавном
            EndWave(); // Завершите текущую волну
        }

        // Сбрасываем параметры
        waveNumber = 0; // Сбрасываем номер волны
        isWaveInProgress = false; // Обновляем состояние волны
        spawningWave = false; // Останавливаем спавн

        // Перемещаем игрока в центр карты
        MovePlayerToCenter();

        // Инициализируем конфигурации волн заново
        InitializeWaves();

        // Запускаем новую волну
        StartCoroutine(StartNextWave()); // Измените на вызов StartNextWave, чтобы убедиться, что волна начинается заново
    }


    void Update()
    {
        if (!spawningWave && activeEnemies.Count == 0 && waveNumber < maxWaves)
        {
            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartNextWave()
    {

        if (isWaveInProgress) yield break; // Если волна уже в процессе, выходим

        isWaveInProgress = true; // Устанавливаем флаг

        PlayerGold playerGold = FindObjectOfType<PlayerGold>();
        playerGold.OnNewWaveStarted(); // Сбрасываем флаг

        yield return new WaitForSeconds(timeBetweenWaves);
        StartWave();

        playerHealth.UpdateShield();
        playerHealth.UpdateBarrierUI();
        playerHealth.ApplyHealthRegenAtWaveStart();
        playerHealth.ResetBarrierOnLowHealthBuff(); // Сброс состояния барьера на новую волну

        isWaveInProgress = false; // Сбрасываем флаг после завершения
    }

    private void MovePlayerToCenter()
    {
        // Перемещение игрока в центр карты
        player.position = centerOfMap;
    }

    public void StartWave()
    {

        Debug.Log("Начало волны, текущий номер: " + waveNumber);
        killCount = 0; // Сброс счетчика убийств

        Debug.Log("Начало волны, текущий номер: " + waveNumber);
        if (weaponSelectionManager == null)
        {
            Debug.LogError("WeaponSelectionManager не инициализирован!");
            return; // Остановите выполнение, если weaponSelectionManager не инициализирован
        }

        // Проверяем, нужно ли открывать панель выбора оружия
        if (waveNumber == 0 || waveNumber == 2 || waveNumber == 5 || waveNumber == 8)
        {
            weaponSelectionManager.OpenWeaponSelection(); // Открываем панель выбора оружия
        }

        if (waveConfigs == null || waveConfigs.Count == 0)
        {
            return;
        }

        if (waveNumber == 0)
        {
            ResetEnemyStats();
        }

        if (waveNumber <= 10 && waveNumber != 0)
        {
            UpdateEnemyStats();

        }

        if (waveNumber > 10)
        {
            UpdateEnemyStats();
            UpdateEnemyStats();
        }

        if (!spawningWave)
        {
            // Проверяем, был ли куплен бафф критического шанса
            foreach (var weapon in FindObjectsOfType<Weapon>())
            {
                if (weapon != null) // Убедитесь, что оружие существует
                {
                    weapon.ActivateCritChanceBuff(); // Активируем бафф на всех оружиях
                    weapon.ActivateCritDamageBuff(); // Перезапускаем бафф для критического урона
                }
            }

            isSpawningActive = true;
            StartCoroutine(SpawnWave());


        }
    }

    private IEnumerator SpawnWave()
    {


        waveNumber++;
        if (waveConfigs != null && waveConfigs.TryGetValue(waveNumber, out currentWaveConfig))
        {
            UpdateKillThreshold(currentWaveConfig);  // Обновляем killThreshold для текущей волны
        }
        else
        {
            Debug.LogError($"Конфигурация для волны {waveNumber} не найдена!");
            yield break;
        }

        spawningWave = true;
       

        if (waveConfigs != null && waveConfigs.ContainsKey(waveNumber))
        {
            WaveConfig currentWave = waveConfigs[waveNumber];
            waveDuration = currentWave.waveDuration;
            timeStartedWave = Time.time;

            List<EnemySpawn> allEnemiesToSpawn = new List<EnemySpawn>(currentWave.enemiesToSpawn);

            bool isKillWave = (waveNumber % 3 == 0); // Каждая третья волна на убийства
            float spawnTimeLimit = waveDuration - 7f; // Время для спавна мобов (до 7 секунд до конца волны)
            int totalEnemyTypes = allEnemiesToSpawn.Count;
            int[] enemiesLeftToSpawn = allEnemiesToSpawn.Select(e => e.count).ToArray();
            float[] nextSpawnTime = new float[totalEnemyTypes];
            killCount = 0; // Сбрасываем счетчик убийств для новой волны

            // Рассчитываем интервал спавна для каждого типа врага
            for (int i = 0; i < totalEnemyTypes; i++)
            {
                allEnemiesToSpawn[i].CalculateSpawnInterval(spawnTimeLimit); // Используем spawnTimeLimit для спавна
                nextSpawnTime[i] = timeStartedWave + 0.25f; // Первые 2 секунды враги не спавнятся
            }

            // Основной цикл спавна врагов
            while (isSpawningActive)
            {
                for (int i = 0; i < totalEnemyTypes; i++)
                {
                    if (enemiesLeftToSpawn[i] > 0 && Time.time >= nextSpawnTime[i])
                    {
                        EnemySpawn enemySpawn = allEnemiesToSpawn[i];
                        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                        Vector2 spawnPosition = spawnPoint.position + (Vector3)Random.insideUnitCircle * spawnRadius;

                        // Запуск корутины для спавна крестика и моба
                        StartCoroutine(SpawnCrossAndEnemy(spawnPosition, enemySpawn.enemyPrefab));

                        enemiesLeftToSpawn[i]--;
                        nextSpawnTime[i] = Time.time + enemySpawn.spawnInterval; // Используем рассчитанный интервал
                    }
                }

                // Для волн на убийства (каждая третья)
                if (isKillWave && killCount >= killThreshold)
                {
                    break; // Выход из цикла, если убито достаточно мобов
                }

                // Для волн на время, проверяем, если время волны истекло
                if (!isKillWave && Time.time - timeStartedWave >= spawnTimeLimit)
                {
                    break; // Выход из цикла, если время для спавна мобов истекло
                }

                yield return null; // Ждем следующий кадр
            }

            // Если волна не на убийства, ждем еще 7 секунд после спавна всех мобов
            if (!isKillWave)
            {
                float timeToEndWave = Time.time + 7f; // Еще 7 секунд до конца волны

                // Ожидаем 7 секунд, прежде чем завершить волну
                while (Time.time < timeToEndWave && isSpawningActive)
                {
                    yield return null; // Ждем 7 секунд
                }
            }

            // Завершаем волну
            RemoveRemainingEnemies();
            EndWave();
            shop.OpenShop();
        }
        else
        {
            Debug.LogError("WaveConfig для waveNumber " + waveNumber + " не найден или waveConfigs не инициализировано.");
        }

        spawningWave = false;
        StartCoroutine(StartNextWave());
        playerHealth.barrierActivatedThisWave = false;
    }




    // Метод для спавна крестика и моба
    private IEnumerator SpawnCrossAndEnemy(Vector3 spawnPosition, GameObject enemyPrefab)
    {
        // Спавним крестик за 1.75 секунды до моба
        GameObject cross = Instantiate(crossPrefab, spawnPosition, Quaternion.identity);

        // Ждем 1.75 секунды перед спавном моба
        yield return new WaitForSeconds(1.75f);

        // Проверяем, активен ли спавн
        if (!isSpawningActive)
        {
            // Удаляем крестик, если спавн был отключен
            Destroy(cross);
            yield break; // Завершаем корутину, не спавня моба
        }

        // Удаляем крестик перед спавном моба
        Destroy(cross);

        // Спавним моба
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        AddEnemy(enemy);
    }

    private void UpdateEnemyStats()
    {
        // Цикл для мобов Death
        foreach (GameObject prefab in deathMobPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.UpdateStats(damageMultiplier, healthMultiplier, speedMultiplier);
            }
        }

        foreach (GameObject prefab in deathPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.UpdateStats(damageMultiplier, healthMultiplier, speedMultiplier);
            }
        }

        // Цикл для мобов Bat
        foreach (GameObject prefab in batPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.UpdateStats(damageMultiplier, healthMultiplier, speedMultiplier);
            }
        }

        // Цикл для мобов Wizard
        foreach (GameObject prefab in wizardPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.UpdateStats(damageMultiplier, healthMultiplier, speedMultiplier);
            }
        }

        // Цикл для мобов Samurai
        foreach (GameObject prefab in samuraiPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.UpdateStats(damageMultiplier, healthMultiplier, speedMultiplier);
            }
        }

        // Цикл для мобов Archer
        foreach (GameObject prefab in archerPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.UpdateStats(damageMultiplier, healthMultiplier, speedMultiplier);
            }
        }

        // Цикл для мобов Boom
        foreach (GameObject prefab in boomPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.UpdateStats(damageMultiplier, healthMultiplier, speedMultiplier);
            }
        }

        // Цикл для мобов Healer
        foreach (GameObject prefab in healerPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.UpdateStats(damageMultiplier, healthMultiplier, speedMultiplier);
            }
        }

        // Цикл для мобов BuffMob
        foreach (GameObject prefab in buffMobPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.UpdateStats(damageMultiplier, healthMultiplier, speedMultiplier);
            }
        }

        foreach (GameObject prefab in ArchersArrow)
        {
            Arrow arrow = prefab.GetComponent<Arrow>();
            if (arrow != null)
            {
                arrow.UpdateStats(projectile);
            }
        }

        foreach (GameObject prefab in WitchsProjectile)
        {
            WitchsProjectile witchsProjectile = prefab.GetComponent<WitchsProjectile>();
            if (witchsProjectile != null)
            {
                witchsProjectile.UpdateStats(projectile);
            }
        }
    }

    private void ResetEnemyStats()
    {
        // Цикл для мобов Death
        foreach (GameObject prefab in deathMobPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RefreshStats();
            }
        }

        foreach (GameObject prefab in deathPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RefreshStats();
            }
        }

        // Цикл для мобов Bat
        foreach (GameObject prefab in batPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RefreshStats();
            }
        }

        // Цикл для мобов Wizard
        foreach (GameObject prefab in wizardPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RefreshStats();
            }
        }

        // Цикл для мобов Samurai
        foreach (GameObject prefab in samuraiPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RefreshStats();
            }
        }

        // Цикл для мобов Archer
        foreach (GameObject prefab in archerPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RefreshStats();
            }
        }

        // Цикл для мобов Boom
        foreach (GameObject prefab in boomPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RefreshStats();
            }
        }

        // Цикл для мобов Healer
        foreach (GameObject prefab in healerPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RefreshStats();
            }
        }

        // Цикл для мобов BuffMob
        foreach (GameObject prefab in buffMobPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RefreshStats();
            }
        }

        foreach (GameObject prefab in ArchersArrow)
        {
            Arrow arrow = prefab.GetComponent<Arrow>();
            if (arrow != null)
            {
                arrow.RefreshStats();
            }
        }

        foreach (GameObject prefab in WitchsProjectile)
        {
            WitchsProjectile witchsProjectile = prefab.GetComponent<WitchsProjectile>();
            if (witchsProjectile != null)
            {
                witchsProjectile.RefreshStats();
            }
        }
    }

    public void EndWave()
    {
        // Уничтожаем всех врагов с тегом "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }

        // Уничтожаем все снаряды с тегом "Projectile"
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (var projectile in projectiles)
        {
            Destroy(projectile);
        }

        // Останавливаем другие баффы для всех оружий
        foreach (var weapon in FindObjectsOfType<Weapon>())
        {
            weapon.CritChanceWave(); // Останавливаем другие баффы
        }

        foreach (var weapon in FindObjectsOfType<Weapon>())
        {
            weapon.CritDamageWave(); // Останавливаем другие баффы
        }

        playerHealth.MaxHpEndWave();
        MovePlayerToCenter();
        RemoveAllEXPObjects();
    }


    private void RemoveAllEXPObjects()
    {
        // Находим все объекты с тегом "EXP"
        GameObject[] expObjects = GameObject.FindGameObjectsWithTag("EXP");

        // Удаляем каждый найденный объект
        foreach (GameObject expObject in expObjects)
        {
            Destroy(expObject);
        }
    }

    private void InitializeWaves()
    {
        waveConfigs = new Dictionary<int, WaveConfig>();

        // Волна 1
        waveConfigs.Add(1, new WaveConfig(205f, new List<EnemySpawn>
    {
            new EnemySpawn (bossPrefabs[0], 1),
       // new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(55 + 21 * 1.5f)),

    }));

        // Волна 2
        waveConfigs.Add(2, new WaveConfig(30f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(60 + 22 * 1.5f)),

    }));

        // Волна 3
        waveConfigs.Add(3, new WaveConfig(35f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(120 + 23 * 1.5f)),
        new EnemySpawn(batPrefabs[0], 15),
        new EnemySpawn(samuraiPrefabs[0], 1)
    }));

        // Волна 4
        waveConfigs.Add(4, new WaveConfig(40f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(140 + 24 * 1.5f)),
        new EnemySpawn(batPrefabs[0], 20),
        new EnemySpawn(wizardPrefabs[0], 2),
        new EnemySpawn(samuraiPrefabs[0], 2)
    }));

        // Волна 5
        waveConfigs.Add(5, new WaveConfig(45f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(150 + 5 * 21.5f))
    }));

        // Волна 6
        waveConfigs.Add(6, new WaveConfig(50f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(260 + 26 * 1.5f)),
        new EnemySpawn(deathPrefabs[0], 3),
        new EnemySpawn(wizardPrefabs[0], 3),
        new EnemySpawn(samuraiPrefabs[0], 9)
    }));

        // Волна 7
        waveConfigs.Add(7, new WaveConfig(55f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(270 + 27 * 1.5f)),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(boomPrefabs[0], 5),
        new EnemySpawn(samuraiPrefabs[0], 10)
    }));

        // Волна 8
        waveConfigs.Add(8, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(280 + 28 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 10),
        new EnemySpawn(boomPrefabs[0], 5),
        new EnemySpawn(healerPrefabs[0], 2),
        new EnemySpawn(deathPrefabs[0], 3),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 5)
    }));

        // Волна 9
        waveConfigs.Add(9, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(350 + 29 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 4),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 10
        waveConfigs.Add(10, new WaveConfig(60f, new List<EnemySpawn>
    {
         new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(290 + 30 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 5),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 11
        waveConfigs.Add(11, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(300 + 31 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 10),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 12
        waveConfigs.Add(12, new WaveConfig(60f, new List<EnemySpawn>
    {
            new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(420 + 32 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 13
        waveConfigs.Add(13, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 33 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 14
        waveConfigs.Add(14, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 34 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 15
        waveConfigs.Add(15, new WaveConfig(60f, new List<EnemySpawn>
    {
       new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(430 + 35 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 16
        waveConfigs.Add(16, new WaveConfig(60f, new List<EnemySpawn>
    {
       new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 36 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 17
        waveConfigs.Add(17, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 37 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 18
        waveConfigs.Add(18, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(430 + 38 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 19
        waveConfigs.Add(19, new WaveConfig(60f, new List<EnemySpawn>
    {
       new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 39 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 20
        waveConfigs.Add(20, new WaveConfig(60f, new List<EnemySpawn>
    {
       new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 40 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 21
        waveConfigs.Add(21, new WaveConfig(60f, new List<EnemySpawn>
    {
       new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(430 + 41 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 22
        waveConfigs.Add(22, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 42 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 23
        waveConfigs.Add(23, new WaveConfig(60f, new List<EnemySpawn>
    {
      new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 43 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 24
        waveConfigs.Add(24, new WaveConfig(60f, new List<EnemySpawn>
    {
       new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(430 + 44 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 25
        waveConfigs.Add(25, new WaveConfig(60f, new List<EnemySpawn>
    {
      new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 45 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 26
        waveConfigs.Add(26, new WaveConfig(60f, new List<EnemySpawn>
    {
       new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 46 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 27
        waveConfigs.Add(27, new WaveConfig(60f, new List<EnemySpawn>
    {
     new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(430 + 47 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 28
        waveConfigs.Add(28, new WaveConfig(60f, new List<EnemySpawn>
    {
     new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 48 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 29
        waveConfigs.Add(29, new WaveConfig(60f, new List<EnemySpawn>
    {
       new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(330 + 49 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Волна 30
        waveConfigs.Add(30, new WaveConfig(60f, new List<EnemySpawn>
    {
       new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(430 + 50 * 1.5f)),
        new EnemySpawn(samuraiPrefabs[0], 15),
        new EnemySpawn(boomPrefabs[0], 6),
        new EnemySpawn(deathPrefabs[0], 15),
        new EnemySpawn(wizardPrefabs[0], 5),
        new EnemySpawn(archerPrefabs[0], 2),
        new EnemySpawn(buffMobPrefabs[0], 2)
    }));

        // Рассчитываем количество мобов для каждой волны
        foreach (var wave in waveConfigs)
        {
            int totalEnemies = 0;

            // Считаем общее количество мобов для текущей волны
            foreach (var spawn in wave.Value.enemiesToSpawn)
            {
                totalEnemies += spawn.count;
            }

            // Устанавливаем threshold убийств равным общему количеству мобов
            wave.Value.killThreshold = totalEnemies;  // Обновляем killThreshold для текущей волны
                                                      // Предположим, у вас есть метод для передачи этого значения в менеджер волн
                                                      // Обновляем количество мобов для текущей волны
            UpdateKillThreshold(wave.Value);
        }

    }

    public int GetWaveNumber()
    {
        return waveNumber;
    }

    public void AddEnemy(GameObject enemy)
    {
        if (enemy != null && !activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);

            // Подписываемся на событие OnDeath
            var enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.OnDeath += () =>
                {
                    killCountAll++;
                    killCount++; // Увеличиваем счетчик убийств при смерти врага
                    activeEnemies.Remove(enemy);
                };
            }
        }
    }



    public float GetTimeUntilNextWave()
    {
        if (spawningWave)
        {
            float timePassed = Time.time - timeStartedWave;
            float timeUntilNext = waveDuration - timePassed;

            if (timeUntilNext <= 0)
            {
                timeUntilNext = 0;
                RemoveRemainingEnemies();
            }

            return timeUntilNext;
        }

        return -1;
    }

    void RemoveRemainingEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
    }
}

public class WaveConfig
{
    public float waveDuration;
    public List<EnemySpawn> enemiesToSpawn;
    public float spawnInterval;

    // Публичное поле для изменения killThreshold
    public int killThreshold;

    public WaveConfig(float waveDuration, List<EnemySpawn> enemies)
    {
        this.waveDuration = waveDuration;
        this.enemiesToSpawn = enemies;
        this.spawnInterval = waveDuration / GetTotalEnemies();
        this.killThreshold = GetTotalEnemies(); // Стартовое значение, но можно изменить позже
    }

    private int GetTotalEnemies()
    {
        int total = 0;
        foreach (var enemySpawn in enemiesToSpawn)
        {
            total += enemySpawn.count;
        }
        return total;
    }
}


[System.Serializable]
public class EnemySpawn
{
    public GameObject enemyPrefab;  // Префаб врага
    public int count;               // Количество врагов
    [HideInInspector] public float spawnInterval; // Интервал спавна, вычисляется автоматически

    public EnemySpawn(GameObject prefab, int spawnCount)
    {
        enemyPrefab = prefab;
        count = spawnCount;
    }

    // Метод для установки интервала спавна на основе времени волны
    public void CalculateSpawnInterval(float spawnDuration)
    {
        if (count > 0)
            spawnInterval = spawnDuration / count;
        else
            spawnInterval = 0f;
    }
}