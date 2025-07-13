using System.Collections;
using UnityEngine;


public class Street_Light : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private LightType lightType;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float minIntensity;
    [SerializeField] private Light[] lightObject;
    private enum LightType { Normal, Flicker }

    [Header("Flicker Settings")]
    [SerializeField] private float flickerDurationMin;
    [SerializeField] private float flickerDurationMax;
    [SerializeField] private float waitBetweenFlickerMin;
    [SerializeField] private float waitBetweenFlickerMax;
    private Coroutine coroutine;


    private void OnEnable()
    {
        switch (lightType)
        {
            case LightType.Normal:
                break;

            case LightType.Flicker:
                if (coroutine != null) StopCoroutine(coroutine);
                coroutine = StartCoroutine(OnOff());
                break;
        }
    }

    private IEnumerator OnOff()
    {
        while(gameObject.activeSelf)
        {
            int flickerCount = Random.Range(2, 6);
            for (int i = 0; i < flickerCount; i++)
            {
                for (int i2 = 0; i2 < lightObject.Length; i2++)
                {
                    lightObject[i2].intensity = Random.Range(minIntensity, maxIntensity * 0.4f);
                }
                yield return new WaitForSeconds(Random.Range(flickerDurationMin, flickerDurationMax));

                // ´Ù½Ã ¹à°Ô
                for (int i2 = 0; i2 < lightObject.Length; i2++)
                {
                    lightObject[1].intensity = maxIntensity;
                }
                yield return new WaitForSeconds(Random.Range(flickerDurationMin, flickerDurationMax));
            }

            yield return new WaitForSeconds(Random.Range(waitBetweenFlickerMin, waitBetweenFlickerMax));
        }
    }
}
