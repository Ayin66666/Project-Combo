using System.Collections;
using UnityEngine;


public class Field_Boss : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Base boss;
    private  WaitForSeconds delay = new WaitForSeconds(1f);


    // 엘리트 몬스터 용 소환 트리 추가할 지?
    // 일부러 코드도 베이스 기반 구현으로 나눴는데 합칠 필요도 없다
    // 기존에 엘리트 제어코드 제거하고 소환코드 여기에 하는 것도?
    // 체크 대상을 컨트롤러로 잡고 제어하면 문제 없을듯 / 결론 - 그냥 하나로 사용
    

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
        while(boss.isActiveAndEnabled)
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
            yield return delay;
        }

        // 스테이지 종료
        Field_End();
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
