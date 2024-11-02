using UnityEngine;
using TMPro;

public class DamageTextController : MonoBehaviour
{
    public float moveUpSpeed = 1.0f;        // �������� ����������� ������ �����
    public float duration = 1.0f;           // �����, ����� ������� ����� ��������

    private TextMeshProUGUI damageText;
    private float timer;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>(); // �������� ���������
    }

    public void SetDamage(int damage)
    {
        if (damageText != null)
        {
            damageText.text = damage.ToString(); // ��������� ������ �����
        }
        else
        {
            Debug.LogError("Damage text component is null!");
        }
    }

    private void Update()
    {
        // ��������� ����� �����
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime);

        // ��������� ������ ��� �������� ������ ����� duration
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
