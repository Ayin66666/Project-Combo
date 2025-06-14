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

    [Header("---Movement Setting---")]
    [SerializeField] private float curspeed;
    [SerializeField] private Vector2 minMaxSpeed;
    [SerializeField] private float accelerationTime;


    private void OnEnable()
    {
        // 컴포넌트
        rigid = GetComponent<Rigidbody>();

        // 아이템 이펙트 셋팅
        ratingVFX[(int)item.itemRating].SetActive(true);
    }


    /// <summary>
    /// 아이테 드랍 시 호출 - 튕기듯 드랍되는 효과
    /// </summary>
    public void Spawn()
    {
        Vector3 moveDir = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f)).normalized;
        rigid.AddForce(moveDir * Random.Range(1f, 3.5f), ForceMode.Impulse);
    }

    /// <summary>
    /// 이동 로직 - 플레이어의 감지 범위 내로 들어오면 호출
    /// </summary>
    /// <returns></returns>
    public void Movement()
    {
        if (!isMovement)
        {
            isMovement = true;
            StartCoroutine(Item_Movement());
        }
    }

    private IEnumerator Item_Movement()
    {
        // 가속 코루틴
        StartCoroutine(MoveSpeed());

        // 최대 10초간 추적 - 이후 습득
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
    /// 아이템 전달 함수 - 호출시 아이템 파괴
    /// </summary>
    /// <returns></returns>
    public (Item_Base item, int itemCount) Get_Item()
    {
        return (item, count);
        Destroy(gameObject);
    }

    /// <summary>
    /// 아이템 습득 시 파괴 로직
    /// </summary>
    public void DestoryItem()
    {
        pickupVFX.transform.parent = null;
        pickupVFX.SetActive(true);
        Destroy(gameObject);
    }
}
