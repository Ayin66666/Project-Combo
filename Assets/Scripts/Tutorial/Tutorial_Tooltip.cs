using UnityEngine;
using UnityEngine.EventSystems;

public class Tutorial_Tooltip : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }
}
