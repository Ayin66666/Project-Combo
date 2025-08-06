using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class Player_Sound : MonoBehaviour
{
    public static Player_Sound instance;

    [Header("---Component---")]
    [SerializeField] private AudioSource audioSource_Player;
    [SerializeField] private AudioSource audioSource_UI;

    #region Fight
    [Header("---Fight Sound---")]
    [SerializeField] private AudioClip[] playerFight_Normal;
    [SerializeField] private AudioClip[] playerFight_Smash;
    [SerializeField] private AudioClip[] playerFight_Skill;
    [SerializeField] private AudioClip[] playerFight_Special;
    [SerializeField] private AudioClip[] player_Movement;
    [SerializeField] private AudioClip[] hit;

    private Dictionary<Normal, AudioClip> normalSound;
    private Dictionary<Smash, AudioClip> smashSound;
    private Dictionary<Skill, AudioClip> skillSound;
    private Dictionary<Special, AudioClip> specialSound;
    private Dictionary<Movement, AudioClip> movementSound;
    public enum Normal { Normal1, Normal2, Normal3, Normal4 }
    public enum Skill { RushSlash, Counter_Start, Counter_Success, Counter_Add, Awakening }
    public enum Special { Special_Charge, Special_Jump, Special_SwordAura, Special_Strike, Special_BackstepSlash }
    public enum Smash
    {
        Smash1_1, Samsh1_2, Samsh1_3,
        Smash2_1, Smash2_2, Smash2_3,
        Smash3_1, Smash3_2, Smash3_3, Smash3_4,
        Smash4_Charge, Smash4_Slash
    }
    public enum Movement { Move, Dash }
    #endregion

    #region UI
    [Header("---UI Sound---")]
    [SerializeField] private AudioClip[] ingame;
    [SerializeField] private AudioClip[] system;
    private Dictionary<IngameSystem, AudioClip> inGameSystemSound;
    private Dictionary<SystemSound, AudioClip> systemSound;

    public enum IngameSystem { ItemUse, Recovery, Reward, StageClear }
    public enum SystemSound { Click, Out, Save, LoadingEnd }
    #endregion

    [System.Serializable]
    public struct Data
    {
        public AudioSource audio;
        public string name;
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Setting();
    }

    private void Setting()
    {
        // 플레이어 전투
        normalSound = new Dictionary<Normal, AudioClip>();
        for (int i = 0; i < playerFight_Normal.Length; i++)
        {
            normalSound[(Normal)i] = playerFight_Normal[i];
        }

        smashSound = new Dictionary<Smash, AudioClip>();
        for (int i = 0; i < playerFight_Smash.Length; i++)
        {
            smashSound[(Smash)i] = playerFight_Smash[i];
        }

        skillSound = new Dictionary<Skill, AudioClip>();
        for (int i = 0; i < playerFight_Skill.Length; i++)
        {
            skillSound[(Skill)i] = playerFight_Skill[i];
        }

        specialSound = new Dictionary<Special, AudioClip>();
        for (int i = 0; i < playerFight_Special.Length; i++)
        {
            specialSound[(Special)i] = playerFight_Special[i];
        }

        movementSound = new Dictionary<Movement, AudioClip>();
        for (int i = 0; i < player_Movement.Length; i++)
        {
            movementSound[(Movement)i] = player_Movement[i];
        }


        // 시스템 & UI
        inGameSystemSound = new Dictionary<IngameSystem, AudioClip>();
        for (int i = 0; i < ingame.Length; i++)
        {
            inGameSystemSound[(IngameSystem)i] = ingame[i];
        }

        systemSound = new Dictionary<SystemSound, AudioClip>();
        for (int i = 0; i < system.Length; i++)
        {
            systemSound[(SystemSound)i] = system[i];
        }
    }


    #region 플레이어 전투 사운드
    public void Sound_Normal(Normal type)
    {
        Debug.Log($"Sound Call {type}");
        audioSource_Player.PlayOneShot(normalSound[type]);
    }

    public void Sound_Smash(Smash type)
    {
        Debug.Log($"Sound Call {type}");
        audioSource_Player.PlayOneShot(smashSound[type]);
    }

    public void Sound_Skill(Skill type)
    {
        Debug.Log($"Sound Call {type}");
        audioSource_Player.PlayOneShot(skillSound[type]);
    }

    public void Sound_Speical(Special type)
    {
        Debug.Log($"Sound Call {type}");
        //audioSource_Player.PlayOneShot(specialSound[type]);
    }

    public void Sound_Movement(Movement type)
    {
        audioSource_Player.PlayOneShot(movementSound[type]);
    }

    public void Sound_Hit()
    {
        audioSource_Player.PlayOneShot(hit[Random.Range(0, hit.Length)]);
    }

    public void Sound_Walk(bool isOn)
    {
        if (isOn && !audioSource_Player.isPlaying)
        {
            Debug.Log("Call Sound On");
            audioSource_Player.Play();
        }
        else if(!isOn && audioSource_Player.isPlaying)
        {
            Debug.Log("Call Sound off");
            audioSource_Player.Pause();
        }
    }
    #endregion


    #region 플레이어 UI 사운드
    public void Sound_Ingame(IngameSystem type)
    {
        Debug.Log($"Sound Call {type}");
        // audioSource_UI.PlayOneShot(inGameSystemSound[type]);
    }

    public void Sound_System(SystemSound type)
    {
        Debug.Log($"Sound Call {type}");
        audioSource_UI.PlayOneShot(systemSound[type]);
    }
    #endregion
}
