using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Skill_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("---Slot Setting---")]
    [SerializeField] private int skill_Index;
    private Skill_Value_SO so;

    [Header("---UI---")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image borderImage;


    /// <summary>
    /// UI ����
    /// </summary>
    /// <param name="uiData"></param>
    /// <param name="skillLevel"></param>
    public void UI_Setting(Skill_Value_SO uiData, int skillLevel, int skillIndex)
    {
        // Index Setting
        skill_Index = skillIndex;
        so = uiData;

        // UI Setting
        iconImage.sprite = uiData.Icon;
        nameText.text = uiData.SkillName;
        levelText.text = $"{skillLevel} / 5";
    }

    /// <summary>
    /// ��ų ���� Ŭ��
    /// </summary>
    public void Use()
    {
        Player_Manager.instance.skill.LevelUp(skill_Index);
    }


    #region ���콺 �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        borderImage.color = Color.gray;
        UI_Manager.instance.Skill_Description(so);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Use();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        borderImage.color = Color.white;
    }
    #endregion
}
