using System.Collections.Generic;
using UnityEngine;

public class DamageSoundManager : MonoBehaviour
{
    public string damageSoundObjectName = "УронПоМобам"; // Название объекта со звуком урона на сцене
    public int maxAudioSources = 5; // Максимальное количество одновременно проигрываемых звуков урона

    private AudioClip damageSoundClip; // Аудиоклип звука урона
    private List<AudioSource> audioSourcesPool; // Пул источников звука для параллельного воспроизведения

    private void Awake()
    {
        // Находим объект со звуком урона на сцене
        GameObject damageSoundObject = GameObject.Find(damageSoundObjectName);
        if (damageSoundObject != null)
        {
            AudioSource source = damageSoundObject.GetComponent<AudioSource>();
            if (source != null)
            {
                damageSoundClip = source.clip;
            }
            else
            {
                Debug.LogError($"На объекте '{damageSoundObjectName}' отсутствует компонент AudioSource!");
            }
        }
        else
        {
            Debug.LogError($"Объект '{damageSoundObjectName}' не найден на сцене!");
        }

        // Инициализация пула аудиоисточников
        audioSourcesPool = new List<AudioSource>();
        for (int i = 0; i < maxAudioSources; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = damageSoundClip;
            audioSource.playOnAwake = false;
            audioSourcesPool.Add(audioSource);
        }
    }

    public void PlayDamageSound()
    {
        if (damageSoundClip == null)
        {
            Debug.LogWarning("Звук урона не задан!");
            return;
        }

        // Поиск свободного аудиоисточника в пуле
        AudioSource audioSource = audioSourcesPool.Find(a => !a.isPlaying);
        if (audioSource == null)
        {
            // Если все источники заняты, добавляем новый в пул
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = damageSoundClip;
            audioSourcesPool.Add(audioSource);
        }

        // Проигрывание звука
        audioSource.Play();
    }
}
