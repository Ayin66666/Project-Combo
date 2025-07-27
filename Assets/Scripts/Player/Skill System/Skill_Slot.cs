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


    #region UI ����
    /// <summary>
    /// UI ����
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
        int level = Mathf.Min(data.attack.skillLevel + 1, data.attack.maxLevel);
        levelText.text = $"{level} / {data.attack.maxLevel}";
    }

    /// <summary>
    /// ������ �� UI �ֽ�ȭ
    /// </summary>
    public void LevelUp()
    {
        int level = Mathf.Min(data.attack.skillLevel + 1, data.attack.maxLevel);
        levelText.text = $"{level} / {data.attack.maxLevel}";
    }
    #endregion


    #region ���콺 �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        borderImage.color = Color.gray;
        UI_Manager.instance.Skill_Description(data.ui, data.attack.skillLevel);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ��ų ���� Ŭ��
        Player_Manager.instance.skill.Skill_LevelUp(skill_Index, data.attack.maxLevel);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        borderImage.color = Color.white;
    }
    #endregion
}
