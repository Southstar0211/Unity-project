using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class charge_cb : MonoBehaviour
{
    private float time = 0f; //조건 하 시간 작동
    public float chargetime = 2f; //second 충전시간 변환 시 애니메이션도 변환해야함. +ExecuteAni() 매서드도 변경.
    public float cbslot = 5; //초기 충전된 대포 슬롯
    public GameObject chargebar; // UI에서 충전 막대를 나타내는 sprite
    public SpriteRenderer chargebarRenderer; //item에서 충전 막대 색상을 조정하기 위해 선언

    /*
    public Animator chargebar_ani; //animation 불러오기 inspector 창에서 drag and drop
    private bool canExecuteAni = true;
    */

    // Start is called before the first frame update
    void Start()
    //시작하자마자 spawnprefabs을 새로운 list로 선언하고, 하트 5개를 리스트에 채우기, 차지리셋을 통해 bar 원위치
    {
        //chargebar_ani = GetComponent<Animator>(); //animation get component
        spawnedPrefabs = new List<GameObject>();
        SpawnMethodRepetition(5);
        ResetCharge();
        chargebarRenderer = GetComponent<SpriteRenderer>();

        fortest();
    }

        private IEnumerator fortest()
    {
        Color originalChargebarcolor = chargebarRenderer.color;
        chargebarRenderer.color = Color.white; 
        chargebarRenderer.color = Color.red;   // 색상 변경
        yield return new WaitForSeconds(3f);  // 대기
        //chargebarRenderer.color = originalChargebarcolor;  // 색상 복구
    }

    public GameObject chargefullprefab;            // 생성할 프리팹
    public Transform[] chargeSpawnP;      // 5개의 위치를 저장할 배열
    private List<GameObject> spawnedPrefabs; // 생성된 프리팹을 관리할 리스트

    public void Spawnchargefullprefab()
    {
         // 현재 리스트의 크기가 스폰 포인트의 범위를 초과하지 않는지 확인
        if (spawnedPrefabs.Count >= chargeSpawnP.Length)
        {
            Debug.LogError("모든 스폰 포인트에 프리팹이 이미 생성되었습니다.");
            return;
        }

        // 현재 리스트의 크기를 인덱스로 사용
        int index = spawnedPrefabs.Count;

        // 특정 인덱스 위치에 프리팹을 생성
        GameObject newPrefab = Instantiate(chargefullprefab, chargeSpawnP[index].position, chargeSpawnP[index].rotation);

        spawnedPrefabs.Add(newPrefab);
    }

    // Update is called once per frame

    void Update() //실시간으로 cbslot 갯수 확인하여 조건에 따라 충전시간이 흘러가게 하고, 그 비율에 따라 bar 변환
    {
        if(cbslot < 5)  //최대 슬롯 = 5
        {
            // 애니메이션이 끝날 동안 한 번만 애니메이션이 작동하도록 조작
            time += Time.deltaTime; 

            // 충전 막대의 fillAmount를 비율에 맞게 업데이트
            float scale = time / chargetime;
            chargebar.transform.localScale = new Vector3(3.5f, 5.6f*scale, 1f);

            if(time > chargetime)
            {
                cbslot += 1;
                Spawnchargefullprefab();
                // 충전될 때마다 prefab 생성
                time = 0;
                ResetCharge();
            }
        }
        else //cbslot이 다 채워진 경우.
        {

        } 
    }

    public void ResetCharge()
    {   
        time = 0f;
        chargebar.transform.localScale = new Vector3(1f, 0f, 1f); // 초기 상태
        Debug.Log("충전 초기화됨.");
    }

    /*
    void ExecuteAni()
    {
        chargebar_ani.SetTrigger("chargebar_charge");  //ani 작동
        // 메서드를 실행하고, 일정 시간 동안 다시 실행되지 않도록 설정
        canExecuteAni= false;
        // 3초 후에 다시 메서드 실행 가능하도록 설정
        Invoke("EnableMethod", 2f);
    }

    void EnableMethod()
    {
        canExecuteAni = true;
    }
    */

        // 리스트에 있는 프리팹을 제거
    public void DestroyLastPrefab()
    {
        if (spawnedPrefabs.Count > 0) // 리스트에 프리팹이 있을 경우
        {
            // 마지막 프리팹 가져오기
            int lastIndex = spawnedPrefabs.Count - 1;
            GameObject prefabToDestroy = spawnedPrefabs[lastIndex];

            // 프리팹 제거
            Destroy(prefabToDestroy);

            // 리스트에서 제거
            spawnedPrefabs.RemoveAt(lastIndex);

            Debug.Log($"{prefabToDestroy.name} 삭제됨.");
        }
        else
        {
            Debug.LogWarning("제거할 프리팹이 없습니다.");
        }
    }

        void SpawnMethodRepetition(int times)
    {
        for (int i = 0; i < times; i++) //int는 0으로 시작해서 times 보다 같아지기 직전까지 +1씩 더하며 아래 반복
        {
            Spawnchargefullprefab(); // 반복할 메서드 호출
        }
    }

}
