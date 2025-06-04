using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack_Base : MonoBehaviour
{
    [Header("---Status---")]
    public string skillName;
    public Owner attackOwner;
    public SkillType skillType;
    public ArmorType armorType;
    public CancelType cancelType;
    public bool haveAwakningValue;
    public int skillLevel;
    public List<Value> value_Normal;
    public List<Value> value_Awakening;
    [SerializeField] protected string[] nextAttackData;
    [SerializeField] protected float time;

    protected Coroutine useCoroutine;
    protected Coroutine movementDelayCoroutine;

    
    [System.Serializable]
    public struct Value
    {
        [SerializeField] private string name;
        public Skill_Value_SO levelValue; // ��ų���� �� ������
        public AttackCollider_Controller attackCollider;
    }
    
    public enum SkillType { None, Attack, Buff, Other }
    public enum ArmorType { None, Knockback, Super }
    public enum CancelType { Free, Lock }
    public enum Owner { Player, Enemy }


    [Header("---Movement Setting---")]
    public MoveData[] moveDatas;

    [System.Serializable]
    public struct MoveData
    {
        public string name;
        public Transform movePos;
        public float moveSpeed;
    }


    [Header("--- Component ---")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected Enemy_Base enemy;


    /// <summary>
    /// ���� ��� �� ȣ��
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// ���� ����Ʈ
    /// </summary>
    /// <param name="index">����Ʈ �ε���</param>
    public abstract void AttackVFX(int index);

    /// <summary>
    /// ���� �� �̵� ���
    /// </summary>
    public void Attack_Movement(int index)
    {
        switch (attackOwner)
        {
            case Owner.Player:
                PlayerAction_Manager.instance.Attack_Movement(moveDatas[index].movePos, moveDatas[index].moveSpeed);
                break;

            case Owner.Enemy:
                enemy.Attack_Movement(moveDatas[index].movePos, moveDatas[index].moveSpeed);
                break;
        }
    }

    /// <summary>
    /// ��¡ ���ݵ� �ְ� ���� ���¿� ���� Ÿ�� ����/������ ���浵 �־ �������� �̵�
    /// ������ ��� �� �ݶ��̴��� ������ �Է�
    /// </summary>
    /// <param name="index">������ �ε���</param>
    public abstract void DamageCal(int index);

    /// <summary>
    /// ���� ���� �� ���� �ݶ��̴� ����
    /// </summary>
    public void Attack_ColliderReset()
    {
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (value_Normal[i].attackCollider != null)
                value_Normal[i].attackCollider.ListReset();
        }

        for (int i = 0; i < value_Awakening.Count; i++)
        {
            if (value_Awakening[i].attackCollider != null)
                value_Awakening[i].attackCollider.ListReset();
        }
    }

    /// <summary>
    /// �������̴� �ൿ ���� ���� -> abstract�� �����ϴ� �͵�?
    /// </summary>
    public abstract void Attack_Reset();
}
