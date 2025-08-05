using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Tutorial_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private List<GameObject> tutorials;

    [System.Serializable]
    public struct TutorialData
    {
        public VideoClip cilp;
        [TextArea] public string descrriptionText;
    }

    /// <summary>
    /// ��ư Ŭ�� �� Ʃ�丮�� ǥ��
    /// </summary>
    /// <param name="index"></param>
    public void Click_Button(int index)
    {
        foreach (var tutorial in tutorials)
        {
            tutorial.SetActive(false);
        }

        tutorials[index].SetActive(true);
    }
}
