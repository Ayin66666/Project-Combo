using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.AI;


public class Item_Drop : MonoBehaviour
{
    private Rigidbody rigid;


    [Header("---Item Setting---")]
    [SerializeField] private Item_Base item;
    [SerializeField] private int count;
    [SerializeField] private GameObject pickupVFX;
    [SerializeField] private GameObject[] ratingVFX;
    [SerializeField] private LayerMask ground;
    private bool isSpawnDelay;


    private void OnEnable()
    {
        // 컴포넌트
        rigid = GetComponent<Rigidbody>();
    }


    /// <summary>
    /// 아이테 드랍 시 호출 - 튕기듯 드랍되는 효과
    /// </summary>
    public void Spawn(Item_Base item, int count)
    {
        if(!isSpawnDelay)
        {
            // 아이템 셋팅
            this.item = item;
            this.count = count;

            // 아이템 이펙트 셋팅
            ratingVFX[(int)item.itemRating].SetActive(true);

            // 스폰 효과 셋팅
            StartCoroutine(SpawnCall());
        }
    }

    private IEnumerator SpawnCall()
    {
        // 아이템 드랍 이동
        isSpawnDelay = true;
        Vector3 moveDir = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f)).normalized;
        rigid.AddForce(moveDir * Random.Range(1f, 3.5f), ForceMode.Impulse);

        // 대기
        yield return new WaitForSeconds(10f);

        // 아이템 파괴
        pickupVFX.transform.parent = null;
        pickupVFX.SetActive(true);
        Destroy(gameObject);
    }

    /// <summary>
    /// 아이템 습득 / 파괴
    /// </summary>
    public void Item_Add()
    {
        // 아이템 습득
        Player_Manager.instance.inventory.Item_Add(item, count);

        // 드롭 아이템 파괴
        pickupVFX.transform.parent = null;
        pickupVFX.SetActive(true);
        Destroy(gameObject);
    }

    /// <summary>
    /// 아이템 데이터 전달
    /// </summary>
    /// <returns></returns>
    public (Item_Base item, int itemCount) Get_Item()
    {
        return (item, count);
    }
}
