using Cinemachine;
using Easing.Tweening;
using System.Collections;
using UnityEngine;

public class CutScene_Chapter1_2_Elite_Die : MonoBehaviour
{
    [Header("---Setting - Mech---")]
    public GameObject explosion_Big;
    public GameObject explosion_Big2;
    public GameObject explosion_Ring;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private Transform[] explosionPos;


    [Header("---Setting - Yuria---")]
    [SerializeField] private GameObject ExCam;
    [SerializeField] private Transform[] camPos;
    [SerializeField] private float moveTime;
    [SerializeField] private GameObject body_Yuria;
    [SerializeField] private Transform[] movePos;
    [SerializeField] private GameObject[] effect;
    [SerializeField] float camSpeed;
    [SerializeField] float camSpeed2;
    [SerializeField] float camUpSpeed;
    [SerializeField] float camOutSpeed;


    [Header("---Component---")]
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private Animator anim_Mech;
    [SerializeField] private Animator anim_Yuria;
    [SerializeField] private GameObject[] cams;
    private Coroutine actionCoroutine;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (actionCoroutine != null)
                StopCoroutine(actionCoroutine);

            actionCoroutine = StartCoroutine(Action());
        }
    }

    public void Cam(int index)
    {
        foreach (GameObject t in cams)
        {
            t.SetActive(false);
        }

        cams[index].SetActive(true);
    }

    public void CamMove()
    {
        // Effect(1);
        var blend = brain.m_DefaultBlend;
        blend.m_Time = camSpeed2;
        blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        brain.m_DefaultBlend = blend;
        Cam(4);
    }

    public void Effect(int index)
    {

        effect[index].SetActive(true);
    }

    private IEnumerator Action()
    {
        // 걷기
        var blend = brain.m_DefaultBlend;
        blend.m_Time = 0f; // 시간 0
        blend.m_Style = CinemachineBlendDefinition.Style.Cut; // 완전 즉시 전환
        brain.m_DefaultBlend = blend;

        // 애니메이션
        anim_Mech.SetTrigger("Action");
        anim_Mech.SetBool("isAction", true);

        // 폭발 이펙트
        StartCoroutine(Exposion());

        // 애니메이션 대기
        yield return new WaitWhile(() => anim_Mech.GetBool("isAction"));

        // 안개
        effect[0].SetActive(true);
        yield return new WaitForSeconds(1.5f);

        // 걷기
        blend = brain.m_DefaultBlend;
        blend.m_Time = 0f; // 시간 0
        blend.m_Style = CinemachineBlendDefinition.Style.Cut; // 완전 즉시 전환
        brain.m_DefaultBlend = blend;
        Cam(3);

        Invoke(nameof(CamMove), camUpSpeed);

        effect[1].SetActive(true);
        body_Yuria.SetActive(true);
        anim_Yuria.SetFloat("AnimValue", 0);
        float timer = 0;
        float speed = 0;
        while (timer < 1)
        {
            speed += speed < 1 ? Time.deltaTime * 5f : speed = 1;
            anim_Yuria.SetFloat("AnimValue", speed);

            timer += Time.deltaTime / moveTime;
            body_Yuria.transform.position = Vector3.Lerp(movePos[0].position, movePos[1].position, timer);
            yield return null;
        }


        // 뭔가 액션
        anim_Yuria.SetTrigger("Action");
        anim_Yuria.SetBool("isAction", true);

        blend = brain.m_DefaultBlend;
        blend.m_Time = camSpeed;
        blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        brain.m_DefaultBlend = blend;

        // 총 겨누기 대기
        yield return new WaitWhile(() => anim_Yuria.GetBool("isAction"));

        // 로봇 제거
        // anim_Mech.gameObject.SetActive(false);
        explosion_Big2.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        explosion_Ring.SetActive(true);
    }

    public void CamExMove()
    {
        StartCoroutine(CamMovement());
    }

    private IEnumerator CamMovement()
    {
        yield return new WaitForSeconds(0.025f);
        float timer = 0;
        while (timer < 1)
        {
            if (timer < 1)
            {
                timer += Time.deltaTime / camOutSpeed;
            }

            ExCam.transform.position = Vector3.Lerp(camPos[0].transform.position, camPos[1].transform.position, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        ExCam.transform.position = camPos[1].transform.position;
    }

    private IEnumerator Exposion()
    {
        while (anim_Mech.GetBool("isAction"))
        {
            int ran = Random.Range(0, explosionPos.Length - 1);
            GameObject obj = Instantiate(explosionVFX, explosionPos[ran].position, explosionPos[ran].rotation);
            obj.transform.parent = explosionPos[ran];
            obj.transform.localScale = explosionPos[ran].transform.localScale;

            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }
    }
}
