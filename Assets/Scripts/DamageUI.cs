using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DamageUI : MonoBehaviour
{
    [Header("--- UI ---")]
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private Image borderImage;
    [SerializeField] private Sprite[] borderSprite;
    [SerializeField] private Color[] outLineColor;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform movePos;


    public void Setting(IDamageSysteam.DamageType type, bool isCritical, int damage)
    {
        borderImage.color = type == IDamageSysteam.DamageType.Physical ? outLineColor[0] : outLineColor[1];
        borderImage.sprite = isCritical ? borderSprite[0] : borderSprite[1];
        damageText.text = damage.ToString();

        StartCoroutine(LookAk());
        StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        // 대기
        yield return new WaitForSeconds(1f);

        // 위로 올라가며 사라지기
        transform.DOMove(movePos.position, 1f).SetEase(Ease.OutQuad);
        canvasGroup.DOFade(0, 1f).SetEase(Ease.OutQuad).OnComplete(() => Destroy(gameObject));
    }

    private IEnumerator LookAk()
    {
        while (true)
        {
            Vector3 lookDir = (PlayerAction_Manager.instance.cam.position - transform.position).normalized;
            lookDir.y = 0;
            transform.rotation = Quaternion.LookRotation(-lookDir);
            yield return null;
        }
    }
}
