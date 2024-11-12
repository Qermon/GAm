using System.Collections.Generic;
using UnityEngine;

public class DamageSoundManager : MonoBehaviour
{
    public string damageSoundObjectName = "�����������"; // �������� ������� �� ������ ����� �� �����
    public int maxAudioSources = 5; // ������������ ���������� ������������ ������������� ������ �����

    private AudioClip damageSoundClip; // ��������� ����� �����
    private List<AudioSource> audioSourcesPool; // ��� ���������� ����� ��� ������������� ���������������

    private void Awake()
    {
        // ������� ������ �� ������ ����� �� �����
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
                Debug.LogError($"�� ������� '{damageSoundObjectName}' ����������� ��������� AudioSource!");
            }
        }
        else
        {
            Debug.LogError($"������ '{damageSoundObjectName}' �� ������ �� �����!");
        }

        // ������������� ���� ���������������
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
            Debug.LogWarning("���� ����� �� �����!");
            return;
        }

        // ����� ���������� �������������� � ����
        AudioSource audioSource = audioSourcesPool.Find(a => !a.isPlaying);
        if (audioSource == null)
        {
            // ���� ��� ��������� ������, ��������� ����� � ���
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = damageSoundClip;
            audioSourcesPool.Add(audioSource);
        }

        // ������������ �����
        audioSource.Play();
    }
}
