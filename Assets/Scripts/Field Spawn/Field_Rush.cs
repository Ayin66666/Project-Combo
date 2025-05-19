using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_Rush : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private List<DialogData> datas;
    [SerializeField] private float nextRoundDelay;
    private readonly WaitForSeconds checkInterval = new WaitForSeconds(1f);


    public override void Field_Start()
    {
        StartCoroutine(StartCall());
    }

    private IEnumerator StartCall()
    {
        // 문 동작
        foreach (GameObject obj in door)
        {
            obj.SetActive(true);
        }

        // 시작 다이얼로그 체크
        if (haveStartDialog)
            UI_Manager.instance.Dialog_Fight(startDialog.dialog);

        // 맵 UI 최소화
        UI_Manager.instance.MiniMap_SizeSetting(false);

        // 몬스터 소환 - 라운드
        for (int i = 0; i < datas.Count; i++)
        {
            // 몬스터 소환 - 몬스터
            enemyCount = spawnDatas[i].enemys.Count;
            for (int j = 0; j < spawnDatas[i].enemys.Count; j++)
            {
                spawnDatas[i].enemys[j].SetActive(true);
            }

            // 몬스터 수 체크
            int aliveCount = 0;
            foreach (var enemy in spawnDatas[i].enemys)
            {
                if (enemy != null)
                    aliveCount++;
            }

            // 라운드 종료 대기
            while (true)
            {
                // 몬스터 체크
                if (aliveCount == 0)
                    break;

                yield return checkInterval;
            }

            // 다음 라운드 대기 시간
            yield return new WaitForSeconds(nextRoundDelay);
        }

        // 필드 종료
        Field_End();
    }

    public override void Field_End()
    {
        // 클리어 UI
        UI_Manager.instance.FieldClear_Normal();

        // 종료 다이얼로그
        if (haveEndDialog)
            UI_Manager.instance.Dialog_Fight(endDialog.dialog);

        // 문 개방
        foreach (GameObject obj in door)
        {
            obj.SetActive(false);
        }

        // 맵 UI 최대화
        UI_Manager.instance.MiniMap_SizeSetting(true);
    }
}
