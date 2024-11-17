using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdManager : MonoBehaviour
{
    private InterstitialAd interstitialAd;
    public GameManager gameManager;
    public AudioSource gameMusic;

    void Start()
    {
        // Инициализация SDK
        MobileAds.Initialize(initStatus => { });

        // Загрузка межстраничной рекламы
        LoadInterstitialAd();
    }

    private void LoadInterstitialAd()
    {
        string adUnitId = "ca-app-pub-3940256099942544/1033173712"; // Замените на ваш реальный Ad Unit ID или тестовый ID

        // Создаем запрос на загрузку рекламы
        AdRequest adRequest = new AdRequest();

        // Загружаем межстраничную рекламу
        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"Failed to load interstitial ad: {error}");
                    return;
                }

                Debug.Log("Interstitial ad loaded successfully.");
                interstitialAd = ad;

                // Подписываемся на событие закрытия рекламы
                interstitialAd.OnAdFullScreenContentOpened += HandleAdOpened;
                interstitialAd.OnAdFullScreenContentClosed += HandleAdClosed;

            });
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial ad is not ready yet. Restarting scene.");
            RestartScene(); // Если реклама не загрузилась, сразу перезапускаем сцену
        }
    }

    private void HandleAdOpened()
    {
        Debug.Log("Ad opened. Pausing music.");
        if (gameMusic != null)
        {
            gameMusic.Pause(); // Приостановка музыки
        }
    }

    private void HandleAdClosed()
    {
        Debug.Log("Interstitial ad closed.");
        RestartScene(); // Перезапуск сцены после закрытия рекламы

        // Загружаем новое объявление после показа
        LoadInterstitialAd();
    }

    private void RestartScene()
    {
        gameManager.RestartGame();
    }

    private void OnDestroy()
    {
        if (interstitialAd != null)
        {
            interstitialAd.OnAdFullScreenContentClosed -= HandleAdClosed;
            interstitialAd.OnAdFullScreenContentOpened -= HandleAdOpened;
        }
    }
}
