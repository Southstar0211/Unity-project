using System.Collections.Generic;
using UnityEngine;

public class ChargedPool : MonoBehaviour
{
    public GameObject chargefullPrefab; // 풀에 사용할 프리팹
    public int poolSize = 5; // 풀에 저장할 오브젝트 수
    private List<GameObject> pool; // 오브젝트 풀 리스트

    public Transform[] chargeSpawnPoints; // 충전 표시 위치 배열

    void Start()
    {
        pool = new List<GameObject>();

        // 초기 풀 생성 및 오브젝트 비활성화
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(chargefullPrefab);
            obj.SetActive(false); //처음에 충전된 상태로 시작
            pool.Add(obj);
        }
    }

    // 오브젝트 활성화 메서드
    public GameObject ActivatePooledObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return null; // 풀에 여유 오브젝트가 없을 때
    }

    // 오브젝트 비활성화 메서드
    public void DeactivatePooledObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}