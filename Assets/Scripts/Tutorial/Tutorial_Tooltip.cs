using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Tutorial_Tooltip : MonoBehaviour, IPointerClickHandler
{
    private void OnEnable()
    {
        StartCoroutine(Off());
    }

    private IEnumerator Off()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }
}
