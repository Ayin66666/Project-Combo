using System.Collections;
using UnityEngine;
using TMPro;
using Easing.Tweening;
using Unity.VisualScripting;


public class Obejct_Elevator : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Movement curMovement;
    [SerializeField] private bool isUsed;
    [SerializeField] private bool isPlayerIn;

    private enum Movement { AtoB, BtoA }


    [Header("---Player Movement---")]
    private Vector3 lastPosition;
    public Vector3 DeltaMovement { get; private set; }


    [Header("---Component---")]
    [SerializeField] private GameObject uiSet;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Transform[] movePos;
    private Collider coll;


    private void Awake()
    {
        coll = GetComponent<Collider>();
    }


    public void Use()
    {
        if(isUsed)
        {
            return;
        }

        StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;
        PlayerAction_Manager.instance.transform.parent = transform;

        // �ؽ�Ʈ ����
        text.text = "���������� �⵿";

        // �̵�
        float timer = 0;
        Vector3 start = movePos[curMovement == Movement.AtoB ? 0 : 1].position;
        Vector3 end = movePos[curMovement == Movement.AtoB ? 1 : 0].position;
        Vector3 lastPos = transform.position;
        while (timer < 1)
        {
            // ���������� �̵�
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(movePos[curMovement == Movement.AtoB ? 0 : 1].position, 
                movePos[curMovement == Movement.AtoB ? 1 : 0].position, timer);

            // �÷��̾� �̵�
            Vector3 newPos = Vector3.Lerp(start, end, timer);
            Vector3 delta = newPos - lastPos;

            transform.position = newPos;
            lastPos = newPos;

            // �÷��̾ ���� �ִٸ� ���� �̵�
            if (isPlayerIn)
            {
                PlayerAction_Manager.instance.RideMovement(delta);
            }


            yield return null;
        }
        transform.position = movePos[curMovement == Movement.AtoB ? 1 : 0].position;
        PlayerAction_Manager.instance.transform.parent = null;

        // �ؽ�Ʈ ����
        text.text = "�⵿�Ϸ�";
        yield return new WaitForSeconds(0.1f);

        curMovement = curMovement == Movement.AtoB ? Movement.BtoA : Movement.AtoB;
        coll.enabled = true;
        isUsed = false;

        text.text = "���������� Ȱ��ȭ";
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;

            // �߷� ��Ȱ��ȭ - ������ ���� ����
            PlayerAction_Manager.instance.useGravity = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� ȣ��
            if(Input.GetKeyDown(KeyCode.F) && !isUsed)
            {
                coll.enabled = false;
                Use();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;

            // �߷� Ȱ��ȭ - ������ ���� ����
            PlayerAction_Manager.instance.useGravity = true;
        }
    }
}
