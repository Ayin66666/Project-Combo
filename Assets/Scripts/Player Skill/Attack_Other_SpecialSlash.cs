using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Attack_Other_SpecialSlash : Attack_Base
{
    [Header("---Special Setting---")]
    [SerializeField] private Transform shotPos;
    [SerializeField] private CinemachineDollyCart dolly;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private GameObject mCam;
    [SerializeField] private GameObject[] effectCams;


    [Header("---Attack VFX---")]
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private GameObject[] auraVFX;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.Collider_Ignore(true);
        PlayerAction_Manager.instance.Animation_Reset();
        PlayerAction_Manager.instance.isAttack = true;

        // ������ ����
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // ��¡
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSpecialSlash", true);
        attackVFX[0].SetActive(true);

        // ����
        Player_Sound.instance.Sound_Speical(Player_Sound.Special.Special_Charge);

        //ī�޶� ��ȯ
        brain.m_DefaultBlend.m_Time = 0;
        mCam.SetActive(false);
        StartCoroutine(Chargning_CameraMovement());
        while (dolly.m_Position < 1)
        {
            yield return null;
        }

        attackVFX[0].SetActive(false);
        attackVFX[1].SetActive(true);
        yield return new WaitForSeconds(0.15f);

        // ��� -> ���߰˱� 2�� ��ȭ 1�� -> ������� -> �齺�� ��˱�
        // ������ʹ� for�� ����ص�?
        for (int i = 0; i < 5; i++)
        {
            // �ִϸ��̼�
            anim.SetTrigger("Smash");
            anim.SetBool("isAttack", true);
            anim.SetBool("isSpecialSlash", true);

            // ���� ���
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        // ī�޶� �ʱ�ȭ
        for (int i = 0; i < effectCams.Length; i++)
        {
            effectCams[i].SetActive(false);
        }
        brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        brain.m_DefaultBlend.m_Time = 1.25f;
        mCam.SetActive(true);

        // ���� ����Ʈ �ʱ�ȭ
        for (int i = 0; i < value_Normal.Count; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        // �ݶ��̴� ����
        PlayerAction_Manager.instance.Collider_Ignore(false);

        // ���� ����
        Player_Manager.instance.status.curAwakening = 0;

        // ���� ����
        PlayerAction_Manager.instance.MovementLock(cancelType, false);
        PlayerAction_Manager.instance.AttackOver();
    }


    #region Camera Movement
    // 1�� - ��¡ ī�޶� ����
    private IEnumerator Chargning_CameraMovement()
    {
        effectCams[0].SetActive(true);
        dolly.m_Position = 0;
        dolly.m_Speed = 0.5f;

        yield return DOTween.To(() => dolly.m_Position, x => dolly.m_Position = x, 1f, 1f)
            .SetEase(Ease.OutExpo)
            .WaitForCompletion();

        effectCams[0].SetActive(false);
        dolly.m_Position = 1;
        dolly.m_Speed = 0;

        effectCams[0].SetActive(false);
        dolly.m_Position = 1;
        dolly.m_Speed = 0;
    }

    // 2�� - ��� ī�޶� ����
    public void CameraChange(int index)
    {
        for (int i = 0; i < effectCams.Length; i++)
        {
            effectCams[i].SetActive(false);
        }

        effectCams[index].SetActive(true);
    }
    #endregion


    #region Body Movement
    public void Body_Movement(int index)
    {
        Debug.Log($"ȣ�� �ε��� (�ʻ�� �� ������Ʈ �̵�) : {index}");
        PlayerAction_Manager.instance.bodyObject.transform.DOKill();
        PlayerAction_Manager.instance.bodyObject.transform.DOMove(moveDatas[index].movePos.position, moveDatas[index].moveSpeed);
    }
    #endregion


    public override void AttackVFX(int index)
    {
        // ����
        switch (index)
        {
            case 2: // ����
                Player_Sound.instance.Sound_Speical(Player_Sound.Special.Special_Jump);
                break;

            case 3:
            case 4:
            case 5: // ���� �˱�
                Player_Sound.instance.Sound_Speical(Player_Sound.Special.Special_SwordAura);
                break;

            case 7: // �������
                Player_Sound.instance.Sound_Speical(Player_Sound.Special.Special_Strike);
                break;

            case 11: // ���� ����
                Player_Sound.instance.Sound_Speical(Player_Sound.Special.Special_BackstepSlash);
                break;
        }


        attackVFX[index].SetActive(true);
    }

    public void SwordAuraVFX(int index)
    {
        // �˱� ��ȯ
        GameObject obj = Instantiate(auraVFX[index], shotPos.position, Quaternion.identity);
        Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

        // ������ ����
        (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
        Skill_Value_SO.Value_Data skillData = value_Normal[index].levelValue.GetData(skillLevel);
        shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

        // �̵� ����
        Vector3 shotDir = shotPos.position - PlayerAction_Manager.instance.bodyObject.transform.position;
        shoot.Movement_Setting(shotDir, 15f, 15f);
    }

    public override void DamageCal(int index)
    {
        (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
        Skill_Value_SO.Value_Data skillData = value_Normal[index].levelValue.GetData(skillLevel);
        value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Player);
    }

    public override void Attack_Reset()
    {
        // �����̶� �ʿ����!!!
    }
}
