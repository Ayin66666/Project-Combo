using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Effect", menuName = "Item Effect/Healing", order = int.MaxValue)]
public class Item_Effect_Healing : Item_Effect_SO
{
    [Header("---Healing Setting---")]
    [SerializeField] private Type type;
    [SerializeField] private float effect_duration;
    [SerializeField] private float heal_interval;

    [SerializeField] private int hp;
    [SerializeField] private int stamina;
    [SerializeField] private int awakning;
    protected enum Type { oneoff, persistence }


    protected override void Effect()
    {
        switch (type)
        {
            case Type.oneoff:
                Healing_Oneoff();
                break;

            case Type.persistence:
                Cooldown_Manager.instance.Coroutine_Delegate(key, Healing_Persistence());
                break;
        }
    }



    /// <summary>
    /// 단발 회복 동작
    /// </summary>
    private void Healing_Oneoff()
    {
        Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Hp, hp);
        Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Stamina, stamina);
        Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Awakening, awakning);
    }

    /// <summary>
    /// 지속 회복 동작 로직
    /// </summary>
    /// <returns></returns>
    private IEnumerator Healing_Persistence()
    {
        float timer = 0f;
        float intervalTimer = 0f;

        while (timer < effect_duration)
        {
            timer += Time.deltaTime;
            intervalTimer += Time.deltaTime;

            if (intervalTimer >= heal_interval)
            {
                intervalTimer = 0f;

                // 틱 회복
                Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Hp, hp);
                Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Stamina, stamina);
                Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Awakening, awakning);
            }

            yield return null;
        }

        Cooldown_Manager.instance.Remove(key);
    }
}
