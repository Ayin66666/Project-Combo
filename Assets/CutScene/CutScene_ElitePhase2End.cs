using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene_ElitePhase2End : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float smokeTime;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject body;
    [SerializeField] private Transform movePos;
    [SerializeField] private GameObject[] smoke;
    private Coroutine actioncoroutine;


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(actioncoroutine != null) 
                StopCoroutine(actioncoroutine);

            actioncoroutine = StartCoroutine(Action());
        }
    }

    private IEnumerator Action()
    {
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);


        // 백스텝 - 사격 - 안개 - 종료


        // 백스텝
        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / moveSpeed;
            body.transform.position = Vector3.Lerp(startPos, endPos, timer);
            anim.SetFloat("AnimValue", timer);
            yield return null;
        }
        body.transform.position = endPos;
        anim.SetFloat("AnimValue", 1);


        // 사격
        anim.SetTrigger("Action");
        smoke[0].SetActive(true);

        // 안개 지속
        yield return new WaitForSeconds(smokeTime);

        // 안개 제거
        smoke[0].SetActive(false);
        smoke[1].SetActive(true);
        yield return new WaitWhile(() => smoke[1].activeSelf);

        // 컷신 종료
    }
}
