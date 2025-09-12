using UnityEngine;


public class Stage_FallChacker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.position = Stage_Manager.instance.spawnPos;
        }
    }
}
