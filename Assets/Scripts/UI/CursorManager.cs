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
        Cursor.visible = false; // �������� ������
        Cursor.lockState = CursorLockMode.Locked; // ��������� ������ � ������ ������ (�����������)
    }
    

    public void ShowCursor()
    {
        Cursor.visible = true; // ���������� ������
        Cursor.lockState = CursorLockMode.None; // ������������ ������ (�����������)
    }
}
