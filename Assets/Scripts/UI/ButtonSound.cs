using UnityEngine;
using UnityEngine.UI; // для работы с UI элементами

public class ButtonClickSound : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Находим объект AudioSource, который будет воспроизводить звук
        audioSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();

        // Добавляем обработчик события нажатия для этой кнопки
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    // Функция, которая будет вызываться при нажатии на кнопку
    void PlaySound()
    {
        audioSource.Play();
    }
}
