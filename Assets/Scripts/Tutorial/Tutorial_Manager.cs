using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Manager : MonoBehaviour
{
    public static Tutorial_Manager instance;

    // 1. 시작 다이얼로그
    // 2. 기본 조작 설명 & 이동 지시 - 조작판
    // 3. 오브젝트 설명
    // 4. 이동 지시 - 전투 필
    // 5. 전투 1번 - 일반 & 스매쉬 (일반 소환)
    // 6. 전투 2번 - 스킬 & 각성
    // 7. 전투 2번 - 필살기
    // 7. 전투 종료 - 아지트 이동


    [Header("---Tutorial UI---")]
    [SerializeField] private GameObject[] tutorials_Tooltip;
    [SerializeField] private GameObject[] tutorial_UI;
    public bool istutorialOn;
    private bool isItem;
    private bool isSkill;


    [Header("---Tutorial Spawn---")]
    [SerializeField] private Field_Base[] spawn;


    [Header("---Tutorial Item & Skill---")]
    [SerializeField] private GameObject[] tutorial_Item_Skill;
    public System.Action tutorialAction;
    public bool isAction;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    #region 장비 착용 & 스킬트리 퀘스트
    /// <summary>
    /// 액션으로 호출할 이벤트 1 - 장비 착용 여부 체크
    /// </summary>
    private void Item_Setting()
    {
        isItem = true;
    }

    /// <summary>
    /// 액션으로 호출할 이벤트 2 - 스킬 획득 여부 체크
    /// </summary>
    private void Skill_Setting()
    {
        isSkill = true;
    }

    /// <summary>
    /// 장비 착용 & 스킬트리 퀘스트 종료 후 호출
    /// </summary>
    public void ItemSkill_TutorialOver()
    {
        Stage_Manager.instance.Dialog(4);
    }
    #endregion


    /// <summary>
    /// 우 하단 작은 텍스트 UI
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="index"></param>
    public void Tutorial_Tooltip(bool isOn, int index)
    {
        tutorials_Tooltip[index].SetActive(isOn);
    }

    /// <summary>
    /// 화면 전체 UI
    /// </summary>
    /// <param name="index"></param>
    public void Tutorial_Big(int index)
    {
        Player_Manager.instance.canAction = false;
        istutorialOn = true;

        Time.timeScale = 0f;

        // 튜토리얼 전체 비활성화
        foreach (GameObject obj in tutorial_UI)
        {
            obj.SetActive(false);
        }

        // 튜토리얼 활성화
        tutorial_UI[index].SetActive(true);
    }

    /// <summary>
    /// 튜토리얼 종료 버튼 함수
    /// </summary>
    public void TutorialOff()
    {
        Time.timeScale = 1f;

        foreach (GameObject obj in tutorial_UI)
        {
            obj.SetActive(false);
        }

        Player_Manager.instance.canAction = true;
        istutorialOn = false;
    }

    /// <summary>
    /// 튜토리얼 에너미 소환 기능 - 필드 설치해두고 스폰 기능 동작!
    /// </summary>
    /// <param name="index"></param>
    public void Tutorial_Spawn(int index)
    {
        spawn[index].Field_Start();
    }
}
