using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Hideout_StageSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("---Slot UI---")]
    [SerializeField] private Image borderImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;


    [Header("---Description UI---")]
    [SerializeField] private GameObject descriptionSet;
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private TextMeshProUGUI stageTypeText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image stageImage;


    public void UI_Setting(Chapter_Data_SO data, int index)
    {
        nameText.text = data.stageData[index].stageName;
        typeText.text = data.stageType == Chapter_Data_SO.StageType.Normal ? "Normal" : "Boss";
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
        descriptionSet.SetActive(true);
    }
    #endregion
}
