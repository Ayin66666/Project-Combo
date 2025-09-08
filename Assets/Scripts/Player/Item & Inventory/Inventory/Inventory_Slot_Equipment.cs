using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;


public class Inventory_Slot_Equipment : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("---Setting---")]
    [SerializeField] private Item_Equipment item;
    public bool haveItem;


    [Header("---UI---")]
    [SerializeField] private Image icon;


    #region 프로퍼티
    public Item_Equipment Item { get { return item; } private set { item = value; } }
    #endregion


    /// <summary>
    /// 장비 장착 & 해제 로직
    /// </summary>
    /// <param name="equipment"></param>
    /// <param name="item"></param>
    public void Item_Setting(bool equipment, Item_Equipment item)
    {
        if (equipment)
        {
            // 장비 착용
            icon.gameObject.SetActive(true);
            icon.sprite = item.Icon;
            this.item = item;
            haveItem = true;

            // 스테이터스 증가
            Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);

            // 장비 효과 추가
            if (item.haveEffect)
                Player_Manager.instance.equipment.Add_ItemEffect(item.Effect);
        }
        else
        {
            // 장비 효과 제거
            if(item != null)
            {
                // 스테이터스 감소
                Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);

                // 이펙트 추가
                if (item.haveEffect)
                {
                    Player_Manager.instance.equipment.Remove_ItemEffect(item.Effect);
                }
            }

            // 장비 해제
            icon.gameObject.SetActive(false);
            icon.sprite = null;
            this.item = null;
            haveItem = false;
        }
    }


    #region 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 마우스 우클릭 시 - 아이템 해제
            if (haveItem)
            {
                // 클릭 사운드
                Player_Sound.instance.Sound_System(Player_Sound.SystemSound.Click);

                // 장비 해제
                Player_Manager.instance.equipment.EnEquipment(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (haveItem)
        {
            // 아이템 설명 UI On - 장비 아이템용 UI로 변경 필요
            UI_Manager.instance.ItemEquipment_DescriptionUI(true, item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (haveItem)
        {
            // 아이템 설명 UI Off - 장비 아이템용 UI로 변경 필요
            UI_Manager.instance.ItemEquipment_DescriptionUI(false, null);
        }
    }
    #endregion
}
