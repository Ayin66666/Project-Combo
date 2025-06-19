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


    #region 기능 동작
    /// <summary>
    /// 아이템 추가
    /// </summary>
    public void Slot_Setting(Item_Base data, int addCount)
    {
        Debug.Log("call slot itemSetting / " + gameObject + " / " + addCount);
        // 아이템 추가
        haveItem = true;
        item = data;
        itemCount = addCount;

        // UI 셋팅
        icon.sprite = item.Icon;
        dragUIIcon.sprite = item.Icon;
        countText.text = itemCount.ToString();
    }

    /// <summary>
    /// 슬롯 데이터 리셋 기능
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
    /// 아이템 사용?
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


    #region 클릭 기능 - 좌클릭 = 이동 | 우클릭 = 사용 | 드래그용 오브젝트 추가할 것!
    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 오른쪽 클릭만 체크
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 아이템이 있다면 동작 - 없으면 무시
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

    // 드래그 기능 - 일단 비활성화
    /*
    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 UI 활성화
        dragUI.SetActive(true);
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 간 위치 최신화
        rectTransform.anchoredPosition = eventData.position;
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 시 위치 체크
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        GraphicRaycaster raycaster = Player_Manager.instance.inventory.canvas.GetComponent<GraphicRaycaster>();
        raycaster.Raycast(eventData, raycastResults);

        // 검출된 슬롯 체크
        foreach(RaycastResult result in raycastResults)
        {
            // 아래에 아이템 슬롯이 있다면 - 해당 위치와 교환
            Inventory_Slot slot = result.gameObject.GetComponent<Inventory_Slot>();
            if (slot != null)
            {
                Player_Manager.instance.inventory.Item_Change(this, slot);
                return;
            }

            // 장비 슬롯이 있다면 - 장비 아이템의 경우 장착
            Inventory_Slot_Equipment slotEq = result.gameObject.GetComponent<Inventory_Slot_Equipment>();
            if (slotEq != null)
            {
                return;
            }

            // 아래에 쇼트컷 슬롯이 있다면 - 소비 아이템의 경우 장착
            ShortCut_Slot slotSh = result.gameObject.GetComponent<ShortCut_Slot>();
            if (slotSh != null)
            {
                return;
            }
        }

        // 드래그 UI 비활성화
        dragUI.SetActive(false);

        // 아래에 아이템 슬롯이 없다면 - 원위치
        rectTransform.anchoredPosition = Vector2.zero;
    }
    */


    #endregion
}
