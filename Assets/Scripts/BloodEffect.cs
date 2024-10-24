using System.Collections;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    public GameObject[] bloodTextures; // ������ �������� ������� �����

    // ����� ��� ������ �����
    public void SpawnBlood(Vector3 position)
    {
        // ����� ���������� ������� ����� �� �������
        int randomIndex = Random.Range(0, bloodTextures.Length);
        GameObject bloodEffectInstance = Instantiate(bloodTextures[randomIndex], position, Quaternion.identity);

        // ��������� �������� ��� �������� �����
        StartCoroutine(RemoveBloodEffect(bloodEffectInstance));
    }

    // ��������� ��� �������� ������� �����
    private IEnumerator RemoveBloodEffect(GameObject bloodEffect)
    {
        // ���� 4 �������
        yield return new WaitForSeconds(4f);

        // ������� ������������
        SpriteRenderer spriteRenderer = bloodEffect.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            float fadeDuration = 3f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                spriteRenderer.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // ������� ������ �� �����
        Destroy(bloodEffect);
    }
}
