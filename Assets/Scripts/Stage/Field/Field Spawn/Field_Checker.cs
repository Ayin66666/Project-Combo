using UnityEngine;


public class Field_Checker : MonoBehaviour
{
    [Header("---Settting---")]
    [SerializeField] private Field_Base field;
    private Collider checkCollider;


    private void Awake()
    {
        checkCollider = GetComponent<Collider>();
    }

    public void Reset_Checker()
    {
        checkCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            field.Field_Start();
            checkCollider.enabled = false;
        }
    }
}
