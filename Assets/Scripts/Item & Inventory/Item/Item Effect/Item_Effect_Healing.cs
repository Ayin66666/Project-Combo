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

    [Header("---effect---")]
    [SerializeField] private GameObject effect;



    public override void Effect()
    {
        switch (type)
        {
            case Type.oneoff:
                Player_Manager.instance.cooldown.EffectUse(Key, Healing_Oneoff(), Cooldown);
                break;

            case Type.persistence:
                Player_Manager.instance.cooldown.EffectUse(Key, Healing_Persistence(), Cooldown);
                break;
        }
    }

    /// <summary>
    /// 단발 회복 동작
    /// </summary>
    private IEnumerator Healing_Oneoff()
    {
        // 이펙트 생성
        GameObject obj = Instantiate(effect, Player_Manager.instance.Player.transform.position, Quaternion.identity);
        obj.transform.parent = Player_Manager.instance.Player.transform;

        // 틱 회복
        if (hp > 0)
            Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Hp, hp);

        if (stamina > 0)
            Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Stamina, stamina);

        if (awakning > 0)
            Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Awakening, awakning);

        yield return null;
    }

    /// <summary>
    /// 지속 회복 동작 로직
    /// </summary>
    /// <returns></returns>
    private IEnumerator Healing_Persistence()
    {
        Debug.Log($"Call Equipment Healing / Duration & interval : {effect_duration}, {heal_interval} / (Hp : {hp} / Stamina : {stamina} / Awakning : {awakning})");
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
                if (hp > 0)
                    Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Hp, hp);

                if (stamina > 0)
                    Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Stamina, stamina);

                if (awakning > 0)
                    Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Awakening, awakning);
            }

            yield return null;
        }
    }
}
