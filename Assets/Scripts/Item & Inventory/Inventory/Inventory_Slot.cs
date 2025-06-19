using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Inventory_Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler , IPointerExitHandler /*IBeginDragHandler, IDragHandler, IEndDragHandler*/
{
    [Header("---Item Data---")]
    public Item_Base item;
    public bool haveItem;
    public int itemCount;


    [Header("---UI---")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;


    [Header("---Drog UI---")]
    [SerializeField] private GameObject dragUI;
    [SerializeField] private Image dragUIIcon;


    #region ��� ����
    /// <summary>
    /// ������ �߰�
    /// </summary>
    public void Slot_Setting(Item_Base data, int addCount)
    {
        Debug.Log("call slot itemSetting / " + gameObject + " / " + addCount);
        // ������ �߰�
        haveItem = true;
        item = data;
        itemCount = addCount;

        // UI ����
        icon.sprite = item.Icon;
        dragUIIcon.sprite = item.Icon;
        countText.text = itemCount.ToString();
    }

    /// <summary>
    /// ���� ������ ���� ���
    /// </summary>
    public void Slot_Reset()
    {
        haveItem = false;
        item = null;
        itemCount = 0;

        icon.sprite = null;
        dragUIIcon.sprite = null;
        countText.text = "";
    }

    /// <summary>
    /// ������ ���?
    /// </summary>
    public void Slot_Use()
    {
        if (!haveItem)
        {
            return;
        }

        switch (item.itemType)
        {
            case Item_Base.Item_Type.Equipment:
                break;

            case Item_Base.Item_Type.Consumable:
                item.Use();
                itemCount--;
                countText.text = itemCount.ToString();

                if (itemCount <= 0)
                {
                    Slot_Reset();
                }
                break;

            case Item_Base.Item_Type.Other:
                break;
        }

    }
    #endregion


    #region Ŭ�� ��� - ��Ŭ�� = �̵� | ��Ŭ�� = ��� | �巡�׿� ������Ʈ �߰��� ��!
    public void OnPointerClick(PointerEventData eventData)
    {
        // ���콺 ������ Ŭ���� üũ
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // �������� �ִٸ� ���� - ������ ����
            if (haveItem)
            {
                Debug.Log("UseCall");
                Slot_Use();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (haveItem)
        {
            UI_Manager.instance.Item_DescriptionUI(true, item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (haveItem)
        {
            UI_Manager.instance.Item_DescriptionUI(false, null);
        }
    }

    // �巡�� ��� - �ϴ� ��Ȱ��ȭ
    /*
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
    */


    #endregion
}
