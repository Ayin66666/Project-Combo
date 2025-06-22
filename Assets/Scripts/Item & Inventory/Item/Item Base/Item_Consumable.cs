using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Consumable", menuName = "Item/Item Consumable", order = int.MaxValue)]
public class Item_Consumable : Item_Base
{
    [Header("---Consumable Setting---")]
    [SerializeField] protected ConsumableType consumableType;
    [SerializeField] private float timeCooldown;
    [SerializeField] private float effect_duration;
    [SerializeField] private float heal_interval;
    protected enum ConsumableType { oneoff, persistence }


    [Header("---Heal Setting---")]
    [SerializeField] private int healing;
    [SerializeField] private int stamina;
    [SerializeField] private int awakening;


    [Header("---Effect---")]
    [SerializeField] private GameObject recoveryVFX;


    public override void Use()
    {
        switch (consumableType)
        {
            case ConsumableType.oneoff:
                OneOff();
                break;

            case ConsumableType.persistence:
                if (Cooldown_Manager.instance.IsCooldownActive(coroutine_Key)) return;
                Cooldown_Manager.instance.Coroutine_Delegate(coroutine_Key, Persistence());
                break;
        }
    }


    #region ȸ�� ����
    /// <summary>
    /// ��� ȸ�� ����
    /// </summary>
    private void OneOff()
    {
        // ȸ�� ����Ʈ
        Instantiate(recoveryVFX, Player_Manager.instance.Player.transform.position, Quaternion.identity);

        // ȸ��
        Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Hp, healing);
        Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Stamina, stamina);
        Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Awakening, awakening);
    }

    /// <summary>
    /// ���� ȸ�� ����
    /// </summary>
    private IEnumerator Persistence()
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

                // ȸ�� ����Ʈ
                Instantiate(recoveryVFX, Player_Manager.instance.Player.transform.position, Quaternion.identity);

                // ƽ���� ȸ��
                Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Hp, healing);
                Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Stamina, stamina);
                Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Awakening, awakening);
            }

            yield return null;
        }

        Cooldown_Manager.instance.Remove(coroutine_Key);
    }
    #endregion
}
