using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class Option_Manager : MonoBehaviour
{
    [Header("---Sound---")]
    private float Master_Volume;
    private float BGM_Volume;
    private float SFX_Volume;

    private bool isMasterOn;
    private bool isBGMOn;
    private bool isSFXOn;
    [SerializeField] private Slider soundSlider_Master;
    [SerializeField] private Slider soundSlider_BGM;
    [SerializeField] private Slider soundSlider_SFX;


    [Header("---Frame rate---")]
    [SerializeField] private int[] flame;
    private Dictionary<int, int> frameDic = new Dictionary<int, int>();
    [SerializeField] private Dropdown dropdown_Framerate;
    [SerializeField] private Dropdown dropdown_Vsync;


    [Header("---Component---")]
    [SerializeField] private AudioSource uiAudio;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioClip[] clips;


    private void Awake()
    {
        // 데이터 셋팅
        for (int i = 0; i < flame.Length; i++)
        {
            frameDic.Add(i, flame[i]);
        }
    }


    #region Start Setting
    /// <summary>
    /// 저장되어 있던 옵션 설정 받아오기?
    /// </summary>
    public void Data_Setting()
    {
        
    }
    #endregion


    #region Sound
    // --- Volume --- //
    public void Setting_Master(float value)
    {
        Master_Volume = Mathf.Log10(value) * 20;
        mixer.SetFloat("Master", isMasterOn ? Master_Volume : 0);
    }

    public void Setting_BGM(float value)
    {
        BGM_Volume = Mathf.Log10(value) * 20;
        mixer.SetFloat("BGM", isBGMOn ? BGM_Volume : 0);
    }

    public void Setting_SFX(float value)
    {
        SFX_Volume = Mathf.Log10(value) * 20;
        mixer.SetFloat("SFX", isSFXOn ? SFX_Volume : 0);
    }


    // --- On Off --- //
    public void OnOff_Master(bool isOn)
    {
        isMasterOn = isOn;
        mixer.SetFloat("Master", isMasterOn ? Master_Volume : 0);
    }

    public void OnOff_BGM(bool isOn)
    {
        isBGMOn = isOn;
        mixer.SetFloat("BGM", isBGMOn ? BGM_Volume : 0);
    }

    public void OnOff_SFX(bool isOn)
    {
        isSFXOn = isOn;
        mixer.SetFloat("SFX", isSFXOn ? SFX_Volume : 0);
    }

    #endregion


    #region Frame
    public void Setting_FPS(int value)
    {
        Application.targetFrameRate = frameDic[value];
    }
    #endregion
}
