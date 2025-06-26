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
    private bool isSpawnDelay;


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
    /// ������ ���� / �ı�
    /// </summary>
    public void Item_Add()
    {
        // ������ ����
        Player_Manager.instance.inventory.Item_Add(item, count);

        // ��� ������ �ı�
        pickupVFX.transform.parent = null;
        pickupVFX.SetActive(true);
        Destroy(gameObject);
    }

    /// <summary>
    /// ������ ������ ����
    /// </summary>
    /// <returns></returns>
    public (Item_Base item, int itemCount) Get_Item()
    {
        return (item, count);
    }
}
