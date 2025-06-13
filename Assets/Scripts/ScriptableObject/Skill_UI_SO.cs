using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


[CreateAssetMenu(fileName = "Skill UI Data", menuName = "Scriptable Object/Skill UI Data", order = int.MaxValue)]
public class Skill_UI_SO : ScriptableObject
{
    [Header("---Description---")]
    [SerializeField] private Sprite iconImage;
    [SerializeField] private VideoClip clip;
    [SerializeField] private string skillName;
    [SerializeField, TextArea] private string skillDescription;


    #region Property
    public string SkillName
    {
        get { return skillName; }
        private set { skillName = value; }

    }

    public string SkillDescription
    {
        get { return skillDescription; }
        private set { skillDescription = value; }
    }

    public Sprite Icon
    {
        get { return iconImage; }
        private set { iconImage = value; }
    }

    public VideoClip SkillClip
    {
        get { return clip; }
        private set { clip = value; }
    }
    #endregion
}
