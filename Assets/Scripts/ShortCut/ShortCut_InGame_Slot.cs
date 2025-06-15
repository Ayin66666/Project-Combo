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
        cooldown.fillAmount = 1;
        float timer = 0;
        while(timer < 1)
        {
            Cooldown_Manager.instance.GetCooldown(item.coroutine_Key);
            yield return null;
        }
    }
}
