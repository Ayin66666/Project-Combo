using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamageSysteam
{
    public enum DamageType { Physical, Magical }
    public enum HitVFX { None, KnockBack, Down }

    public void Take_Damage(GameObject attackObj, DamageType type, HitVFX hitType, bool isCirtical, int hitCount, int damage);

    public IEnumerator Hit_KnockBack(GameObject attackObj);

    public IEnumerator Hit_Down(GameObject attackObj);
}
