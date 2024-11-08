using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 offset; // �������� ������ ������������ ������
    private Transform target; // ������ �� ������

    public float smoothSpeed = 0.125f; // �������� �������� �������� ������

    // ������� �������� ������ �� ���� X � Y
    public float minX = 13f;
    public float maxX = 23.5f;
    public float minY = -19.1f;
    public float maxY = -4.5f;

    void Update()
    {
        // �������� ������� ������
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                return; // ���� ����� ��� �� ��������, ������� �� ������
            }
        }

        // ������������ �������� ������� ������
        Vector3 desiredPosition = target.position + offset;

        // ������������ ������� ������ �� �����
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

        // ������ ������� ������ � �������� ������� � ������ �����������
        transform.position = Vector3.Lerp(transform.position, new Vector3(clampedX, clampedY, transform.position.z), smoothSpeed);
    }
}
