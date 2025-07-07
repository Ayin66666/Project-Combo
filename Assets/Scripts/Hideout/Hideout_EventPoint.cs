using UnityEngine;


public class Hideout_EventPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Hideout_Manager.instance.Icon_Setting(true);
    }

    private void OnTriggerExit(Collider other)
    {
        Hideout_Manager.instance.Icon_Setting(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && Input_Manager.instance.inputDatas[7].isInput)
        {
            Hideout_Manager.instance.Hideout_Setting(true);
        }
    }
}
