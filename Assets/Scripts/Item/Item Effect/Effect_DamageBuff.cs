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
        // ���� �ٸ� ������� �պ�����!
        Player_Manager.instance.cooldown.Cooldown(key, UseCall());
    }

    private IEnumerator UseCall()
    {
         // �������ͽ� ����

        // ��� �ð�
        yield return new WaitForSeconds(buffTime);

        // �������ͽ� ����
    }
}
