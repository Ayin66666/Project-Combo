using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Inventory_Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("---State---")]
    public bool haveItem;


    [Header("---Item Data---")]
    public Item_Base item;
    public int count;


    [Header("---UI---")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;


    [Header("---Drog UI---")]
    [SerializeField] private GameObject dragUI;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image dragUIIcon;


    #region ��� ����
    /// <summary>
    /// ������ �߰� ���� 1ȸ - ���� üũ ��� ��� ����? / �κ��丮�� �Ұǰ�?
    /// </summary>
    public void Slot_Setting(Item_Base data, int addCount)
    {
        // ���� ����
        Slot_Reset();

        // ������ �߰�
        haveItem = true;
        item = data;
        count = addCount;

        // UI ����
        icon.sprite = item.icon;
        dragUIIcon.sprite = item.icon;
        countText.text = count.ToString();
    }

    /// <summary>
    /// ���� ������ ���� ���
    /// </summary>
    public void Slot_Reset()
    {
        haveItem = false;
        item = null;
        count = 0;

        icon.sprite = null;
        dragUIIcon.sprite = null;
        countText.text = "";
    }

    /// <summary>
    /// �̹� �������� �ִ� ���Կ� ���� �߰� - ���� ���ϱ� ���� �ִ� ������ �Ѵ��� üũ�ϴ� �����? - �� �Լ� ȣ���ϴ� �κп���?
    /// </summary>
    /// <param name="count"></param>
    public void Slot_CountAdd(int addCount)
    {
        // ������ �߰�
        count += addCount;

        // ���� �ֽ�ȭ
        countText.text = count.ToString();
    }

    /// <summary>
    /// ������ ���?
    /// </summary>
    public void Slot_Use()
    {
        // Ÿ�� �� ������
        switch (item.itemType)
        {
            case Item_Base.ItemType.Equipment:
                break;

            case Item_Base.ItemType.Consumable:
                count--;
                if (count <= 0)
                {
                    haveItem = false;
                    item = null;
                    count = 0;
                }

                countText.text = count.ToString();
                break;

            case Item_Base.ItemType.Other:
                break;
        }

        item.Use();
    }
    #endregion


    #region Ŭ�� ��� - ��Ŭ�� = �̵� | ��Ŭ�� = ��� | �巡�׿� ������Ʈ �߰��� ��!
    public void OnPointerClick(PointerEventData eventData)
    {
        // ���콺 ������ Ŭ���� üũ
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            // �������� �ִٸ� ���� - ������ ����
            if (haveItem)
            {
                Slot_Use();
            }
        }
    }

    // �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        // �巡�� UI Ȱ��ȭ
        dragUI.SetActive(true);
    }

    // �巡�� ��
    public void OnDrag(PointerEventData eventData)
    {
        // �巡�� �� ��ġ �ֽ�ȭ
        rectTransform.anchoredPosition = eventData.position;
    }

    // �巡�� ����
    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� ��ġ üũ
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        GraphicRaycaster raycaster = Player_Manager.instance.inventory.canvas.GetComponent<GraphicRaycaster>();
        raycaster.Raycast(eventData, raycastResults);

        // ����� ���� üũ
        foreach(RaycastResult result in raycastResults)
        {
            // �Ʒ��� ������ ������ �ִٸ� - �ش� ��ġ�� ��ȯ
            Inventory_Slot slot = result.gameObject.GetComponent<Inventory_Slot>();
            if (slot != null)
            {
                Player_Manager.instance.inventory.Item_Change(this, slot);
                return;
            }

            // ��� ������ �ִٸ� - ��� �������� ��� ����
            Inventory_Slot_Equipment slotEq = result.gameObject.GetComponent<Inventory_Slot_Equipment>();
            if (slotEq != null)
            {
                return;
            }

            // �Ʒ��� ��Ʈ�� ������ �ִٸ� - �Һ� �������� ��� ����
            ShortCut_Slot slotSh = result.gameObject.GetComponent<ShortCut_Slot>();
            if (slotSh != null)
            {
                return;
            }
        }

        // �巡�� UI ��Ȱ��ȭ
        dragUI.SetActive(false);

        // �Ʒ��� ������ ������ ���ٸ� - ����ġ
        rectTransform.anchoredPosition = Vector2.zero;
    }
    #endregion
}
