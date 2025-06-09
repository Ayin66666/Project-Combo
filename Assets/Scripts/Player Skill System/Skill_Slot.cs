using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Skill_Slot : MonoBehaviour
{
    [Header("---Slot Setting---")]
    [SerializeField] private int skill_Index;


    [Header("---UI---")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;


    /// <summary>
    /// UI 셋팅
    /// </summary>
    /// <param name="uiData"></param>
    /// <param name="skillLevel"></param>
    public void UI_Setting(Skill_Value_SO uiData, int skillLevel, int skillIndex)
    {
        // Index Setting
        skill_Index = skillIndex;

        // UI Setting
        iconImage.sprite = uiData.Icon;
        nameText.text = uiData.SkillName;
        levelText.text = $"{skillLevel} / 5";
    }

    /// <summary>
    /// 스킬 슬롯 클릭
    /// </summary>
    public void Use()
    {
        Player_Manager.instance.skill.LevelUp(skill_Index);
    }
}
