using UnityEngine;

public class Player_SearchAera : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DropItem"))
        {
            Debug.Log(other);
            Item_Drop drop = other.GetComponent<Item_Drop>();
            Debug.Log(drop);

            if (drop != null)
            {
                Debug.Log("add Check");

                // 남은 자리 체크
                (Item_Base item, int count) = drop.Get_Item();
                if (!Player_Manager.instance.inventory.IsFull(item))
                {
                    // 아이템 추가
                    Debug.Log("item add");
                    drop.Item_Add();
                }
            }
        }
    }
}
