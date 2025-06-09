using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SkillData
{
    public string skillNmae;
    public int skillLevel;
}


public class Skill_Manager : MonoBehaviour
{
    [Header("---Skill Tree Setting---")]
    public int skillPoint;
    public List<Skill> attackData;
    public List<Skill_Slot> slots;


    [System.Serializable]
    public struct Skill
    {
        public Skill_Value_SO uiData;
        public Attack_Base attack;
    }


    /// <summary>
    /// 데이터 로드 및 스킬 레벨 설정
    /// </summary>
    public void Skill_Setting()
    {
        // 스킬 데이터 셋팅
        Data data = SaveLoad_Manager.instance.LoadData(SaveLoad_Manager.instance.curSlot);
        for (int i = 0; i < attackData.Count; i++)
        {
            attackData[i].attack.skillLevel = data.skillData[i].skillLevel;
        }

        // 슬롯 UI 셋팅
        for (int i = 0;i < slots.Count; i++)
        {
            slots[i].UI_Setting(attackData[i].uiData, attackData[i].attack.skillLevel, i);
        }
    }

    /// <summary>
    /// 스킬 노드 클릭 시 호출되는 기능 - 스킬 레벨 업
    /// </summary>
    /// <param name="skill_Index"></param>
    public void LevelUp(int skill_Index)
    {
        // 스킬표인트 체크
        if(skillPoint < 1)
        {
            // 스킬포인트가 부족하다면 - 안내 UI 표가
            return;
        }

        // 레벨 체크
        if (attackData[skill_Index].attack.skillLevel < 5)
        {
            // 최대레벨보다 낮다면 - 레벨 업
            attackData[skill_Index].attack.skillLevel++;
            skillPoint--;

            // 습득 성공 UI
            UI_Manager.instance.Skill_Result(true);
        }
        else
        {
            // 최대레벨이라면 - 안내 UI 표기
            UI_Manager.instance.Skill_Result(false);
        }
    }
}
