using UnityEngine;


public class FallChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.position = Vector3.zero;
        }
    }
}
