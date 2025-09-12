using Cinemachine;
using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitScene_Boss_Special : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private float wingMoveSpeed;
    [SerializeField] private float chargeTime;
    [SerializeField] private float eyeSpeed;
    [SerializeField] private float lastCamSpeed;
    [SerializeField] private GameObject[] vfx;
    [SerializeField] private List<GameObject> cams;


    [Header("---Component---")]
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private Animator anim;
    [SerializeField] private SkinnedMeshRenderer face;
    [SerializeField] private SkinnedMeshRenderer wing;
    private Coroutine actionCoroutine;


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(actionCoroutine != null)
                StopCoroutine(actionCoroutine);

            actionCoroutine = StartCoroutine(Acton());
        }
    }


    private IEnumerator Acton()
    {
        Cams(0);
        anim.SetFloat("AnimValue", 0);
        face.SetBlendShapeWeight(26, 100);


        var blend = brain.m_DefaultBlend;
        blend = brain.m_DefaultBlend;
        blend.m_Time = wingMoveSpeed/2;
        blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        brain.m_DefaultBlend = blend;
        Cams(1);


        // 차지 이펙트 & 날개 개방 & 카메라 회전
        vfx[0].SetActive(true);
        float timer = 0;
        float t = 100;
        while (timer < 1)
        {
            timer += Time.deltaTime / wingMoveSpeed;
            t = Mathf.Lerp(0, 100, EasingFunctions.OutExpo(timer));
            wing.SetBlendShapeWeight(0, t);
            anim.SetFloat("AnimValue", timer);
            yield return null;
        }
        vfx[1].SetActive(true);
        anim.SetFloat("AnimValue", 1);


        // 차지 대기
        yield return new WaitForSeconds(chargeTime);


        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / eyeSpeed;
            face.SetBlendShapeWeight(26, Mathf.Lerp(100, 0, timer));
            yield return null;
        }
        face.SetBlendShapeWeight(26, 0);

        yield return new WaitForSeconds(0.25f);

        // 눈 뜨기
        blend = brain.m_DefaultBlend;
        blend.m_Time = lastCamSpeed;
        blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        brain.m_DefaultBlend = blend;
        Cams(2);

        // 차지 종료 이펙트 + 카메라 정면 전환
        anim.SetTrigger("Action");
        vfx[0].SetActive(false);
        vfx[2].SetActive(true);
    }

    public void Cams(int index)
    {
        cams.ForEach(obj => obj.SetActive(false));
        cams[index].SetActive(true);
    }
}
