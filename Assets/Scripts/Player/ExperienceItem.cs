using UnityEngine;

public class ExperienceItem : MonoBehaviour
{
    public int experienceAmount = 20; // ���������� �����, ������� ������� �����

    void OnTriggerEnter2D(Collider2D other)
    {
        // ���������, ���� ����� ���������� � ���������
        PlayerLevelUp player = other.GetComponent<PlayerLevelUp>();
        if (player != null)
        {
            // ����������� ���� ������
            player.GainExperience(experienceAmount);

            // ���������� �������
            Destroy(gameObject);
        }
    }
}
