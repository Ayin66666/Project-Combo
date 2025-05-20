using System.Collections;
using UnityEngine;
using Easing.Tweening;


public class EAttack_AlterEgo_SwordAura : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private GameObject[] swordAuraVFX;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private Transform shootPos;

    [SerializeField] private GameObject alterEgo_SwordAura;
    [SerializeField] private Transform[] alterEgoSpawnPos;
    [SerializeField] private Transform[] explosionPos;
    private Coroutine spawnCoroutine;
    private Coroutine explosionCoroutine;

    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        for (int i = 0; i < 4; i++)
        {
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[i]);
            Value value = value_Normal[i];
            Skill_Value_SO.Value_Data skillData = value.levelValue.GetData(skillLevel);
            value.attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }

        // 검 이펙트
        ((Enemy_Boss_Arie)enemy).Weapon_Setting(true);

        // 분신 소환
        spawnCoroutine = StartCoroutine(Spawn());

        // 돌진
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isAlterEgoStep", true);
        anim.SetBool("isAlterEgoAttack", true);
        anim.SetBool("isAlterEgo", true);

        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = moveDatas[0].movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveDatas[0].moveSpeed;
            anim.SetFloat("AnimValue", timer);
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isAlterEgoStep", false);


        // 2연타 검기 + 올려베기 + 360도 베기
        enemy.LookAt(enemy.target, 0);
        anim.SetTrigger("Action");
        anim.SetBool("isAlterEgoAttack", true);
        while (anim.GetBool("isAlterEgo"))
        {
            yield return null;
        }

        for (int i = 0; i < 4; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.isPatten = false;
    }

    public void SwordAura(int index)
    {
        // 검기 소환
        GameObject obj = Instantiate(swordAuraVFX[0], shootPos.position, Quaternion.identity);
        Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

        // 바라보기
        Vector3 lookDir = enemy.transform.forward;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir.normalized);
        obj.transform.rotation = lookRotation;

        // 데미지 셋팅
        int targetIndex = (index == 0) ? 4 : 6;
        Value valueData = value_Normal[targetIndex];
        Skill_Value_SO.Value_Data skillData = valueData.levelValue.GetData(skillLevel);

        (bool isCritical, int damage) = enemy.DamageCalculation(valueData);
        shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

        // 이동 셋팅
        Vector3 moveDir = enemy.transform.forward;
        shoot.Movement_Setting(moveDir.normalized, 40, 25f);
    }

    public void Explosion()
    {
        explosionCoroutine = StartCoroutine(ExplosionCall());
    }

    private IEnumerator ExplosionCall()
    {
        for (int i = 0; i < explosionPos.Length; i++)
        {
            // 폭발 소환
            GameObject obj = Instantiate(explosionVFX, explosionPos[i].position, Quaternion.identity);
            Attack_Collider_AOE aoe = obj.GetComponent<Attack_Collider_AOE>();

            // 데미지 셋팅
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[5]);
            Skill_Value_SO.Value_Data skillData = value_Normal[5].levelValue.GetData(skillLevel);
            aoe.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);

            // 폭발 딜레이
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator Spawn()
    {
        for (int i = 0; i < alterEgoSpawnPos.Length; i++)
        {
            GameObject obj = Instantiate(alterEgo_SwordAura, enemy.transform.position, enemy.transform.rotation);
            AlterEgo_SwordAura alterEgo = obj.GetComponent<AlterEgo_SwordAura>();

            // 데미지 계산
            Value valueData = value_Normal[7];
            Skill_Value_SO.Value_Data skillData = valueData.levelValue.GetData(skillLevel);
            (bool isCritical, int damage) = enemy.DamageCalculation(valueData);

            // 분신 데미지 셋팅
            alterEgo.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
            alterEgo.Movement_Setting(alterEgoSpawnPos[i].transform, 1.5f);
            alterEgo.Target_Setting(enemy.target);

            yield return null;
        }
    }

    public override void AttackVFX(int index)
    {
        attackVFX[index].SetActive(true);
    }

    public override void DamageCal(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        if (explosionCoroutine != null)
            StopCoroutine(explosionCoroutine);

        // 이펙트 종료
        ((Enemy_Boss_Arie)enemy).Weapon_Setting(false);
        foreach (GameObject obj in attackVFX)
        {
            obj.SetActive(false);
        }

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
