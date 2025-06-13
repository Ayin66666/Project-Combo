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


    #region 기능 동작
    /// <summary>
    /// 아이템 추가 최초 1회 - 여기 체크 기능 어떻게 할지? / 인벤토리가 할건가?
    /// </summary>
    public void Slot_Setting(Item_Base data, int addCount)
    {
        // 슬롯 리셋
        Slot_Reset();

        // 아이템 추가
        haveItem = true;
        item = data;
        count = addCount;

        // UI 셋팅
        icon.sprite = item.icon;
        dragUIIcon.sprite = item.icon;
        countText.text = count.ToString();
    }

    /// <summary>
    /// 슬롯 데이터 리셋 기능
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
    /// 이미 아이템이 있는 슬롯에 수량 추가 - 여기 더하기 전에 최대 수량을 넘는지 체크하는 기능은? - 이 함수 호출하는 부분에서?
    /// </summary>
    /// <param name="count"></param>
    public void Slot_CountAdd(int addCount)
    {
        // 아이템 추가
        count += addCount;

        // 수량 최신화
        countText.text = count.ToString();
    }

    /// <summary>
    /// 아이템 사용?
    /// </summary>
    public void Slot_Use()
    {
        // 타입 별 사용로직
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


    #region 클릭 기능 - 좌클릭 = 이동 | 우클릭 = 사용 | 드래그용 오브젝트 추가할 것!
    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 오른쪽 클릭만 체크
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            // 아이템이 있다면 동작 - 없으면 무시
            if (haveItem)
            {
                Slot_Use();
            }
        }
    }

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
    #endregion
}
