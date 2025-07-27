using System.Collections.Generic;
using UnityEngine;


public class Enemy_Sound : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("---Setting---")]
    [SerializeField] private List<Data> soundData;
    private Dictionary<string, AudioClip> dic = new();
    public enum SoundKey { Hit, Die }

    [System.Serializable]
    private struct Data
    {
        public string key;
        public AudioClip clip;
    }


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < soundData.Count; i++)
        {
            dic.Add(soundData[i].key, soundData[i].clip);
        }
    }

    public void Sound(string key)
    {
        if(dic.TryGetValue(key, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log($"키 에러 발생 {key}");
        }
    }
}
