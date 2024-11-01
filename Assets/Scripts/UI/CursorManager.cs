using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
       
    }

    public void HideCursor()
    {
        Cursor.visible = false; // Скрываем курсор
        Cursor.lockState = CursorLockMode.Locked; // Блокируем курсор в центре экрана (опционально)
    }
    

    public void ShowCursor()
    {
        Cursor.visible = true; // Показываем курсор
        Cursor.lockState = CursorLockMode.None; // Разблокируем курсор (опционально)
    }
}
