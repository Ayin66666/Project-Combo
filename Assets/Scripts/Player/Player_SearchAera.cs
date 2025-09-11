using UnityEngine;


public class Player_SearchAera : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DropItem"))
        {
            Item_Drop drop = other.GetComponent<Item_Drop>();
            if (drop != null)
            {
                // 남은 자리 체크
                (Item_Base item, int count) = drop.Get_Item();
                if (Player_Manager.instance.inventory.IsFull(item) == false)
                {
                    // 아이템 추가
                    drop.Item_Add();
                }
                else
                {
                    // 자리가 없다면
                    Debug.Log("자리 없음!");
                }
            }
        }
    }
}
