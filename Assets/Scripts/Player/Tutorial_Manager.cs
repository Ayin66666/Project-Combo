using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;


public class Tutorial_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private List<TutorialData> data;

    [System.Serializable]
    public struct TutorialData
    {
        public VideoClip cilp;
        [TextArea] public string descrriptionText;
    }


    [Header("---Component---")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private VideoPlayer videoPlayer;


    private void OnEnable()
    {
        videoPlayer.clip = data[0].cilp;
        videoPlayer.Play();
        descriptionText.text = data[0].descrriptionText;
    }

    private void OnDisable()
    {
        videoPlayer.Pause();
    }


    /// <summary>
    /// ��ư Ŭ�� �� Ʃ�丮�� ǥ��
    /// </summary>
    /// <param name="index"></param>
    public void Click_Button(int index)
    {
        videoPlayer.Pause();

        videoPlayer.clip = data[index].cilp;
        videoPlayer.Play();
        descriptionText.text = data[index].descrriptionText;
    }
}
