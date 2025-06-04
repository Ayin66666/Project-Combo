using UnityEngine;


public class Hideout_EventPoint : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && Input_Manager.instance.inputDatas[7].isInput)
        {
            Hideout_Manager.instance.Hideout_Setting(true);
        }
    }
}
