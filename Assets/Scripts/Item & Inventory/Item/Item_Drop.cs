using Easing.Tweening;
using System.Collections;
using UnityEngine;


public class Item_Drop : MonoBehaviour
{
    private Rigidbody rigid;


    [Header("---Item Setting---")]
    [SerializeField] private Item_Base item;
    [SerializeField] private int count;
    [SerializeField] private GameObject pickupVFX;
    [SerializeField] private GameObject[] ratingVFX;
    private bool isMovement;
    private bool isSpawnDelay;


    [Header("---Movement Setting---")]
    [SerializeField] private float curspeed;
    [SerializeField] private Vector2 minMaxSpeed;
    [SerializeField] private float accelerationTime;


    private void OnEnable()
    {
        // ������Ʈ
        rigid = GetComponent<Rigidbody>();
    }


    /// <summary>
    /// ������ ��� �� ȣ�� - ƨ��� ����Ǵ� ȿ��
    /// </summary>
    public void Spawn(Item_Base item, int count)
    {
        if(!isSpawnDelay)
        {
            // ������ ����
            this.item = item;
            this.count = count;

            // ������ ����Ʈ ����
            ratingVFX[(int)item.itemRating].SetActive(true);

            // ���� ȿ�� ����
            StartCoroutine(SpawnCall());
        }
    }

    private IEnumerator SpawnCall()
    {
        isSpawnDelay = true;
        Vector3 moveDir = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f)).normalized;
        rigid.AddForce(moveDir * Random.Range(1f, 3.5f), ForceMode.Impulse);

        yield return new WaitForSeconds(1f);
        isSpawnDelay = false;
    }


    /// <summary>
    /// �̵� ���� - �÷��̾��� ���� ���� ���� ������ ȣ��
    /// </summary>
    /// <returns></returns>
    public void Movement()
    {
        if (!isMovement && !isSpawnDelay)
        {
            isMovement = true;
            StartCoroutine(Item_Movement());
        }
    }

    private IEnumerator Item_Movement()
    {
        rigid.useGravity = false;


        // ���� �ڷ�ƾ
        StartCoroutine(MoveSpeed());

        // �ִ� 10�ʰ� ���� - ���� ����
        GameObject target = Player_Manager.instance.action.gameObject;
        Vector3 moveDir = (target.transform.position - transform.position).normalized;

        float ChaseTimer = 0;
        while (ChaseTimer < 10f)
        {
            moveDir = (target.transform.position - transform.position).normalized;
            transform.position += moveDir * curspeed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveSpeed()
    {
        float timer = 0;
        while (timer < accelerationTime)
        {
            float t = Mathf.Clamp01(timer / accelerationTime);
            curspeed = Mathf.Lerp(minMaxSpeed.x, minMaxSpeed.y, EasingFunctions.OutExpo(t));
            yield return null;
        }

        curspeed = minMaxSpeed.y;
    }


    /// <summary>
    /// ������ ���� �Լ�
    /// </summary>
    /// <returns></returns>
    public (Item_Base item, int itemCount) Get_Item()
    {
        return (item, count);
    }

    /// <summary>
    /// ������ ���� �� �ı� ����
    /// </summary>
    public void DestoryItem()
    {
        pickupVFX.transform.parent = null;
        pickupVFX.SetActive(true);
        Destroy(gameObject);
    }
}
