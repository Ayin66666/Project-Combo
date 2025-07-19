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


    #region ���콺 �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ���콺 ����
        borderImage.color = Color.gray;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ���콺 ����
        borderImage.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ���� ���� ���� üũ
        if(canSelect)
        {
            // ���� ���� �� - ���� UI
            Hideout_Manager.instance.DescriptionUI_Setting(stageIndex);
        }
        else
        {
            // ���� �Ұ��� �� - �ȳ� UI
            Hideout_Manager.instance.EnterUI();
        }
    }
    #endregion
}
