using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public TMP_Text fpsText; // —сылка на UI-элемент дл€ отображени€ FPS
    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int fps = Mathf.CeilToInt(1.0f / deltaTime);
        fpsText.text = "FPS: " + fps.ToString();
    }

}
