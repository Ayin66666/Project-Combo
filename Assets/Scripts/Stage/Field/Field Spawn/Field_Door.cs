using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private ForceFieldController controller;
    [SerializeField] private Collider doorCollider;
    private Coroutine WallCoroutine;


    public void Use(bool isOn)
    {
        if (WallCoroutine != null)
            StopCoroutine(WallCoroutine);

        WallCoroutine = StartCoroutine(isOn ? On() : Off());
    }

    private IEnumerator On()
    {
        doorCollider.enabled = true;
        controller.openCloseProgress = 0;
        float start = 0;
        float end = 2;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            controller.openCloseProgress = Mathf.Lerp(start, end, timer);
            yield return null;
        }
    }

    private IEnumerator Off()
    {
        doorCollider.enabled = false;
        controller.openCloseProgress = 2;
        float start = 2;
        float end = -2;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            controller.openCloseProgress = Mathf.Lerp(start, end, timer);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}