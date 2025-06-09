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
    /// ������ �ε� �� ��ų ���� ����
    /// </summary>
    public void Skill_Setting()
    {
        // ��ų ������ ����
        Data data = SaveLoad_Manager.instance.LoadData(SaveLoad_Manager.instance.curSlot);
        for (int i = 0; i < attackData.Count; i++)
        {
            attackData[i].attack.skillLevel = data.skillData[i].skillLevel;
        }

        // ���� UI ����
        for (int i = 0;i < slots.Count; i++)
        {
            slots[i].UI_Setting(attackData[i].uiData, attackData[i].attack.skillLevel, i);
        }
    }

    /// <summary>
    /// ��ų ��� Ŭ�� �� ȣ��Ǵ� ��� - ��ų ���� ��
    /// </summary>
    /// <param name="skill_Index"></param>
    public void LevelUp(int skill_Index)
    {
        // ��ųǥ��Ʈ üũ
        if(skillPoint < 1)
        {
            // ��ų����Ʈ�� �����ϴٸ� - �ȳ� UI ǥ��
            return;
        }

        // ���� üũ
        if (attackData[skill_Index].attack.skillLevel < 5)
        {
            // �ִ뷹������ ���ٸ� - ���� ��
            attackData[skill_Index].attack.skillLevel++;
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
}
