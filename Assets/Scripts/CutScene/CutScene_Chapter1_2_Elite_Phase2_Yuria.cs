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
    [SerializeField] private Transform[] movePos_Yuria;


    [Header("---Setting / Eunha---")]
    [SerializeField] private GameObject body_Eunha;
    [SerializeField] private Animator anim_Eunha;
    [SerializeField] private GameObject swordAura_VFX;
    [SerializeField] private Transform shootPos_Eunha;
    [SerializeField] private Transform movePos_Eunha;
    [SerializeField] private GameObject[] vfx_EunHa;
    private Coroutine movementCoroutine;

    
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
        yield return new WaitWhile(() => anim_Eunha.GetBool("isAction")); // 검기 종료 대기


        // 2. 은하 백스탭
        anim_Eunha.SetTrigger("Action");
        anim_Eunha.SetBool("isAction", true);
        Vector3 startPos = body_Eunha.transform.position;
        Vector3 endPos = movePos_Eunha.transform.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / 0.5f;
            anim_Eunha.SetFloat("AnimValue", timer);
            body_Eunha.transform.position = Vector3.Lerp(startPos, endPos, timer);
            yield return null;
        }
        body_Eunha.transform.position = endPos;


        // 3. 은하 가드
        anim_Eunha.SetTrigger("Action");
        yield return new WaitWhile(() => !anim_Yuria.GetBool("isCounter")); // 가드 다 올릴때까지 대기


        // 4. 유리아 사격
        yield return new WaitForSeconds(0.25f);
        anim_Yuria.SetTrigger("Action");


        // 5. 은하 피격 & 안개 생성 & 유리아 Off - 이벤트 호출


        // 6. 카운터 공격 


        // 6. 안개 제거 & 컷신 종료
    }


    #region
    public void Movement_Yuria(int index)
    {
        if(movementCoroutine != null) 
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(MovementYuria(transform.position, movePos_Yuria[index].position));
    }

    private IEnumerator MovementYuria(Vector3 startPos, Vector3 endPos)
    {
        anim_Yuria.SetTrigger("Action");
        anim_Yuria.SetFloat("AnimValue", 0);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / 0.45f;
            body_Yuria.transform.position = Vector3.Lerp(startPos, endPos, timer);
            anim_Yuria.SetFloat("AnimValue", timer);
            yield return null;
        }
        anim_Yuria.SetFloat("AnimValue", 1);
        body_Yuria.transform.position = endPos;
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
        Vector3 moveDir = (shootPos_Eunha.transform.position - shootPos_Yuria.transform.position).normalized;
        obj.GetComponent<CutScene_Shooting>().Movement_Setting(moveDir, 10, 30f);
    }

    /// <summary>
    /// 유리아 사격으로 인한 안개 생성
    /// </summary>
    public void Shoot_Smoke(bool isOn)
    {
        smoke_VFX.SetActive(isOn);
    }
    #endregion


    /// <summary>
    /// 유리아 바디 비활성화
    /// </summary>
    public void Remove_Yuria()
    {
        body_Yuria.SetActive(false);
    }


    #region
    /// <summary>
    /// 공격 이펙트 활성화
    /// </summary>
    /// <param name="index"></param>
    public void VFX_Eunha(int index)
    {
        //vfx_EunHa[index].SetActive(true);
    }

    /// <summary>
    /// 은하 검기 발사
    /// </summary>
    public void Shoot_Eunha()
    {
        
        GameObject obj = Instantiate(swordAura_VFX, shootPos_Yuria.transform.position, Quaternion.identity);
        Vector3 moveDir = (shootPos_Yuria.transform.position - shootPos_Eunha.transform.position).normalized;
        obj.GetComponent<CutScene_Shooting>().Movement_Setting(moveDir, 10, 30f);
        
    }
    #endregion
}