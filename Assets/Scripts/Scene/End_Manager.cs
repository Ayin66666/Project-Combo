using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class End_Manager : MonoBehaviour
{
    /*
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
                SceneLoad_Manager.LoadScene("Chapter 1 - Hideout");
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
    */

    [SerializeField] private Image image;
    [SerializeField] private RectTransform trans;
    [SerializeField] private Text endText;
    [SerializeField] private List<Image> backGround_Image;

    [SerializeField] private float endPos;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float speedUpval;

    [SerializeField] private bool is_BackGround_Image_FadeOn;
    [SerializeField] private bool isScrolling;
    [SerializeField] private bool scrollingEnd;
    [SerializeField] private bool speedUp;
    [SerializeField] private AudioSource audioSource;


    private void Start()
    {
        isScrolling = true;
        scrollingEnd = false;

        // Audio Play
        audioSource.Play();

        StartCoroutine(Sc());
    }

    private void Update()
    {
        if (isScrolling && Input.anyKey && !speedUp)
        {
            speedUp = true;
            scrollSpeed *= speedUpval;
        }
        else
        {
            speedUp = false;
            scrollSpeed = 100;
        }

        if (trans.anchoredPosition.y > endPos && isScrolling)
        {
            StartCoroutine(nameof(EndText));
        }

        if (!isScrolling && scrollingEnd && Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.Stop();
            SceneLoad_Manager.LoadScene("Chapter 1 - Hideout");
        }
    }

    private IEnumerator Sc()
    {
        // Delay
        yield return new WaitForSeconds(3f);

        // Image On
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        image.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);

        // Image Off
        float a = 1;
        while (a > 0)
        {
            a -= 1f * Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, EasingFunctions.InOutElastic(a));
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, a);
        image.gameObject.SetActive(false);

        // Delay
        yield return new WaitForSeconds(1.5f);

        // Move
        while (isScrolling)
        {
            trans.anchoredPosition = new Vector2(0, trans.anchoredPosition.y + scrollSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator EndText()
    {
        endText.gameObject.SetActive(true);
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            endText.color = new Color(endText.color.r, endText.color.g, endText.color.b, a);
            yield return null;
        }

        endText.color = new Color(endText.color.r, endText.color.g, endText.color.b, 1);

        scrollingEnd = true;
        isScrolling = false;
    }
}
