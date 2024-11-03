using UnityEngine;

public class ExperienceItem : MonoBehaviour
{
    public int experienceAmount = 3; // ���������� �����, ������� ������� �����
    public int goldAmount = 1;
    public float baseMoveSpeed = 1f; // ��������� ��������
    public float maxMoveSpeed = 5f; // ������������ ��������
    public float acceleration = 0.5f; // ���������
    private float currentSpeed; // ������� ��������
    private bool isAttracted = false; // ����, ������������� �� ���� � ������
    private Transform player; // ������ �� ������

    void Start()
    {
        currentSpeed = baseMoveSpeed; // ������������� ��������� ��������
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� ����� � ������ ������, �������� �����������
            isAttracted = true;
            player = other.transform;
        }
    }

    void Update()
    {
        // ���� ������� ����� ������������� � ������, ������� ��� � ������
        if (isAttracted && player != null)
        {
            // ����������� ��������
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxMoveSpeed);

            // ������ ������� ���� � ������
            transform.position = Vector3.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);

            // ���������, ������ �� ���� ������
            if (Vector3.Distance(transform.position, player.position) < 0.1f)
            {
                // ��������� ������ ������
                PlayerGold playerGold = player.GetComponent<PlayerGold>();
                if (playerGold != null)
                {
                    playerGold.AddGold(goldAmount);
                }

                PlayerLevelUp playerScript = player.GetComponent<PlayerLevelUp>();
                if (playerScript != null)
                {
                    playerScript.GainExperience(experienceAmount);
                    Destroy(gameObject); // ���������� ������� ����� ����, ��� ���� ������
                }
            }
        }
    }
}
