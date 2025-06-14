using UnityEngine;

public class Inventory_Slot_Equipment : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Item_Base item;
    //private Item_Equipment.PartType type;


    public void Equipment(/*Item_Equipment item*/)
    {
        // 플레이어 매니저에 스텟 증감 - 이거 따로 함수 뽑아둘건지?
        
    }

    public void UnEquipment()
    {
        // 플레이어 매니저에 스텟 증강 - 이거 따로 함수 뽑아둘건지?
    }
}
