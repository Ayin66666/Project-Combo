using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Hideout_StageSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("---Slot UI---")]
    [SerializeField] private Image borderImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Color highlightColor;
    [SerializeField] private int stageIndex;
    public bool canSelect;

    public void SlotUI_Setting(Chapter_Data_SO data, int index, bool canSelect)
    {
        this.canSelect = canSelect;
        stageIndex = index;
        nameText.text = data.stageData[index].stageType + " " + data.stageData[index].stageName;
    }


    #region 마우스 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 오버
        borderImage.color = Color.gray;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 제거
        borderImage.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 진입 가능 여부 체크
        if(canSelect)
        {
            // 진입 가능 시 - 설명 UI
            Hideout_Manager.instance.DescriptionUI_Setting(stageIndex);
        }
        else
        {
            // 진입 불가능 시 - 안내 UI
            Hideout_Manager.instance.EnterUI();
        }
    }
    #endregion
}
