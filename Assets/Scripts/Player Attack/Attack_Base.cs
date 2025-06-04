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
        public Skill_Value_SO levelValue; // 스킬레벨 별 데미지
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
    /// 공격 사용 시 호출
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// 공격 이펙트
    /// </summary>
    /// <param name="index">이펙트 인덱스</param>
    public abstract void AttackVFX(int index);

    /// <summary>
    /// 공격 간 이동 기능
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
    /// 차징 공격도 있고 각성 상태에 따라 타수 변경/데미지 변경도 있어서 이쪽으로 이동
    /// 데미지 계산 후 콜라이더에 데미지 입력
    /// </summary>
    /// <param name="index">데미지 인덱스</param>
    public abstract void DamageCal(int index);

    /// <summary>
    /// 공격 종료 후 판정 콜라이더 리셋
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
    /// 동작중이던 행동 전부 정지 -> abstract로 변경하는 것도?
    /// </summary>
    public abstract void Attack_Reset();
}
