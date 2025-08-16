using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CutScene_Chapter1_2_Elite_Phase2_Yuria : MonoBehaviour
{

    /*
     * 은하 검기 공격
     * 유리아 좌우 회피
     * 유리아 사격
     * 은하 좌로 달림
     * 유리아 저격
     * 은하 카운터 + 안개
     * 대화
     * 안개 제거 + 유리아 퇴장
    */
    private Coroutine coroutine;
    public enum Evnet { Dodge, Shoot, }

    [Header("---Setting / Yuria---")]
    [SerializeField] private GameObject body_Yuria;
    [SerializeField] private Animator anim_Yuria;
    [SerializeField] private Transform shootPos_Yuria;
    [SerializeField] private GameObject bulliet_VFX;
    [SerializeField] private GameObject smoke_VFX;


    [Header("---Setting / Eunha---")]
    [SerializeField] private GameObject body_Eunha;
    [SerializeField] private Animator anim_Eunha;
    [SerializeField] private GameObject swordAura_VFX;
    [SerializeField] private Transform shootPos_Eunha;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Action());
        }
    }

    private IEnumerator Action()
    {
        // 1. 은하 검기 - 유리아 좌우 회피
        anim_Eunha.SetTrigger("Action");
        anim_Eunha.SetBool("isAction", true);
        yield return new WaitWhile(() => anim_Yuria.GetBool("isAction")); // 검기 종료 대기


        // 2. 은하 백스탭 & 가드
        anim_Eunha.SetTrigger("Action");
        anim_Eunha.SetBool("isAction", true);
        anim_Eunha.SetBool("isCounter", true);
        yield return new WaitWhile(() => anim_Yuria.GetBool("isAction")); // 백스탭 종료 대기


        // 3. 유리아 사격
        anim_Eunha.SetTrigger("Action");
        yield return new WaitWhile(() => anim_Yuria.GetBool("isCounte")); // 카운터 종료 대기


        // 4. 은하 카운터 공격 - & 유리아 Off
        anim_Eunha.SetTrigger("Action");
        anim_Eunha.SetBool("isAction", true);
        yield return new WaitWhile(() => anim_Yuria.GetBool("isAction")); // 검기 종료 대기


        // 4. 유리아 대사

        // 5. 안개 제거 & 컷신 종료
    }

    /// <summary>
    /// 유리아의 다음 애니메이션 지시
    /// </summary>
    public void Action_Yuria()
    {
        anim_Yuria.SetTrigger("Action");
    }

    /// <summary>
    /// 유리아 조준사격
    /// </summary>
    public void Shoot_Yuria()
    {
        GameObject obj = Instantiate(bulliet_VFX, shootPos_Yuria.transform.position, Quaternion.identity);
        Vector3 moveDir = (body_Yuria.transform.position - body_Eunha.transform.position).normalized;
        obj.GetComponent<CutScene_Shooting>().Movement_Setting(moveDir, 10, 30f);
    }

    /// <summary>
    /// 유리아 사격으로 인한 안개 생성
    /// </summary>
    public void Shoot_Smoke(bool isOn)
    {
        smoke_VFX.SetActive(isOn);
    }


    /// <summary>
    /// 유리아 바디 비활성화
    /// </summary>
    public void Remove_Yuria()
    {
        body_Yuria.SetActive(false);
    }


}