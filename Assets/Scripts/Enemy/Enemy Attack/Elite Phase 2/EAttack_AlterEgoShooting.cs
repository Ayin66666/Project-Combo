using Easing.Tweening;
using System.Collections;
using UnityEngine;


public class EAttack_AlterEgoShooting : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Elite_Phase2 elite;
    [SerializeField] private Transform backstepPos;
    [SerializeField] private Transform forwardstepPos;

    [SerializeField] private GameObject alterObj;
    [SerializeField] private Transform[] alterSpawnPos;

    [SerializeField] private GameObject StealthVFX;
    [SerializeField] private GameObject[] body;

    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject[] bullet;

    [SerializeField] private Transform[] shootPos;
    private Coroutine shootCoroutine;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        // 사운드
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.Backstep_Move.ToString());

        // 백스탭
        anim.SetTrigger("Action");
        anim.SetBool("isAlterStep", true);
        anim.SetBool("isAlterAttack", true);
        anim.SetFloat("AnimValue", 0);
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = backstepPos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            anim.SetFloat("AnimValue", EasingFunctions.OutExpo(timer));
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isAlterStep", false);

        // 은신
        Instantiate(StealthVFX, enemy.transform.position, Quaternion.identity);
        Body(false);

        // 분신 4회 소환 - 스나이핑
        for (int i = 0; i < alterSpawnPos.Length; i++)
        {
            // 분신 소환
            GameObject obj = Instantiate(alterObj, alterSpawnPos[i].position, Quaternion.identity);

            // 공격 지시
            Enemy_AlterEgo alter = obj.GetComponent<Enemy_AlterEgo>();
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
            Skill_Value_SO.Value_Data skillData = value_Normal[1].levelValue.GetData(skillLevel);
            alter.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, enemy.target);
            alter.Attack();

            yield return new WaitForSeconds(0.35f);
        }

        // 은신 해제
        Instantiate(StealthVFX, enemy.transform.position, Quaternion.identity);
        Body(true);

        yield return new WaitForSeconds(0.05f);

        // 사운드
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.Forward_Move.ToString());

        // 전진 이동
        anim.SetTrigger("Action");
        anim.SetBool("isAlterStep", true);
        anim.SetFloat("AnimValue", 0);
        startPos = enemy.transform.position;
        endPos = forwardstepPos.position;
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            anim.SetFloat("AnimValue", timer);
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isAlterStep", false);

        // 사운드
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.AlterEgo_Charge.ToString());

        // 강화 사격 - 차징
        anim.SetTrigger("Action");
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            enemy.LookAt(enemy.target, 0);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);


        // 강화 사격 - 사격
        anim.SetTrigger("Action");
        anim.SetBool("isAlterStep", false);
        while (anim.GetBool("isAlterAttack"))
        {
            yield return null;
        }
        
        // 콜라이더 리셋
        for (int i = 1; i < value_Normal.Count; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.isPatten = false;
    }

    private void Body(bool isOn)
    {
        for (int i = 0; i < body.Length; i++)
        {
            body[i].SetActive(isOn);
        }
    }

    public void Shoot()
    {
        shootCoroutine = StartCoroutine(ShootVFX());
    }

    private IEnumerator ShootVFX()
    {
        // 1번 - 시작 데미지
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[1]);
        Skill_Value_SO.Value_Data skillData = value_Normal[1].levelValue.GetData(skillLevel);
        value_Normal[1].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Enemy);

        // 2번 - 중간 데미지
        (isCritical, damage) = enemy.DamageCalculation(value_Normal[2]);
        skillData = value_Normal[2].levelValue.GetData(skillLevel);
        value_Normal[2].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Enemy);

        // 3번 - 최종 데미지
        (isCritical, damage) = enemy.DamageCalculation(value_Normal[3]);
        skillData = value_Normal[3].levelValue.GetData(skillLevel);
        value_Normal[3].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Enemy);

        // 1번 콜라이더
        GameObject obj = Instantiate(bullet[0], shootPos[0].position, Quaternion.identity);
        value_Normal[1].attackCollider.AttackColliderOn(0);
        yield return new WaitForSeconds(0.075f);

        // 2 3 4번 콜라이더
        for (int i = 1; i < 4; i++)
        {
            // 사운드
            elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.AlterEgo_Shoot.ToString());

            // 이펙트 소환
            Instantiate(bullet[1], shootPos[i].position, Quaternion.identity);

            // 콜라이더 OnOff
            value_Normal[2].attackCollider.ListReset();
            value_Normal[2].attackCollider.AttackColliderOn(i - 1);

            // 딜레이
            yield return new WaitForSeconds(0.075f);
        }

        // 5번 콜라이더
        obj = Instantiate(bullet[0], shootPos[4].position, Quaternion.identity);
        value_Normal[3].attackCollider.AttackColliderOn(0);
    }

    public void ChargeVFX(int index)
    {
        chargeVFX.SetActive(index == 0);
    }

    public override void AttackVFX(int index)
    {
        throw new System.NotImplementedException();
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

        if (shootCoroutine != null)
            StopCoroutine(shootCoroutine);

        // 이펙트 종료
        chargeVFX.SetActive(false);
        StealthVFX.SetActive(false);

        // 은신 종료
        Body(true);

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
