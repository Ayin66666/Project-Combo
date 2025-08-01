using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Inventory_Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler /*IBeginDragHandler, IDragHandler, IEndDragHandler*/
{
    [Header("---Slot Setting---")]
    public int slotIndex;


    [Header("---Item Data---")]
    public Item_Base item;
    public bool haveItem;
    public int itemCount;


    [Header("---Slot UI---")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;


    [Header("---Menu UI---")]
    [SerializeField] private GameObject menuSet;



    #region 기능 동작
    /// <summary>
    /// 슬롯의 인덱스 셋팅
    /// </summary>
    /// <param name="index"></param>
    public void SlotIndex_Setting(int index)
    {
        slotIndex = index;
    }

    /// <summary>
    /// 아이템 추가
    /// </summary>
    public void Slot_Setting(Item_Base data, int addCount)
    {
        Debug.Log($"Call slot item Setting{data} {addCount}");

        // 아이템 추가
        haveItem = true;
        item = data;
        itemCount = addCount;

        // UI 셋팅
        icon.sprite = item.Icon;
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
        countText.text = "";
    }

    /// <summary>
    /// 아이템 사용
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
                Use_Equipment();
                break;

            case Item_Base.Item_Type.Consumable:
                Use_Consumable();
                break;

            case Item_Base.Item_Type.Other:
                Use_Other();
                break;
        }

    }

    private void Use_Equipment()
    {
        // 장착 로직 호출
        Player_Manager.instance.equipment.Equipment(this, (Item_Equipment)item);

        // UI Off
        UI_Manager.instance.ItemEquipment_DescriptionUI(false, null);

        // 슬롯 초기화
        Slot_Reset();
    }

    private void Use_Consumable()
    {
        // 쿨타임 체크
        (bool isCooldown, float remainingTime) = Player_Manager.instance.cooldown.Cooldown_Check(((Item_Consumable)item).Key);
        if (isCooldown == false)
        {
            // 기능 동작
            item.Use();
            itemCount--;
            countText.text = itemCount.ToString();

            if (itemCount <= 0)
            {
                // UI Off & 슬롯 초기화
                UI_Manager.instance.Item_DescriptionUI(false, null);
                Slot_Reset();
            }
        }
        else
        {
            // 사용 불가 UI
            UI_Manager.instance.ItemCooldownUI(remainingTime);
        }
    }

    private void Use_Other()
    {
        // UI Off
        UI_Manager.instance.Item_DescriptionUI(false, null);

        // 아이템 설명?
        item.Use();
    }
    #endregion


    #region Item Meun
    /// <summary>
    /// 메뉴 UI 표기 
    /// </summary>
    public void Meun()
    {
        if (haveItem)
            menuSet.SetActive(true);
    }

    /// <summary>
    /// 아이템 사용
    /// </summary>
    public void Click_Use()
    {
        // 메뉴 UI 종료
        menuSet.SetActive(false);

        // 아이템 사용
        Slot_Use();
    }

    /// <summary>
    /// 쇼트컷 등록
    /// </summary>
    public void Click_Shortcut()
    {
        // 메뉴 UI 종료
        menuSet.SetActive(false);

        // 쇼트컷 등록 기능 호출 - 함수 제작 필요
        Player_Manager.instance.shortCut.Shortcut_Setting(this);
    }
    #endregion


    #region 클릭 기능 - 좌클릭 = 이동 | 우클릭 = 사용 | 드래그용 오브젝트 추가할 것!
    public void OnPointerClick(PointerEventData eventData)
    {
        // 아이템이 있다면
        if (!haveItem)
        {
            return;
        }

        // 클릭 사운드
        Player_Sound.instance.Sound_System(Player_Sound.SystemSound.Click);

        // 마우스 오른쪽 클릭만 체크
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item.itemType == Item_Base.Item_Type.Consumable)
            {
                // 소비 아이템이라면 메뉴 UI 표기
                Meun();
            }
            else
            {
                // 이외 아이템은 즉시 사용
                Slot_Use();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (haveItem)
        {
            if (item.itemType == Item_Base.Item_Type.Equipment)
            {
                UI_Manager.instance.ItemEquipment_DescriptionUI(true, (Item_Equipment)item);
            }
            else
            {
                if (!menuSet.activeSelf)
                    UI_Manager.instance.Item_DescriptionUI(true, item);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (haveItem)
        {
            if (item.itemType == Item_Base.Item_Type.Equipment)
            {
                UI_Manager.instance.ItemEquipment_DescriptionUI(false, null);
            }
            else
            {
                UI_Manager.instance.Item_DescriptionUI(false, null);
            }

        }
    }
    #endregion
}
