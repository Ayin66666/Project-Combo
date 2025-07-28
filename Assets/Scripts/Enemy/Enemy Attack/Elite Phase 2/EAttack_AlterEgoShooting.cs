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

        // ����
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.Backstep_Move.ToString());

        // �齺��
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

        // ����
        Instantiate(StealthVFX, enemy.transform.position, Quaternion.identity);
        Body(false);

        // �н� 4ȸ ��ȯ - ��������
        for (int i = 0; i < alterSpawnPos.Length; i++)
        {
            // �н� ��ȯ
            GameObject obj = Instantiate(alterObj, alterSpawnPos[i].position, Quaternion.identity);

            // ���� ����
            Enemy_AlterEgo alter = obj.GetComponent<Enemy_AlterEgo>();
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
            Skill_Value_SO.Value_Data skillData = value_Normal[1].levelValue.GetData(skillLevel);
            alter.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, enemy.target);
            alter.Attack();

            yield return new WaitForSeconds(0.35f);
        }

        // ���� ����
        Instantiate(StealthVFX, enemy.transform.position, Quaternion.identity);
        Body(true);

        yield return new WaitForSeconds(0.05f);

        // ����
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.Forward_Move.ToString());

        // ���� �̵�
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

        // ����
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.AlterEgo_Charge.ToString());

        // ��ȭ ��� - ��¡
        anim.SetTrigger("Action");
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            enemy.LookAt(enemy.target, 0);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);


        // ��ȭ ��� - ���
        anim.SetTrigger("Action");
        anim.SetBool("isAlterStep", false);
        while (anim.GetBool("isAlterAttack"))
        {
            yield return null;
        }
        
        // �ݶ��̴� ����
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
        // 1�� - ���� ������
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[1]);
        Skill_Value_SO.Value_Data skillData = value_Normal[1].levelValue.GetData(skillLevel);
        value_Normal[1].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Enemy);

        // 2�� - �߰� ������
        (isCritical, damage) = enemy.DamageCalculation(value_Normal[2]);
        skillData = value_Normal[2].levelValue.GetData(skillLevel);
        value_Normal[2].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Enemy);

        // 3�� - ���� ������
        (isCritical, damage) = enemy.DamageCalculation(value_Normal[3]);
        skillData = value_Normal[3].levelValue.GetData(skillLevel);
        value_Normal[3].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Enemy);

        // 1�� �ݶ��̴�
        GameObject obj = Instantiate(bullet[0], shootPos[0].position, Quaternion.identity);
        value_Normal[1].attackCollider.AttackColliderOn(0);
        yield return new WaitForSeconds(0.075f);

        // 2 3 4�� �ݶ��̴�
        for (int i = 1; i < 4; i++)
        {
            // ����
            elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.AlterEgo_Shoot.ToString());

            // ����Ʈ ��ȯ
            Instantiate(bullet[1], shootPos[i].position, Quaternion.identity);

            // �ݶ��̴� OnOff
            value_Normal[2].attackCollider.ListReset();
            value_Normal[2].attackCollider.AttackColliderOn(i - 1);

            // ������
            yield return new WaitForSeconds(0.075f);
        }

        // 5�� �ݶ��̴�
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
        // ���� ����
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        if (shootCoroutine != null)
            StopCoroutine(shootCoroutine);

        // ����Ʈ ����
        chargeVFX.SetActive(false);
        StealthVFX.SetActive(false);

        // ���� ����
        Body(true);

        // ����Ʈ ����
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
