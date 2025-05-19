using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_Normal : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private bool haveDialog;
    [SerializeField] private List<DialogData> countDialogData;
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

        // 에너미 소환
        enemyCount = spawnDatas[0].enemys.Count;
        for (int i = 0; i < spawnDatas[0].enemys.Count; i++)
        {
            spawnDatas[0].enemys[i].SetActive(true);
            yield return new WaitForSeconds(spawnDatas[0].spawnDelay);
        }

        // 체크 동작
        StartCoroutine(CheckCall());
    }

    private IEnumerator CheckCall()
    {
        // 몬스터 수 체크
        enemyCount = spawnDatas[0].enemys.Count;

        // 종료 대기
        while (enemyCount > 0)
        {
            // 몬스터 체크
            foreach (var enemy in spawnDatas[0].enemys)
            {
                if (enemy == null)
                    enemyCount--;
            }

            // 다이얼로그 체크
            if (haveDialog)
            {
                for (int i = 0; i < countDialogData.Count; i++)
                {
                    if (countDialogData[i].useCount <= enemyCount && !countDialogData[i].isUsed)
                    {
                        countDialogData[i].isUsed = true;
                        UI_Manager.instance.Dialog_Fight(countDialogData[i].dialog);
                    }
                }
            }

            // 체크 딜레이
            yield return checkInterval;
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

        // 맵 UI 최대화
        UI_Manager.instance.MiniMap_SizeSetting(true);

        // 문 개방
        foreach (GameObject obj in door)
        {
            obj.SetActive(false);
        }
    }
}
