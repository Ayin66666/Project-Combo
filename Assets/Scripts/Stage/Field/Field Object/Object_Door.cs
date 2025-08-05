using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Door : Object_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject[] door;


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
        // 문 개방
        isUsed = true;
        foreach (GameObject go in door)
        {
            go.SetActive(false);
        }

        // UI 동작
        text.text = "시스템 해제 완료";
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
