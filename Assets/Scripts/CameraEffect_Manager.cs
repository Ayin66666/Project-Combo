using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Easing.Tweening;


public class CameraEffect_Manager : MonoBehaviour
{
    public static CameraEffect_Manager instance;


    [Header("---Component---")]
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shakeCoroutine;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }


    public void Camera_Shack(float intensity, float time)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(Turn_CameraShake(intensity, time));
    }

    private IEnumerator Turn_CameraShake(float intensity, float time)
    {
        // 흔들림 초기화
        noise.m_AmplitudeGain = 0;

        // 카메라 흔들기
        float power = intensity;
        float timer = time;
        while (power > 0)
        {
            power -= Time.deltaTime / timer;
            noise.m_AmplitudeGain = power;
            yield return null;
        }

        // 흔들림 초기화
        noise.m_AmplitudeGain = 0;
    }
}
