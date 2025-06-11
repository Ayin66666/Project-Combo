using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SkillData
{
    public SkillType type;
    public string skillNmae;
    public int skillLevel;
    public enum SkillType { Normal, Smash, Other }
}


/*
0 1 2 3 = 일반
4 5 6 7 = 스매쉬
8 = 러쉬 슬래쉬
9 = 카운터
10 = 각성
11 = 각성기
*/

public class Skill_Manager : MonoBehaviour
{
    [Header("---Skill Tree Setting---")]
    public int skillPoint;
    public List<Skill> skillData;
    public List<Skill_Slot> slots;


    [System.Serializable]
    public struct Skill
    {
        public Skill_UI_SO uiData;
        public Attack_Base attack;
    }


    /// <summary>
    /// 저장용 레벨 데이터 반환
    /// </summary>
    /// <returns></returns>
    public List<int> GetSkillData()
    {
        // 레벨 데이터 추출
        List<int> data = new List<int>();
        for (int i = 0; i < skillData.Count; i++)
        {
            data.Add(skillData[i].attack.skillLevel);
        }

        return data;
    }

    /// <summary>
    /// 데이터 로드 및 스킬 레벨 설정
    /// </summary>
    public void Skill_Setting(Data data)
    {
        // 스킬 데이터 셋팅
        for (int i = 0; i < data.skillLevelData.Count; i++)
        {
            skillData[i].attack.skillLevel = data.skillLevelData[i];
        }

        // 슬롯 UI 셋팅
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].UI_Setting(skillData[i].uiData, skillData[i].attack.skillLevel, i);
        }
    }


    /// <summary>
    /// 스킬 노드 클릭 시 호출되는 기능 - 스킬 레벨 업
    /// </summary>
    /// <param name="skill_Index"></param>
    public void Skill_LevelUp(int skill_Index)
    {
        // 스킬표인트 체크
        if (skillPoint < 1)
        {
            // 스킬포인트가 부족하다면 - 안내 UI 표가
            return;
        }

        // 레벨 체크
        if (skillData[skill_Index].attack.skillLevel < 5)
        {
            // 최대레벨보다 낮다면 - 레벨 업
            skillData[skill_Index].attack.skillLevel++;
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

    /// <summary>
    /// 스킬포인트 추가 로직
    /// </summary>
    /// <param name="add"></param>
    public void Skill_PointAdd(int add)
    {
        // 포인트 증감
        skillPoint += add;

        // 포인트 UI 최신화
        UI_Manager.instance.Skill_Point(skillPoint);
    }
}
