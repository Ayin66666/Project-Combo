using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Skill_Node : MonoBehaviour
{
    #region 프로퍼티
    public int CurLevel
    {
        get { return curLevel; }
        private set { curLevel = value; }
    }
    public int MaxLevel
    {
        get { return maxLevel; }
        private set { maxLevel = value; }
    }
    #endregion

    [Header("---Setting---")]
    [SerializeField] private int curLevel;
    [SerializeField] private int maxLevel;
    public bool isLearn;
    public System.Action skillAction;


    [Header("---Compoenet---")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI levelText;


    /// <summary>
    /// 스킬 노드 클릭
    /// </summary>
    public void Use()
    {

    }
}
