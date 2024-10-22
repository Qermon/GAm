using UnityEngine;
using TMPro; // ������ ��� ������ � ������� � TextMeshPro

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro textMesh; // ������ �� ��������� ������
    public float disappearTime = 1f; // ����� �� ������������ ������
    public float floatSpeed = 2f; // ��������, � ������� ����� �����������

    private Color textColor; // ���� ������
    private float disappearTimer; // ������ ��� ������������
    private Vector3 moveVector; // ������ ��� �������� ������

    public void Setup(int damageAmount)
    {
        textMesh.text = damageAmount.ToString(); // ������������� ����� �����
        textColor = textMesh.color; // ��������� ��������� ���� ������
        disappearTimer = disappearTime; // ������������� ������ ������������
        moveVector = new Vector3(0, floatSpeed, 0); // ���������� ������ �������� ������ �����
    }

    private void Update()
    {
        // ��������� ����� ��� ������
        transform.position += moveVector * Time.deltaTime;

        // ��������� ������������ ������ �� ���� ������������
        disappearTimer -= Time.deltaTime;
        if (disappearTimer <= 0)
        {
            // ������� ������������ ������
            textColor.a -= Time.deltaTime / disappearTime;
            textMesh.color = textColor;
            if (textColor.a <= 0)
            {
                Destroy(gameObject); // ������� �����, ����� �� ��������� ��������
            }
        }
    }


}
