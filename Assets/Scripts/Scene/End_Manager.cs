using Easing.Tweening;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class End_Manager : MonoBehaviour
{
    [Header("---Component---")]
    [SerializeField] private Image image;
    [SerializeField] private RectTransform trans;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private AudioSource audio;


    [Header("---Setting---")]
    [SerializeField] private float endPos;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float speedUpval;
    [SerializeField] private bool isScrolling;
    [SerializeField] private bool scrollingEnd;
    [SerializeField] private bool speedUp;


    private void Start()
    {
        isScrolling = true;
        scrollingEnd = false;

        // Audio Play
        audio.Play();

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
            audio.Stop();
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
