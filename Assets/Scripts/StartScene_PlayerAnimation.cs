using System.Collections;
using UnityEngine;


public class StartScene_PlayerAnimation : MonoBehaviour
{
    [Header("---Settting---")]
    [SerializeField] private Light pointLight;
    [SerializeField] private GameObject effectObj;
    [SerializeField] private GameObject offEffect;
    private Animator anim;
    private Coroutine lightCoroutine;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void Light_Setting(int index)
    {
        bool isOn = index == 0 ? true : false;

        if (lightCoroutine != null) StopCoroutine(lightCoroutine);
        lightCoroutine = StartCoroutine(isOn ? Light() : Off());
    }

    private IEnumerator Light()
    {
        // ¿Ã∆Â∆Æ OnOff
        effectObj.SetActive(true);

        pointLight.intensity = 0;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 2f;
            pointLight.intensity = Mathf.Lerp(0, 1, timer);
            yield return null;
        }
        pointLight.intensity = 1;
    }

    private IEnumerator Off()
    {
        effectObj.SetActive(false);
        offEffect.SetActive(true);

        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 2.5f;
            pointLight.intensity = Mathf.Lerp(1, 0, timer);
            yield return null;
        }
        pointLight.intensity = 0;
    }
}
