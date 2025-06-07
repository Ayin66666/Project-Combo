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
    private bool isUIOn;
    private bool isPlayerIn;
    private Quaternion originalRot;
    protected Coroutine uiCoroutine;


    /// <summary>
    /// ���� �� ȣ��
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// ���� �� ȣ��
    /// </summary>
    public abstract void Out();


    #region Icon
    protected IEnumerator IconOn()
    {
        isUIOn = true;

        // ������ Ȱ��ȭ
        iconCanvasGroup.alpha = 1;
        while (isPlayerIn)
        {
            // �ٶ󺸱�
            LookAt();

            yield return null;
        }
    }

    protected IEnumerator IconOff()
    {
        isUIOn = false;

        // ������ ��Ȱ��ȭ
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


        Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized); // ī�޶� �ٶ󺸴� ����
        float deltaY = Quaternion.Angle(originalRot, targetRot);            // �� ȸ�� ������ ��ü ���� ����

        // ȸ�� ������ �ڱ� �������� ��� ���� Vector3.Angle ��� SignedAngle ���
        Vector3 originalForward = originalRot * Vector3.forward;
        Vector3 targetForward = targetRot * Vector3.forward;
        float signedAngle = Vector3.SignedAngle(originalForward, targetForward, Vector3.up);

        float clampedAngle = Mathf.Clamp(signedAngle, -45f, 45f); // ��30�� ����

        // ���ѵ� ȸ�� ������ŭ ȸ���� ���ο� ���� ���
        Quaternion limitedRot = Quaternion.AngleAxis(clampedAngle, Vector3.up) * originalRot;

        iconSet.transform.rotation = limitedRot;
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
