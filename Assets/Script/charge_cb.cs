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
    public ChargedPool chargePool; // ChargedPool 스크립트 참조
    private List<GameObject> activeChargeIndicators; // 활성화된 프리팹 목록

    void Start()
    //시작하자마자 spawnprefabs을 새로운 list로 선언하고, 하트 5개를 리스트에 채우기, 차지리셋을 통해 bar 원위치
    {
        activeChargeIndicators = new List<GameObject>();
        chargebarRenderer = GetComponent<SpriteRenderer>();
        InitializeCharges((int)cbslot); // 초기 충전 슬롯 초기화
        ResetCharge();
    }

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
                AddChargeSlot();
                // 충전될 때마다 prefab 생성
                time = 0;
                ResetCharge();
            }
        }
    }

    public void ResetCharge()
    {   
        time = 0f;
        chargebar.transform.localScale = new Vector3(1f, 0f, 1f); // 초기 상태
    }

    public void AddChargeSlot()
    {
        GameObject newIndicator = chargePool.ActivatePooledObject(); //pool 관리, 활성화 시킴
        if (newIndicator != null)
        {
            int index = activeChargeIndicators.Count; //활성화된 프리팹 목록 개수를 index에 넣음

            if (index < chargePool.chargeSpawnPoints.Length) //index값이 스폰 포인트 전체 개수볻 작으면 실행(오류 방지)
            {
                // 지정된 위치에 프리팹을 활성화 및 위치 설정
                newIndicator.transform.SetPositionAndRotation(
                chargePool.chargeSpawnPoints[index].position,
                chargePool.chargeSpawnPoints[index].rotation
            );
                activeChargeIndicators.Add(newIndicator); //활성화된 목록에 넣기
            }
        }
    }

    public void AfterFireCannon() //fire_ball 에서 사용할 매서드
    {
        if (cbslot > 0)
        {
            cbslot -= 1;
            RemoveLastChargeIndicator();
        }
    }

    public void RemoveLastChargeIndicator()
    {
        if (activeChargeIndicators.Count > 0)
        {
            int lastIndex = activeChargeIndicators.Count - 1;
            GameObject lastIndicator = activeChargeIndicators[lastIndex];

            chargePool.DeactivatePooledObject(lastIndicator); // 프리팹 비활성화 //pool 관리
            activeChargeIndicators.RemoveAt(lastIndex); //활성화된 프리팹에서 삭제
        }
    }

        void InitializeCharges(int times)
    {
        for (int i = 0; i < times; i++)
        {
            AddChargeSlot(); // 초기 충전 슬롯 활성화
        }
    }

}
