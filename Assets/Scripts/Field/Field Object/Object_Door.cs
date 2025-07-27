using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Door : Object_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject[] door;


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
        // �� ����
        isUsed = true;
        foreach (GameObject go in door)
        {
            go.SetActive(false);
        }

        // UI ����
        text.text = "�ý��� ���� �Ϸ�";
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
