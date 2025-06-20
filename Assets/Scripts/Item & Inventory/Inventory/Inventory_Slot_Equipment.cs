using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory_Slot_Equipment : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("---Setting---")]
    [SerializeField] private Item_Equipment item;
    public bool haveItem;


    public void Equipment(Item_Equipment item)
    {
        // 플레이어 매니저에 스텟 증감 - 이거 따로 함수 뽑아둘건지?
        Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
    }

    public void UnEquipment()
    {
        // 플레이어 매니저에 스텟 증강 - 이거 따로 함수 뽑아둘건지?
    }


    #region 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 마우스 우클릭 시 - 아이템 해제
            UnEquipment();
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
    #endregion
}
