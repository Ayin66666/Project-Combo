using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect Healing", menuName = "Item Data / Item Effect / Effect Healing", order = int.MaxValue)]
public class Effect_Healing : Item_EffectBase
{
    [Header("---Healing Status---")]
    [SerializeField] private int heal;
    [SerializeField] private float cooldown;

    [SerializeField] private GameObject healingVFX;


    public override void Use(int indx)
    {
        canUse = false;
        Player_Manager.instance.cooldown.Cooldown(key, TimerCall());

        // ȸ��
        // player_Manager.instance.Healing(heal);

        // ȸ�� ����Ʈ
        // Instantiate(healingVFX, player_Manager.instance.transform.poition, Quaternion.identity);
    }
}
