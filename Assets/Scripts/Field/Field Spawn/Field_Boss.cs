using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class Field_Boss : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Base boss;
    private  WaitForSeconds delay = new WaitForSeconds(1f);


    [Header("---Dialog---")]
    [SerializeField] private List<Dialog> dialogs;

    [System.Serializable]
    public class Dialog
    {
        [SerializeField] private string dialogName;
        public bool isUsed;
        public int hp;
        public Dialog_Data_SO dialog;
    }


    public override void Field_Start()
    {
        StartCoroutine(StartCall());
    }

    private IEnumerator StartCall()
    {
        // 맵 UI 최소화
        UI_Manager.instance.MiniMap_SizeSetting(false);

        // 보스 소환
        boss.gameObject.SetActive(true);

        // 컷신 대기 -> 여기부터 구현 필요 (컷신 대기 / 체력에 따른 다이얼로그 표기 / 페이즈 2 변환 간 대기 등등)
        while(boss.isCutScene)
        {
            yield return null;
        }

        // 사운드
        Field_BGM();

        // 다이얼로그
        if (haveStartDialog)
            UI_Manager.instance.Dialog_Fight(startDialog.dialog);


        // 스테이지 체크
        while (boss == null)
        {
            Dialog_Check();
            yield return delay;
        }

        // 스테이지 종료
        Field_End();
    }

    private void Dialog_Check()
    {
        for (int i = 0; i < dialogs.Count; i++)
        {
            if (boss.curHp <= dialogs[i].hp && !dialogs[i].isUsed)
            {
                UI_Manager.instance.Dialog_Fight(dialogs[i].dialog);
                dialogs[i].isUsed = true;
            }
        }
    }

    public override void Field_End()
    {
        isClear = true;

        // 클리어 UI
        UI_Manager.instance.FieldClearUI(UI_Manager.ClearType.Boss);

        // 맵 UI 최대화
        UI_Manager.instance.MiniMap_SizeSetting(true);

        // 종료 다이얼로그
        if (haveEndDialog)
            UI_Manager.instance.Dialog_Fight(endDialog.dialog);

        // 문 개방
        foreach (GameObject obj in door)
        {
            obj.SetActive(false);
        }
    }

    public override void Field_Reset()
    {

    }
}
