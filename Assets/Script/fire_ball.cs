using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems; //마우스가 UI 요소 위에 있는지 확인하기 위해 호출
//using forpooling; //순환 참조를 막기 위해 (interface 방식)


public class fire_ball : MonoBehaviour
{
    public float cannonballspd;
    //public GameObject cannonball;
    public CannonballPool cannonballPool;

    private UnityEngine.Vector2 lookDirection;
    private float lookAngle;
    public Transform cannontip; //transform은 위치와 회전과 관련된.
    // Start is called before the first frame update
    private float minAngle = -70f; // 최소 회전 각도
    private float maxAngle = 70f; // 최대 회전 각도 
    public charge_cb chargecbsript;


    void Start()
    {
            GameObject chargeBarObject = GameObject.Find("chargebar");
        if (chargeBarObject != null)
        {
            Debug.Log("ChargeBar 오브젝트를 찾았습니다.");
            chargecbsript  = chargeBarObject.GetComponent<charge_cb>(); 
            //getcomponent 앞에 chargebarObject. 를 붙임으로써 작동함. 스크립트 잘 찾음 이제
            if (chargecbsript != null)
            {
                Debug.Log("Charge_cb 스크립트를 찾았습니다.");
            }
            else
            {
                Debug.LogError("ChargeBar 오브젝트에 Charge_cb 스크립트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("ChargeBar 오브젝트를 찾을 수 없습니다.");
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI()) //0은 마우스 왼쪽 버튼 + UI (ex. button 위에 있지 않을 때만 작동)
        {
            lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            //코드가 대포 게임 오브젝트에 부착된 스크립트일 때, transform은 자동으로 그 오브젝트의 트랜스폼을 참조하게 돼. 즉, 스크립트가 대포에 속하므로, transform은 대포의 위치와 회전을 나타내는 것이다.
            //screenworld 안에 마우스 포인트 좌표값을 (맨 하단 아래가 0,0으로 계산) 대포 위치를 뺀다. -> 백터를 얻는 식임.
            lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f; 

            if ( lookAngle < minAngle)
            {
            // 최솟값과 최댓값 중 더 가까운 값으로 설정
                if ( lookAngle > -180f)
                {
                    lookAngle = minAngle;
                }
                else
                {
                    lookAngle = maxAngle;
                }
            }
            //아크탄젠트는 밑변 높이라는 변수를 넣어서 라디안 값으로 구함. 때문에 rad2deg(180/pi)를 통해 각도로 바꾸어 산출
            //아크탄젠트 식을 통해서 "x축" 기준으로 x값을 밑변, y값을 높이로 하여 각도를 계산.
            //각도는 x축을 기준으로 계산하지만 대포의 회전은 y축을 기준으로 시작하므로, 90를 빼주어야 함.
            transform.rotation = UnityEngine.Quaternion.Euler(0f, 0f, lookAngle);
            //Quaternion 쿼터니언은 3D 회전을 표현하는 방식 중 하나
            //하지만 직관적으로 알기 어렵기 때문에 사람이 쉽게 이해가능한 오일러(Euler)각 사용
            
            if(chargecbsript.cbslot > 0)
            {
                FireCannnonball();

                chargecbsript.cbslot -= 1;
                //cbslot 채워진 거 하나를 지워야 함. 마지막으로 채워진 거... 있는 cbslot 중 가장 큰 숫자 slot을 destroy? 
                chargecbsript.DestroyLastPrefab();
            }
            else //cbslot이 0일 때.
            {
                Debug.Log("there is no prepared Cannonball");
                //sound insert
            }
        }

    }

    private void FireCannnonball()
    {
        //GameObject firedCannonball = Instantiate(cannonball, cannontip.position, cannontip.rotation);
        GameObject firedCannonball = cannonballPool.GetCannonball(cannontip.position, cannontip.rotation);

        //firedCannonball.GetComponent<Rigidbody2D>().velocity = cannontip.up * cannonballspd;
        Rigidbody2D rb = firedCannonball.GetComponent<Rigidbody2D>();
        rb.velocity = cannontip.up * cannonballspd;

        //Destroy(firedCannonball, 3.5f);
        StartCoroutine(ReturnCannonballAfterDelay(firedCannonball, 3.5f));
    }

    private IEnumerator ReturnCannonballAfterDelay(GameObject cannonball, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 캐논볼이 이미 풀로 반환되지 않았는지 체크
        if (!cannonballPool.IsInPool(cannonball)) //true가 아니면, if문 작동
        {
            // 캐논볼을 풀로 반환
            cannonballPool.ReturnCannonball(cannonball);
        }
    }

    private bool IsPointerOverUI()
    {
        // 모바일에서 터치 감지
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        //모바일에서는 버튼 터치를 인식하지 못하는 문제 해결

        // PC에서 마우스 클릭 감지
        return EventSystem.current.IsPointerOverGameObject();
    }
}

