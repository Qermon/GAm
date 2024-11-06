using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventorySlot sourceSlot;  // �������� ����, �� �������� ��������������� �������
    public InventorySlot targetSlot;  // ������� ����, � ������� ��������������� �������

    private Image draggedItemIcon;  // ������ ���������������� ��������
    private Vector3 startPosition;  // �������� ������� �������
    private CanvasGroup canvasGroup;  // CanvasGroup ��� ���������� ���������� ����������

    private void Start()
    {
        // ������������� ����������
        draggedItemIcon = GetComponent<Image>();  // �������� ������ ��������
        canvasGroup = GetComponent<CanvasGroup>();  // �������� CanvasGroup ��� ���������� �������������
    }

    // ������ ��������������
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;  // ��������� �������� ������� �������
        canvasGroup.blocksRaycasts = false;  // ��������� �������������� � ���������
        draggedItemIcon.enabled = true;  // �������� ����������� ������

        // ������� �������� ����
        if (sourceSlot != null)
        {
            sourceSlot.ClearSlot();
        }
    }

    // ��������� ��������������
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;  // ���������� ������ �� ��������
    }

    // ����� ��������������
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;  // �������� �������������� � ���������

        // ���������, ����� �� �� ��������� ������� � ������� ����
        if (targetSlot != null && targetSlot.IsEmpty())
        {
            targetSlot.SetItem(sourceSlot.item);  // ���������� ������� � ������� ����
        }
        else
        {
            // ���� ���� �� ���� ��� ��� ����������� �����, ���������� ������� � �������� ����
            if (sourceSlot != null)
            {
                sourceSlot.SetItem(sourceSlot.item);  // ���������� ������� ������� � �������� ����
            }
        }

        draggedItemIcon.enabled = false;  // ��������� ����������� ������
        transform.position = startPosition;  // ���������� ������� �� ��� �������� �������
    }
}
