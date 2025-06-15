using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShortCut_InGame_Slot : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Image icon;
    [SerializeField] private Image cooldown;
    [SerializeField] private Item_Base item;
    private Coroutine cooldownCoroutine;


    /// <summary>
    /// 슬롯 아이템 셋팅
    /// </summary>
    /// <param name="item"></param>
    public void Slot_Setting(Item_Base item)
    {
        icon.sprite = item.Icon;
    }

    /// <summary>
    /// UI 초기화
    /// </summary>
    public void Slot_Reset()
    {
        StopCoroutine(cooldownCoroutine);

        item = null;
        icon.sprite = null;
        cooldown.fillAmount = 0;
    }

    /// <summary>
    /// 사용 시 호출 - 쿨타임 UI
    /// </summary>
    public void Cooldown()
    {
        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);

        cooldownCoroutine = StartCoroutine(CooldownCall());
    }

    private IEnumerator CooldownCall()
    {
        cooldown.fillAmount = 1;
        float timer = 0;
        while(timer < 1)
        {
            Cooldown_Manager.instance.GetCooldown(item.coroutine_Key);
            yield return null;
        }
    }
}
