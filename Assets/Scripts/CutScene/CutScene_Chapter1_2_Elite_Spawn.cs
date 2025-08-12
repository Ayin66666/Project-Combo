using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;


public class CutScene_Chapter1_2_Elite_Spawn : MonoBehaviour
{
    [System.Serializable]
    public struct MovementData
    {
        [SerializeField] private string name;
        public Transform movePos;
        public float moveTime;
    }


    [Header("---Setting---")]
    [SerializeField] private List<MovementData> movementData;
    [SerializeField] private Vector3 TargetRotation;

    [Header("---Component---")]
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject camHolder;
    [SerializeField] private Animator anim;
    public GameObject[] effects;
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
    private IEnumerator Action()
    {
        Cams(0);

        // 1.직선 비행
        Vector3 startPos = body.transform.position;
        Vector3 endPos = movementData[0].movePos.position;
        Quaternion startRot = camHolder.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(TargetRotation);

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / movementData[0].moveTime;
            body.transform.position = Vector3.Lerp(startPos, endPos, timer);
            camHolder.transform.localRotation = Quaternion.Lerp(startRot, endRot, timer);
            yield return null;
        }
        body.transform.position = endPos;

        
        Cams(1);

        // 2. 착지 지점 표기
        effects[1].SetActive(true);

        // 전환 애니메이션 대기
        anim.SetTrigger("Action");
        anim.SetBool("isAction", true);
        yield return new WaitWhile(() => anim.GetBool("isAction"));

        // 3. 착지
        anim.SetTrigger("Action");
        anim.SetBool("isAction", true);
        anim.SetFloat("AnimValue", 0);
        startPos = body.transform.position;
        endPos = movementData[1].movePos.position;
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / movementData[1].moveTime;
            body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            anim.SetFloat("AnimValue", timer);
            yield return null;
        }
        body.transform.position = endPos;
        anim.SetFloat("AnimValue", 1);

        // 케메라 변경
        Cams(2);

        // 착륙 후 일시 대기
        yield return new WaitForSeconds(1.5f);

        // 화염 Off
        effects[0].SetActive(false);

        // 케메라 변경
        Cams(3);

        // 착륙 애니메이션 대기
        anim.SetTrigger("Action");
        anim.SetBool("isAction", true);
        yield return new WaitWhile(() => anim.GetBool("isAction"));
    }

    private void Cams(int index)
    {
        foreach (GameObject cam in cams)
        {
            cam.SetActive(false);
        }

        cams[index].SetActive(true);
    }
}
