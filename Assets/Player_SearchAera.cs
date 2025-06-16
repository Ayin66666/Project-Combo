using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_SearchAera : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DropItem"))
        {
            Item_Drop drop = other.GetComponent<Item_Drop>();
            (Item_Base item, int count) = drop.Get_Item();

            if (!Player_Manager.instance.inventory.IsFull(item))
            {
                drop.Movement();
            }
        }
    }
}
