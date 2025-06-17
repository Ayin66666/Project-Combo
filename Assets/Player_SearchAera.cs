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

                // ���� �ڸ� üũ
                (Item_Base item, int count) = drop.Get_Item();
                if (!Player_Manager.instance.inventory.IsFull(item))
                {
                    // ������ �߰�
                    Debug.Log("item add");
                    drop.Item_Add();
                }
            }
        }
    }
}
