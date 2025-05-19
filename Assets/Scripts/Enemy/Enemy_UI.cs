using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using DG.Tweening;


public class Enemy_UI : MonoBehaviour
{
    [Header("---State---")]
    [SerializeField] private Type type;
    public bool isCutScene;
    private enum Type { Normal, Boss }


    [Header("---Hp & Groggy---")]
    [SerializeField] private GameObject satusSet;
    [SerializeField] private Slider[] hpSlider;
    [SerializeField] private Slider[] groggySlider;
    private Coroutine hpCoroutine;
    private Coroutine groggyCoroutuine;
    private Coroutine CutSceneCoroutine;


    [Header("---CutScene---")]
    [SerializeField] private GameObject cutSceneSet;


    [Header("---Name---")]
    [SerializeField] private GameObject nameSet;
    [SerializeField] private Image nameFadeImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI styleText;

    [Header("---Component---")]
    [SerializeField] private Enemy_Base enemy;
    [SerializeField] private VideoPlayer video;
    [SerializeField] private GameObject damageUI;
    [SerializeField] private Collider damagePosCollider;

    [Header("---LockOn---")]
    [SerializeField] private GameObject lockOnSet;


    private void Start()
    {
        if (type == Type.Normal)
        {
            StartCoroutine(LookAt());
        }
    }


    public void UI_OnOff(bool isOn)
    {
        satusSet.SetActive(isOn);
    }

    public void UI_Setting()
    {
        nameText.text = enemy.statusData.ObjectName;

        hpSlider[0].maxValue = enemy.maxHp;
        hpSlider[0].value = enemy.maxHp;

        hpSlider[1].maxValue = enemy.curHp;
        hpSlider[1].value = enemy.maxHp;

        groggySlider[0].maxValue = enemy.maxHp;
        groggySlider[0].value = enemy.maxHp;

        groggySlider[1].maxValue = enemy.curGroggy;
        groggySlider[1].value = enemy.curGroggy;
    }

    private IEnumerator LookAt()
    {
        Vector3 lookDir = Vector3.zero;
        while (true)
        {
            lookDir = Player_Manager.instance.cam.transform.position - transform.position;
            lookDir.y = 0;
            transform.rotation = Quaternion.LookRotation(-lookDir);
            yield return null;
        }
    }

    public void LockOn(bool isOn)
    {
        lockOnSet.SetActive(isOn);
    }


    #region Boss UI
    public void CutScene(VideoClip clip)
    {
        if (CutSceneCoroutine != null)
            StopCoroutine(CutSceneCoroutine);

        CutSceneCoroutine = StartCoroutine(CutSceneCall(clip));
    }

    private IEnumerator CutSceneCall(VideoClip clip)
    {
        isCutScene = true;
        cutSceneSet.SetActive(true);

        video.clip = clip;
        video.Play();

        yield return new WaitForSeconds(1f);
        while (video.isPlaying)
        {
            yield return null;
        }

        cutSceneSet.SetActive(false);
        isCutScene = false;
    }


    public void EnemyName()
    {
        StartCoroutine(EnemyNameCall());
    }

    private IEnumerator EnemyNameCall()
    {
        nameSet.SetActive(true);
        nameText.text = enemy.statusData.ObjectName;

        // 가장자리 페이드 인
        Tween fadeTween = nameFadeImage.DOFade(1, 1f);
        yield return fadeTween.WaitForCompletion();

        // 대기
        yield return new WaitForSeconds(0.15f);

        // 이름 페이드 인
        Tween nameFadeTween = nameText.DOFade(1, 1f);
        yield return nameFadeTween.WaitForCompletion();

        // 가장자리 & 이름 페이드 아웃
        yield return DOTween.Sequence()
            .Join(nameFadeImage.DOFade(0, 1f))
            .Join(nameText.DOFade(0, 1f))
            .WaitForCompletion();
    }
    #endregion


    #region Status
    public void Hp()
    {
        if (hpCoroutine != null)
            StopCoroutine(hpCoroutine);

        hpCoroutine = StartCoroutine(HpCall());
    }

    private IEnumerator HpCall()
    {
        hpSlider[0].value = enemy.curHp;
        yield return new WaitForSeconds(0.5f);

        float start = hpSlider[1].value;
        float end = enemy.curHp;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 3f;
            hpSlider[1].value = Mathf.Lerp(start, end, timer);
            yield return null;
        }
        hpSlider[1].value = enemy.curHp;
    }


    public void Groggy()
    {
        if (groggyCoroutuine != null)
            StopCoroutine(groggyCoroutuine);

        groggyCoroutuine = StartCoroutine(GroggyCall());
    }

    private IEnumerator GroggyCall()
    {
        groggySlider[0].value = enemy.curGroggy;
        yield return new WaitForSeconds(0.25f);

        float start = groggySlider[1].value;
        float end = enemy.curGroggy;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            groggySlider[1].value = Mathf.Lerp(start, end, timer);
            yield return null;
        }
        groggySlider[1].value = enemy.curGroggy;
    }


    public void DamageUI(IDamageSysteam.DamageType type, bool isCritical, int damage)
    {
        GameObject obj = Instantiate(damageUI, HitVFXPos(), Quaternion.identity);
        obj.GetComponent<DamageUI>().Setting(type, isCritical, damage);
    }

    private Vector3 HitVFXPos()
    {
        Vector3 originPosition = damagePosCollider.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = damagePosCollider.bounds.size.x;
        float range_Y = damagePosCollider.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }
    #endregion
}
