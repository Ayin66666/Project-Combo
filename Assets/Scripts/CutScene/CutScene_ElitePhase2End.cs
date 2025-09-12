using Cinemachine;
using System.Collections;
using UnityEngine;


public class CutScene_ElitePhase2End : MonoBehaviour
{
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private GameObject[] cams;

    [Header("---Setting---")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float smokeTime;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject body;
    [SerializeField] private Transform movePos;
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject[] smoke;
    [SerializeField] private GameObject bulliet;
    private Coroutine actioncoroutine;
    private bool isBulletHit;
    private bool isBackStep;
    private bool isBackStepUse;


    [Header("---Eunha---")]
    [SerializeField] private float moveSpeedE;
    [SerializeField] private Animator animE;
    [SerializeField] private GameObject bodyE;
    [SerializeField] private Transform movePosE;
    [SerializeField] private GameObject attackVFXE;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (actioncoroutine != null)
                StopCoroutine(actioncoroutine);

            actioncoroutine = StartCoroutine(Action());
        }
    }

    private IEnumerator Action()
    {
        // 은하 돌진 - 은하 공격 - 유리아 백스텝 - 유리아 사격 - 안개 - 종료
        isBulletHit = false;

        // 은하 돌진

        var blend = brain.m_DefaultBlend;
        blend = brain.m_DefaultBlend;
        blend.m_Time = 0.25f; // 시간 0
        blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut; // 완전 즉시 전환
        brain.m_DefaultBlend = blend;

        Cam(0);
        animE.SetTrigger("Action");
        Vector3 startPos = bodyE.transform.position;
        Vector3 endPos = movePosE.position;
        float timer = 0;
        while (timer < 1)
        {
            if(timer > 0.35f && !isBackStepUse)
            {
                // 유리아 백스텝
                StartCoroutine(Backstep());
            }

            timer += Time.deltaTime / moveSpeedE;
            bodyE.transform.position = Vector3.Lerp(startPos, endPos, timer);
            yield return null;
        }


        // 은하 공격
        Cam(1);
        animE.SetTrigger("Action");
        animE.SetBool("isAttack", true);
        yield return new WaitWhile(() => animE.GetBool("isAttack"));


        // 은하 가드
        blend.m_Time = 0f;
        blend.m_Style = CinemachineBlendDefinition.Style.Cut; // 완전 즉시 전환
        animE.SetTrigger("Action");


        // 탄 피격 대기
        yield return new WaitWhile(() => !isBulletHit);


        // 안개 지속
        Cam(2);
        smoke[0].SetActive(true);
        body.SetActive(false);
        yield return new WaitForSeconds(smokeTime);


        // 은하 검 내리기
        blend.m_Time = 2.5f;
        blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        Cam(3);
        animE.SetTrigger("Action");


        // 안개 제거
        smoke[0].SetActive(false);
        smoke[1].SetActive(true);
        yield return new WaitWhile(() => smoke[1].activeSelf);

        // 컷신 종료
    }

    private IEnumerator Backstep()
    {
        isBackStepUse = true;
        isBackStep = true;

        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        Vector3 startPos = body.transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / moveSpeed;
            body.transform.position = Vector3.Lerp(startPos, endPos, timer);
            anim.SetFloat("AnimValue", timer);
            yield return null;
        }
        body.transform.position = endPos;
        anim.SetFloat("AnimValue", 1);

        isBackStep = false;


        // 사격
        anim.SetTrigger("Action");
    }

    public void BulletHit()
    {
        isBulletHit = true;
    }

    public void AttackVFXE()
    {
        attackVFXE.SetActive(true);
    }

    public void Shooting()
    {
        GameObject obj = Instantiate(bulliet, shootPos.position, Quaternion.identity);
        Vector3 moveDir = (bodyE.transform.position - body.transform.position).normalized;
        obj.GetComponent<CutScene_Shooting>().Movement_Setting(moveDir, 15, 15, this);
    }

    public void Cam(int index)
    {
        foreach(GameObject obj in cams)
        {
            obj.SetActive(false);
        }
        cams[index].SetActive(true);
    }
}
