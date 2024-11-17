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
        // ������������� SDK
        MobileAds.Initialize(initStatus => { });

        // �������� ������������� �������
        LoadInterstitialAd();
    }

    private void LoadInterstitialAd()
    {
        string adUnitId = "ca-app-pub-3940256099942544/1033173712"; // �������� �� ��� �������� Ad Unit ID ��� �������� ID

        // ������� ������ �� �������� �������
        AdRequest adRequest = new AdRequest();

        // ��������� ������������� �������
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

                // ������������� �� ������� �������� �������
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
            RestartScene(); // ���� ������� �� �����������, ����� ������������� �����
        }
    }

    private void HandleAdOpened()
    {
        Debug.Log("Ad opened. Pausing music.");
        if (gameMusic != null)
        {
            gameMusic.Pause(); // ������������ ������
        }
    }

    private void HandleAdClosed()
    {
        Debug.Log("Interstitial ad closed.");
        RestartScene(); // ���������� ����� ����� �������� �������

        // ��������� ����� ���������� ����� ������
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
