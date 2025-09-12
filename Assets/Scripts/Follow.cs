using UnityEngine;


public class Follow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float yPos;


    private void Update()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y + yPos, target.transform.position.z);
    }
}
