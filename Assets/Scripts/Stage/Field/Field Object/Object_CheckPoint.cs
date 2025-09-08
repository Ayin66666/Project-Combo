using System.Collections;
using UnityEngine;


public class Object_CheckPoint : Object_Base
{
    [Header("---CheckPoint Setting---")]
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject useVFX;

    public override void Use()
    {
        // 재사용 호출 대비
        if (isUsed)
        {
            return;
        }

        StartCoroutine(UseCall());
    }

    public IEnumerator UseCall()
    {
        // 체크포인트 활성화
        Stage_Manager.instance.CheckPoint_Seting(spawnPos);

        // 이펙트
        effect.SetActive(false);
        useVFX.SetActive(true);

        // UI 동작
        text.text = "데이터 업로드 완료";
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
