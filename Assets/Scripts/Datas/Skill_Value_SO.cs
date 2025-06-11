using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Object/Skill Data", order = int.MaxValue)]
public class Skill_Value_SO : ScriptableObject
{
    [Header("---Status---")]
    public List<Value_Data> value_List;

    [System.Serializable]
    public struct Value_Data
    {
        [SerializeField] private string name;
        public IDamageSysteam.DamageType type;
        public IDamageSysteam.HitVFX attackEffect;
        public Vector2 motionValue;
        public int hitCount;
    }


    public Value_Data GetData(int skillLevel)
    {
        if (value_List == null || skillLevel < 0 || skillLevel >= value_List.Count)
        {
            Debug.LogWarning("Skill level is out of range or data is null.");
            return default;
        }

        return value_List[skillLevel]; // 여러 타수면 이 index도 인자로 받을 수 있음
    }
}
