using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Skill_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("---Slot Setting---")]
    [SerializeField] private SkillData data;
    [SerializeField] private int skill_Index;

    [Header("---UI---")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image borderImage;


    #region UI 셋팅
    /// <summary>
    /// UI 셋팅
    /// </summary>
    /// <param name="uiData"></param>
    /// <param name="skillLevel"></param>
    public void UI_Setting(SkillData skillData, int skill_Index)
    {
        data = skillData;
        this.skill_Index = skill_Index;

        // UI Setting
        iconImage.sprite = data.ui.Icon;
        nameText.text = data.ui.SkillName;
        levelText.text = $"{data.attack.skillLevel + 1} / 5";
    }

    /// <summary>
    /// 레벨업 시 UI 최신화
    /// </summary>
    public void LevelUp()
    {
        levelText.text = $"{data.attack.skillLevel + 1} / 5";
    }
    #endregion


    #region 마우스 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        borderImage.color = Color.gray;
        UI_Manager.instance.Skill_Description(data.ui, data.attack.skillLevel);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 스킬 슬롯 클릭
        Player_Manager.instance.skill.Skill_LevelUp(skill_Index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        borderImage.color = Color.white;
    }
    #endregion
}
