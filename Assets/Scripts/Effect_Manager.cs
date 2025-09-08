using System.Collections;
using UnityEngine;
using Cinemachine;


public class Effect_Manager : MonoBehaviour
{
    public static Effect_Manager instance;


    [Header("---Component---")]
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shakeCoroutine;
    private Coroutine timeStopCaoroutine;


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

    /// <summary>
    /// 카메라 흔들기
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="time"></param>
    public void Camera_Shack(float intensity, float time)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

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


    /// <summary>
    /// 시간 조절
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="time"></param>
    public void TimeStop(float speed, float time)
    {
        if(timeStopCaoroutine != null)
            StopCoroutine(timeStopCaoroutine);

        timeStopCaoroutine = StartCoroutine(TimeStopCall(speed, time));
    }

    private IEnumerator TimeStopCall(float speed, float time)
    {
        Time.timeScale = time;
        yield return new WaitForSeconds(time);

        float timer = 0;
        while (timer < 1)
        {
            timer += timer += Time.deltaTime;
            Time.timeScale = Mathf.Lerp(speed, 1, timer);
            yield return null;
        }
        Time.timeScale = 1;
    }
}
