using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Ссылка на игрока
    public BoxCollider2D boundsCollider; // Ссылка на BoxCollider2D, который определяет границы
    public float smoothTime = 0.3f; // Время сглаживания движения камеры

    private Vector3 velocity = Vector3.zero; // Скорость камеры

    void LateUpdate()
    {
        if (player == null || boundsCollider == null) return; // Если игрока или коллайдера нет, выходим

        // Определяем позицию цели (позицию игрока)
        Vector3 targetPosition = player.position;
        targetPosition.z = transform.position.z; // Сохраняем текущее значение по оси Z

      

        // Сглаживаем движение камеры
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
