using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private TMP_Dropdown dropdown_Framerate;
    [SerializeField] private TMP_Dropdown dropdown_Vsync;
    private bool isVsync;
    private int flameIndex;


    [Header("---Component---")]
    [SerializeField] private AudioMixer mixer;


    private void Awake()
    {
        // 데이터 셋팅
        for (int i = 0; i < flame.Length; i++)
        {
            frameDic.Add(i, flame[i]);
        }

        soundSlider_Master.onValueChanged.AddListener(Setting_Master);
        soundSlider_BGM.onValueChanged.AddListener(Setting_BGM);
        soundSlider_SFX.onValueChanged.AddListener(Setting_SFX);

        dropdown_Framerate.value = 1;
    }


    #region Sound
    // --- Volume --- //
    public void Setting_Master(float value)
    {
        Master_Volume = Mathf.Log10(value) * 20;
        mixer.SetFloat("Master", Mathf.Log10(isMasterOn ? value : 0.0001f) * 20);
    }

    public void Setting_BGM(float value)
    {
        BGM_Volume = Mathf.Log10(value) * 20;
        mixer.SetFloat("BGM", Mathf.Log10(isMasterOn ? value : 0.0001f) * 20);
    }

    public void Setting_SFX(float value)
    {
        SFX_Volume = Mathf.Log10(value) * 20;
        mixer.SetFloat("SFX", Mathf.Log10(isMasterOn ? value : 0.0001f) * 20);
    }


    // --- On Off --- //
    public void OnOff_Master(bool isOn)
    {
        isMasterOn = isOn;
        mixer.SetFloat("Master", Mathf.Log10(isMasterOn ? Master_Volume : 0.0001f) * 20);
    }

    public void OnOff_BGM(bool isOn)
    {
        isBGMOn = isOn;
        mixer.SetFloat("BGM", Mathf.Log10(isBGMOn ? BGM_Volume : 0.0001f) * 20);
    }

    public void OnOff_SFX(bool isOn)
    {
        isSFXOn = isOn;
        mixer.SetFloat("SFX", Mathf.Log10(isSFXOn ? SFX_Volume : 0.0001f) * 20);
    }
    #endregion


    #region Frame
    /// <summary>
    /// 프레임 제한
    /// </summary>
    /// <param name="value"></param>
    public void Setting_FPS(int value)
    {
        Debug.Log($"{value}, {frameDic[value]}, {isVsync}");
        flameIndex = value;
        Application.targetFrameRate = isVsync ? -1 : frameDic[flameIndex];
    }

    /// <summary>
    /// 수직 동기화
    /// </summary>
    /// <param name="isOn"></param>
    public void Setting_VSync(int value)
    {
        isVsync = value == 1 ? true : false;
        QualitySettings.vSyncCount = value;
        Debug.Log(QualitySettings.vSyncCount);
    }
    #endregion
}
