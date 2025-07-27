using System.Collections;
using TMPro;
using UnityEngine;
using Easing.Tweening;


public abstract class Object_Base : MonoBehaviour
{
    [Header("---Base Setting---")]
    [SerializeField] private ObjectType type;
    [SerializeField] protected bool isUsed;
    private bool isPlayerIn;
    private bool isUIOn;
    protected Coroutine uiCoroutine;
    private Quaternion originalRot;
    private enum ObjectType { Door, Healing, CheckPoint, Elevator }


    [Header("---UI---")]
    [SerializeField] private GameObject iconSet;
    [SerializeField] private CanvasGroup iconCanvasGroup;
    [SerializeField] protected TextMeshProUGUI text;
    [SerializeField] protected Collider coll;


    private void Start()
    {
        originalRot = iconSet.transform.rotation;
        coll = GetComponent<Collider>();
    }

    public abstract void Use();


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
    }

    protected IEnumerator IconUseOff()
    {
        float timer = 0f;
        while (timer < 1)
        {
            timer += Time.deltaTime * 0.65f;
            iconCanvasGroup.alpha = Mathf.Lerp(1, 0, EasingFunctions.InOutElastic(timer));
            yield return null;
        }

        iconCanvasGroup.alpha = 0f;
    }

    protected void LookAt()
    {
        
        Vector3 lookDir = iconSet.transform.position - PlayerAction_Manager.instance.cam.transform.position;
        lookDir.y = 0;
        iconSet.transform.rotation = Quaternion.LookRotation(lookDir.normalized);


        Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized); // 카메라 바라보는 방향
        float deltaY = Quaternion.Angle(originalRot, targetRot);            // 두 회전 사이의 전체 각도 차이

        // 회전 방향을 자기 기준으로 얻기 위해 Vector3.Angle 대신 SignedAngle 사용
        Vector3 originalForward = originalRot * Vector3.forward;
        Vector3 targetForward = targetRot * Vector3.forward;
        float signedAngle = Vector3.SignedAngle(originalForward, targetForward, Vector3.up);

        float clampedAngle = Mathf.Clamp(signedAngle, -45f, 45f); // ±30도 제한

        // 제한된 회전 각도만큼 회전한 새로운 방향 계산
        Quaternion limitedRot = Quaternion.AngleAxis(clampedAngle, Vector3.up) * originalRot;

        iconSet.transform.rotation = limitedRot;
    }
    #endregion


    #region Ontrigger
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerIn = true;

            if(!isUIOn)
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
            coll.enabled = false;
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
