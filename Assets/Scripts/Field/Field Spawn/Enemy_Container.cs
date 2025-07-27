using System.Collections.Generic;
using UnityEngine;


public class Enemy_Container : MonoBehaviour
{
    [Header("---Container Setting---")]
    [SerializeField] private List<EnemyData> enemys;
    private Dictionary<Enemy, EnemyData> data;
    public enum Enemy { Axe, Flame, Gunner, GrenadeLauncher }

    [System.Serializable]
    private class EnemyData
    {
        [Header("---Setting---")]
        [SerializeField] private int listSize;
        public GameObject enemy;
        public Queue<GameObject> enemyList;

        /// <summary>
        /// 리스트에 몬스터 추가
        /// </summary>
        public void Setting()
        {
            enemyList = new Queue<GameObject>();
            for (int i = 0; i < listSize; i++)
            {
                GameObject obj = Instantiate(enemy, Vector3.zero, Quaternion.identity);
                enemyList.Enqueue(obj);
            }
        }
    }

    private void Awake()
    {
        Data_Setting();
    }

    /// <summary>
    /// 스테이지 시작 시 풀링에 데이터 추가
    /// </summary>
    private void Data_Setting()
    {
        data = new Dictionary<Enemy, EnemyData>();
        for (int i = 0; i < enemys.Count; i++)
        {
            data.Add((Enemy)i, enemys[i]);
            enemys[i].Setting();
        }
    }


    /// <summary>
    /// 몬스터 소환 로직
    /// </summary>
    /// <param name="enemyIndex"></param>
    public GameObject Spawn_Enemy(Enemy enemy)
    {
        if (data[enemy].enemyList.Count == 0)
        {
            GameObject obj = Instantiate(data[enemy].enemy);
            return obj;
        }

        return data[enemy].enemyList.Dequeue();
    }

    /// <summary>
    /// 몬스터 반환 로직
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="obj"></param>
    public void Return_Enemy(Enemy enemy, GameObject obj)
    {
        obj.SetActive(false);
        data[enemy].enemyList.Enqueue(obj);
    }
}
