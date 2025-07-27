using UnityEngine;


public class Enemy_Elite_Controller : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Phase curPhase;
    [SerializeField] private Enemy_Base[] phase_Bodys;
    private enum Phase { Phase1, Phase2 }


    private void Start()
    {
        // ���� 1ȸ Ȱ��ȭ �� ���� - 1������ ��ȯ
        Stage_Spawn(0);
    }

    public void Stage_Spawn(int phaseCount)
    {
        if(phase_Bodys[phaseCount].curState == Enemy_Base.State.None)
        {
            curPhase = (Phase)phaseCount;

            phase_Bodys[phaseCount].gameObject.SetActive(true);
            phase_Bodys[phaseCount].Spawn();
        }
    }

    public void Stage_End()
    {
        Destroy(gameObject);
    }
}
