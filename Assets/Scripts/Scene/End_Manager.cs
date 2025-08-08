using Easing.Tweening;
using System.Collections;
using UnityEngine;


public class End_Manager : MonoBehaviour
{
    [Header("---Component---")]
    [SerializeField] private CanvasGroup image;
    [SerializeField] private RectTransform trans;
    [SerializeField] private CanvasGroup endText;
    [SerializeField] private AudioSource audioS;


    [Header("---Setting---")]
    [SerializeField] private float endPos;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float speedUpval;
    [SerializeField] private bool speedUp;


    private void Start()
    {
        // Audio Play
        audioS.Play();

        StartCoroutine(Scroll());
    }

    private void PlayerInput()
    {
        if (Input.anyKey && !speedUp)
        {
            speedUp = true;
            scrollSpeed *= speedUpval;
        }
        else
        {
            speedUp = false;
            scrollSpeed = 300;
        }
    }

    private IEnumerator Scroll()
    {
        // Delay
        yield return new WaitForSeconds(3f);

        // Image
        image.alpha = 1;
        image.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        float a = 1;
        while (a > 0)
        {
            a -= 1f * Time.deltaTime;
            image.alpha = EasingFunctions.InOutElastic(a);
            yield return null;
        }
        image.alpha = 0;
        image.gameObject.SetActive(false);

        // Scroll Move
        while (trans.anchoredPosition.y > endPos)
        {
            PlayerInput();

            trans.anchoredPosition = new Vector2(0, trans.anchoredPosition.y + scrollSpeed * Time.deltaTime);
            yield return null;
        }

        // 인풋 텍스트 표기
        StartCoroutine(nameof(EndText));

        // 입력 대기
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                audioS.Stop();
                SceneLoad_Manager.LoadScene("Start_Scene");
            }

            yield return null;
        }
    }

    private IEnumerator EndText()
    {
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            endText.alpha = a;
            yield return null;
        }

        endText.alpha = 1;
    }
}
