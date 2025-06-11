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
0 1 2 3 = �Ϲ�
4 5 6 7 = ���Ž�
8 = ���� ������
9 = ī����
10 = ����
11 = ������
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
    /// ����� ���� ������ ��ȯ
    /// </summary>
    /// <returns></returns>
    public List<int> GetSkillData()
    {
        // ���� ������ ����
        List<int> data = new List<int>();
        for (int i = 0; i < skillData.Count; i++)
        {
            data.Add(skillData[i].attack.skillLevel);
        }

        return data;
    }

    /// <summary>
    /// ������ �ε� �� ��ų ���� ����
    /// </summary>
    public void Skill_Setting(Data data)
    {
        // ��ų ������ ����
        for (int i = 0; i < data.skillLevelData.Count; i++)
        {
            skillData[i].attack.skillLevel = data.skillLevelData[i];
        }

        // ���� UI ����
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].UI_Setting(skillData[i].uiData, skillData[i].attack.skillLevel, i);
        }
    }


    /// <summary>
    /// ��ų ��� Ŭ�� �� ȣ��Ǵ� ��� - ��ų ���� ��
    /// </summary>
    /// <param name="skill_Index"></param>
    public void Skill_LevelUp(int skill_Index)
    {
        // ��ųǥ��Ʈ üũ
        if (skillPoint < 1)
        {
            // ��ų����Ʈ�� �����ϴٸ� - �ȳ� UI ǥ��
            return;
        }

        // ���� üũ
        if (skillData[skill_Index].attack.skillLevel < 5)
        {
            // �ִ뷹������ ���ٸ� - ���� ��
            skillData[skill_Index].attack.skillLevel++;
            skillPoint--;

            // ���� ���� UI
            UI_Manager.instance.Skill_Result(true);
        }
        else
        {
            // �ִ뷹���̶�� - �ȳ� UI ǥ��
            UI_Manager.instance.Skill_Result(false);
        }
    }

    /// <summary>
    /// ��ų����Ʈ �߰� ����
    /// </summary>
    /// <param name="add"></param>
    public void Skill_PointAdd(int add)
    {
        // ����Ʈ ����
        skillPoint += add;

        // ����Ʈ UI �ֽ�ȭ
        UI_Manager.instance.Skill_Point(skillPoint);
    }
}
