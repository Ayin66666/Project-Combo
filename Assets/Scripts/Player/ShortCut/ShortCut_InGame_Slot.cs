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
    /// ���� ������ ����
    /// </summary>
    /// <param name="item"></param>
    public void Slot_Setting(Item_Base item)
    {
        this.item = item;
        icon.sprite = item.Icon;
    }

    /// <summary>
    /// UI �ʱ�ȭ
    /// </summary>
    public void Slot_Reset()
    {
        StopCoroutine(cooldownCoroutine);

        item = null;
        icon.sprite = null;
        cooldown.fillAmount = 0;
    }

    /// <summary>
    /// ��� �� ȣ�� - ��Ÿ�� UI
    /// </summary>
    public void Cooldown()
    {
        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);

        cooldownCoroutine = StartCoroutine(CooldownCall());
    }

    private IEnumerator CooldownCall()
    {
        cooldown.fillAmount = 0;
        float timer = 0;
        while(timer < 1)
        {
            cooldown.fillAmount = Mathf.Lerp(0, 1, Player_Manager.instance.cooldown.SlotUI_Cooldown(((Item_Consumable)item).Key));
            yield return null;
        }
        cooldown.fillAmount = 1;
    }
}
