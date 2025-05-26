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
        Player_Manager.instance.MovementLock(cancelType, true);
        Player_Manager.instance.isAttack = true;
        Player_Manager.instance.Animation_Reset();

        // 콜라이더 무시
        Player_Manager.instance.Collider_Ignore(true);

        // 차징
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSpecialSlash", true);
        attackVFX[0].SetActive(true);

        // 데미지 셋팅
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        //카메라 전환
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

        // 상승 -> 공중검기 2발 강화 1발 -> 내려찍기 -> 백스탭 대검기
        // 여기부터는 for문 사용해도?
        for (int i = 0; i < 5; i++)
        {
            // 애니메이션
            anim.SetTrigger("Smash");
            anim.SetBool("isAttack", true);
            anim.SetBool("isSpecialSlash", true);

            // 동작 대기
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        // 카메라 초기화
        for (int i = 0; i < effectCams.Length; i++)
        {
            effectCams[i].SetActive(false);
        }
        brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        brain.m_DefaultBlend.m_Time = 1.25f;
        mCam.SetActive(true);

        // 공격 리스트 초기화
        for (int i = 0; i < value_Normal.Count; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        // 콜라이더 무시
        Player_Manager.instance.Collider_Ignore(false);

        // 각성 종료
        Player_Manager.instance.curAwakening = 0;

        // 공격 종료
        Player_Manager.instance.MovementLock(cancelType, false);
        Player_Manager.instance.AttackOver();
    }


    #region Camera Movement
    // 1번 - 차징 카메라 무브
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

    // 2번 - 상승 카메라 무브
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
        Debug.Log($"호출 인덱스 (필살기 몸 오브젝트 이동) : {index}");
        Player_Manager.instance.bodyObject.transform.DOKill();
        Player_Manager.instance.bodyObject.transform.DOMove(moveDatas[index].movePos.position, moveDatas[index].moveSpeed);
    }
    #endregion


    public override void AttackVFX(int index)
    {
        attackVFX[index].SetActive(true);
    }

    public void SwordAuraVFX(int index)
    {
        // 검기 소환
        GameObject obj = Instantiate(auraVFX[index], shotPos.position, Quaternion.identity);
        Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

        // 데미지 셋팅
        (bool isCritical, int damage) = Player_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
        Skill_Value_SO.Value_Data skillData = value_Normal[index].levelValue.GetData(skillLevel);
        shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

        // 이동 셋팅
        Vector3 shotDir = shotPos.position - Player_Manager.instance.bodyObject.transform.position;
        shoot.Movement_Setting(shotDir, 15f, 15f);
    }

    public override void DamageCal(int index)
    {
        (bool isCritical, int damage) = Player_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
        Skill_Value_SO.Value_Data skillData = value_Normal[index].levelValue.GetData(skillLevel);
        value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
    }

    public override void Attack_Reset()
    {
        // 무적이라 필요없음!!!
    }
}
