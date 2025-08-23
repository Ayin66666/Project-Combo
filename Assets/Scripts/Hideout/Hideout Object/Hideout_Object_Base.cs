using System.Collections;
using UnityEngine;
using Easing.Tweening;


public abstract class Hideout_Object_Base : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] protected Type type;
    [SerializeField] protected bool isUsed;
    protected enum Type { Store, Save, Stage }


    [Header("---Icon UI---")]
    [SerializeField] private GameObject iconSet;
    [SerializeField] private CanvasGroup iconCanvasGroup;
    [SerializeField] private bool isPlayerIn;
    [SerializeField] private bool isUIOn;
    private Quaternion originalRot;
    protected Coroutine uiCoroutine;


    /// <summary>
    /// 동작 시 호출
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// 종료 시 호출
    /// </summary>
    public abstract void Out();


    #region Icon
    protected IEnumerator IconOn()
    {
        isUIOn = true;

        // 아이콘 활성화
        iconCanvasGroup.alpha = 1;
        while (isPlayerIn)
        {
            // 바라보기
            LookAt();

            yield return null;
        }
    }

    protected IEnumerator IconOff()
    {
        isUIOn = false;

        // 아이콘 비활성화
        float start = iconCanvasGroup.alpha;
        float end = 0;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            iconCanvasGroup.alpha = Mathf.Lerp(start, end, timer);
            yield return null;
        }
        iconCanvasGroup.alpha = 0;
    }

    protected void LookAt()
    {
        // 카메라 - 아이콘 방향
        Vector3 lookDir = iconSet.transform.position - PlayerAction_Manager.instance.cam.transform.position;
        lookDir.y = 0; // 상하는 무시하고 수평만 고려

        // 카메라 방향을 그대로 바라보게
        Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);

        // 바로 적용
        iconSet.transform.rotation = targetRot;
    }
    #endregion


    #region Ontrigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;

            if (!isUIOn)
            {
                if (uiCoroutine != null)
                    StopCoroutine(uiCoroutine);

                uiCoroutine = StartCoroutine(IconOn());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.F) && !isUsed)
        {
            Use();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
            if (uiCoroutine != null)
                StopCoroutine(uiCoroutine);

            uiCoroutine = StartCoroutine(IconOff());
        }
    }
    #endregion
}
