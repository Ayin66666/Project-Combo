using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_Manager : MonoBehaviour
{
    [Header("---State---")]
    [SerializeField] private RoomType roomType;
    [SerializeField] private bool haveStartDialog;
    [SerializeField] private int startDialogIndex;
    [SerializeField] private bool haveEndDialog;
    [SerializeField] private int endDialogIndex;

    // Normal - 스폰 타입: 일반, 러쉬, 파괴
    // Puzzle - 스폰 타입: 일반
    // Boss   - 스폰 타입: 일반
    private enum RoomType { Normal, Rush, Destroy, Puzzle, Boss } 
    private List<Func<IEnumerator>> StageTypes;


    [Header("---Dialog---")]
    private int[] dialogIndex;


    [Header("---Enemy---")]
    [SerializeField] private List<SpawnData> spawnDatas;
    [SerializeField] private GameObject destoryObject;
    private int enemyCount;


    [System.Serializable]
    public struct SpawnData
    {
        public List<GameObject> enemys;
        public List<Transform> spawnPos;
    }



    [Header("---Component---")]
    [SerializeField] private GameObject[] walls;
    private Coroutine stageCoroutine;

    private void Start()
    {
        StageTypes = new List<Func<IEnumerator>>
        {
            Type_Normal, Type_Rush, Type_Destroy, Type_Puzzle, Type_Boss
        };
    }

    public void Stage_Start()
    {
        // 벽 생성
        Wall_Setting(true);

        // 스테이지 시작
        stageCoroutine = StartCoroutine(StageTypes[(int)roomType]());

        // 시작 다이얼로그
        if (haveEndDialog)
        {
            Stage_Manager.instance.Dialog(endDialogIndex);
        }
    }

    private void Wall_Setting(bool isOn)
    {

    }

    private void Stage_End()
    {
        // 벽 종료
        Wall_Setting(false);

        // 종료 다이얼로그
        if(haveEndDialog)
        {
            Stage_Manager.instance.Dialog(endDialogIndex);
        }
    }

    #region Stage
    // { Normal, Rush, Destroy, Puzzle, Boss } 
    private IEnumerator Type_Normal()
    {
        // 1회 소환
        enemyCount = spawnDatas[0].enemys.Count;
        for (int i = 0; i < spawnDatas[0].enemys.Count; i++)
        {
            spawnDatas[0].enemys[i].SetActive(true);
            spawnDatas[0].enemys[i].GetComponent<Enemy_Base>().Spawn();
        }

        // 몬스터 체크
        while(enemyCount > 0)
        {
            spawnDatas[0].enemys.RemoveAll(enemy => enemy == null);
            enemyCount = spawnDatas[0].enemys.Count;
            yield return new WaitForSeconds(2f);
        }

        Stage_End();
    }

    private IEnumerator Type_Rush()
    {
        for (int i = 0; i < spawnDatas.Count; i++)
        {
            enemyCount = spawnDatas[i].enemys.Count;
            for (int i1 = 0; i1 < spawnDatas[i].enemys.Count; i1++)
            {
                spawnDatas[i].enemys[i1].SetActive(true);
                spawnDatas[i].enemys[i1].GetComponent<Enemy_Base>().Spawn();
            }

            // 몬스터 체크
            while (enemyCount > 0)
            {
                spawnDatas[i].enemys.RemoveAll(enemy => enemy == null);
                enemyCount = spawnDatas[i].enemys.Count;
                yield return new WaitForSeconds(2f);
            }

            // 다음 스폰 딜레이
            if(i < spawnDatas.Count)
                yield return new WaitForSeconds(2f);
        }

        Stage_End();
    }

    private IEnumerator Type_Destroy()
    {
        bool isFirstSpawn = true; // 첫 소환 여부
        List<GameObject> activeEnemies = new List<GameObject>(); // 현재 활성화된 몬스터 목록

        // 중앙 몬스터가 파괴되기 이전까지 소환 지속
        while (destoryObject != null)
        {
            // 스폰 데이터 선택 (랜덤)
            int ran = UnityEngine.Random.Range(0, spawnDatas.Count);
            List<GameObject> enemyList = spawnDatas[ran].enemys;

            // 첫 번째 소환 = 전체 몬스터 소환
            if (isFirstSpawn)
            {
                isFirstSpawn = false;
                for (int i = 0; i < enemyList.Count; i++)
                {
                    GameObject obj = Instantiate(enemyList[i], spawnDatas[ran].spawnPos[i].position, Quaternion.identity);
                    activeEnemies.Add(obj);
                }
            }
            else
            {
                // 몬스터가 10마리 미만일 때만 추가 소환
                int currentEnemyCount = activeEnemies.Count;

                if (currentEnemyCount < 10)
                {
                    int spawnCount = Mathf.Min(10 - currentEnemyCount, enemyList.Count);

                    for (int i = 0; i < spawnCount; i++)
                    {
                        GameObject obj = Instantiate(enemyList[i], spawnDatas[ran].spawnPos[i].position, Quaternion.identity);
                        activeEnemies.Add(obj); // 활성화된 몬스터 목록에 추가
                    }
                }
            }

            // 🔹 10초 대기 후 다시 확인
            yield return new WaitForSeconds(10f);

            // 🔹 활성화된 몬스터 목록에서 null 제거 (죽은 몬스터 제거)
            activeEnemies.RemoveAll(enemy => enemy == null);
        }
        
        // destoryObject가 파괴되었을 때 모든 몬스터 사망 처리
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            // 이미 죽은 몬스터 제외
            if (activeEnemies[i] != null) 
            {
                activeEnemies[i].GetComponent<Enemy_Base>().Die();
            }
        }
        Stage_End();
    }

    private IEnumerator Type_Puzzle()
    {
        yield return null;
    }

    private IEnumerator Type_Boss()
    {
        yield return null;
    }
    #endregion
}
