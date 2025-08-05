using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_CheckPoint : Object_Base
{
    [Header("---CheckPoint Setting---")]
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject useVFX;

    public override void Use()
    {
        // ���� ȣ�� ���
        if (isUsed)
        {
            return;
        }

        StartCoroutine(UseCall());
    }

    public IEnumerator UseCall()
    {
        // üũ����Ʈ Ȱ��ȭ
        Stage_Manager.instance.CheckPoint_Seting(spawnPos);

        // ����Ʈ
        useVFX.SetActive(true);

        // UI ����
        text.text = "���ε� �Ϸ�";
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
