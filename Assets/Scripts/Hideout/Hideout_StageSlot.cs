using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Hideout_StageSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("---Slot UI---")]
    [SerializeField] private Image borderImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private int stageIndex;


    public void SlotUI_Setting(Chapter_Data_SO data, int index)
    {
        stageIndex = data.stageData.Count;
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
        // 설명 UI On
        Hideout_Manager.instance.DescriptionUI_Setting(stageIndex);
    }
    #endregion
}
