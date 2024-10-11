using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // ������ �� ������
    public BoxCollider2D boundsCollider; // ������ �� BoxCollider2D, ������� ���������� �������
    public float smoothTime = 0.3f; // ����� ����������� �������� ������

    private Vector3 velocity = Vector3.zero; // �������� ������

    void LateUpdate()
    {
        if (player == null || boundsCollider == null) return; // ���� ������ ��� ���������� ���, �������

        // ���������� ������� ���� (������� ������)
        Vector3 targetPosition = player.position;
        targetPosition.z = transform.position.z; // ��������� ������� �������� �� ��� Z

      

        // ���������� �������� ������
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
