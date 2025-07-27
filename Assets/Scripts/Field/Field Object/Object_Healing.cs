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
        // ���� ȣ�� ���
        if (isUsed)
        {
            return;
        }

        StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;

        // ����Ʈ
        GameObject obj = Instantiate(healVFX, PlayerAction_Manager.instance.transform.position, Quaternion.identity);
        obj.transform.parent = PlayerAction_Manager.instance.transform;
        Vector3 pos = obj.transform.position;
        pos.y += 0.5f;
        obj.transform.position = pos;

        // ȸ��
        PlayerAction_Manager.instance.Healing(healHp);

        // UI ����
        text.text = "ȸ�� �ý��� ����";
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
