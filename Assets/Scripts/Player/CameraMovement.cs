using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 offset; // Смещение камеры относительно игрока
    private Transform target; // Ссылка на игрока

    public float smoothSpeed = 0.125f; // Скорость плавного движения камеры

    // Границы движения камеры по осям X и Y
    public float minX = 13f;
    public float maxX = 23.5f;
    public float minY = -19.1f;
    public float maxY = -4.5f;

    void Update()
    {
        // Проверка наличия игрока
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                return; // Если игрок ещё не появился, выходим из метода
            }
        }

        // Рассчитываем желаемую позицию камеры
        Vector3 desiredPosition = target.position + offset;

        // Ограничиваем позицию камеры по краям
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

        // Плавно двигаем камеру к желаемой позиции с учетом ограничений
        transform.position = Vector3.Lerp(transform.position, new Vector3(clampedX, clampedY, transform.position.z), smoothSpeed);
    }
}
