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


    [Header("---Setting / Yuria---")]
    [SerializeField] private GameObject body_Yuria;
    [SerializeField] private Animator anim_Yuria;
    [SerializeField] private GameObject bulliet_VFX;
    [SerializeField] private Transform shootPos_Yuria;


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
        // 1. 은하 검기 & 유리아 좌우 회피
        anim_Eunha.SetTrigger("Action");
        anim_Eunha.SetBool("isAction", true);

        anim_Yuria.SetTrigger("Action");
        anim_Yuria.SetBool("isAction", true);

        yield return new WaitWhile(() => anim_Yuria.GetBool("isAction"));

        // 3. 유리아 사격

        // 4. 은하 좌 이동

        // 5. 유리아 저격

        // 6. 은하 카운터 + 안개

        // 7. 대화

        // 8. 안개 제거 + 유리아 퇴장

        // 9. 컷신 종료

        yield return null;
    }
}