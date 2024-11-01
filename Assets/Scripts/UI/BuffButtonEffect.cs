using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;

    private void Start()
    {
        // ��������� �������� ������ ������
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ����������� ������ �� 10% ��� ���������
        transform.localScale = originalScale * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ��������������� �������� ������ ��� ������
        transform.localScale = originalScale;
    }
}
