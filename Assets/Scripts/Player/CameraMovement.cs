using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 offset;
    private Transform target;

    void Start()
    {
        
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("����� � ����� 'Player' �� ������ �� �����.");
        }

        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
