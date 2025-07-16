using UnityEngine;


public class Street_Light : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Type type;
    [SerializeField] private State currentState;
    [SerializeField] private float targetDuration;
    [SerializeField] private float minIntensity = 0.3f;
    [SerializeField] private float maxIntensity = 1.0f;
    [SerializeField] private float flickerDuration = 0.1f;
    [SerializeField] private float waitDuration = 1.5f;
    [SerializeField] private Light[] lights;
    private float timer;

    private enum Type { None, Flicker }
    private enum State { Waiting, Flickering }


    private void Start()
    {
        switch (type)
        {
            case Type.None:
                break;

            case Type.Flicker:
                ResetWait();
                break;
        }

    }

    private void FixedUpdate()
    {
        switch (type)
        {
            case Type.None:
                break;

            case Type.Flicker:
                FMovement();
                break;
        }
    }

    private void FMovement()
    {
        timer += Time.deltaTime;

        if (timer >= targetDuration)
        {
            if (currentState == State.Waiting)
            {
                // Flicker ����
                float newIntensity = Random.Range(minIntensity, maxIntensity * 0.4f);
                foreach (var light in lights)
                    light.intensity = newIntensity;

                currentState = State.Flickering;
                targetDuration = flickerDuration;
            }
            else
            {
                // �ٽ� ���
                foreach (var light in lights)
                    light.intensity = maxIntensity;

                ResetWait();
            }

            timer = 0f;
        }
    }

    private void ResetWait()
    {
        currentState = State.Waiting;
        targetDuration = Random.Range(waitDuration, waitDuration + 1f);
    }
}
