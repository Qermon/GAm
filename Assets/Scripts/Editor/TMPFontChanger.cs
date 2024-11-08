using UnityEditor;
using UnityEngine;
using TMPro;

public class TMPFontChanger : EditorWindow
{
    public TMP_FontAsset newFontAsset;

    [MenuItem("Tools/Change TMP Font")]
    public static void ShowWindow()
    {
        GetWindow<TMPFontChanger>("TMP Font Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Change TMP Font in Project", EditorStyles.boldLabel);

        newFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font Asset", newFontAsset, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Change Font"))
        {
            if (newFontAsset != null)
            {
                ChangeFonts(newFontAsset);
            }
            else
            {
                Debug.LogWarning("Please select a TMP font asset first.");
            }
        }
    }

    private static void ChangeFonts(TMP_FontAsset newFont)
    {
        // Находим все компоненты TMP_Text в сцене
        TMP_Text[] allTMPTextComponents = FindObjectsOfType<TMP_Text>();

        // Меняем шрифт на новый для каждого компонента
        foreach (TMP_Text textComponent in allTMPTextComponents)
        {
            textComponent.font = newFont;
        }

        Debug.Log("Fonts changed on all TMP_Text components.");
    }
}
