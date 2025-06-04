using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Healing : Object_Base
{
    [Header("---Setting---")]
    [SerializeField] private int healHp;
    [SerializeField] private GameObject healVFX;


    public override void Use()
    {
        // 재사용 호출 대비
        if (isUsed)
        {
            return;
        }

        StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;

        // 이펙트
        GameObject obj = Instantiate(healVFX, PlayerAction_Manager.instance.transform.position, Quaternion.identity);
        obj.transform.parent = PlayerAction_Manager.instance.transform;
        Vector3 pos = obj.transform.position;
        pos.y += 0.5f;
        obj.transform.position = pos;

        // 회복
        PlayerAction_Manager.instance.Healing(healHp);

        // UI 동작
        text.text = "회복 시스템 가동";
        float timer = 0;
        while (timer < 0.25f)
        {
            LookAt();
            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(IconUseOff());
    }
}
