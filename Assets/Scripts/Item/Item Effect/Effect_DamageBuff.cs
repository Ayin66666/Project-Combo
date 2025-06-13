using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_DamageBuff : Item_EffectBase
{
    [Header("---Buff Setting---")]
    [SerializeField] private int add_Physical_Damage;
    [SerializeField] private int add_Magical_Damage;
    [SerializeField] private float add_Critical_Chance;
    [SerializeField] private float add_Critical_Multiplier;
    [SerializeField] private float add_AttackSpeed;

    [SerializeField] private float buffTime;


    public override void Use(int indx)
    {
        // 여기 다른 방식으로 손봐야함!
        Player_Manager.instance.cooldown.Cooldown(key, UseCall());
    }

    private IEnumerator UseCall()
    {
         // 스테이터스 증가

        // 대기 시간
        yield return new WaitForSeconds(buffTime);

        // 스테이터스 감소
    }
}
