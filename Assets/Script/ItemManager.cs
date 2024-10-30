using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject heartItemPrefab;         // 하트 아이템 프리팹 (하트)
    public GameObject chargeSpeedUpItemPrefab; // 차지 스피드업 아이템 프리팹 (초록색 위 화살표)
    public GameObject cannonItemPrefab;        // 대포 아이템 프리팹 (무한 모양)
    public GameObject acquireEffectPrefab;     // 아이템 획득 시 효과 프리팹

    public GameObject leftwall;
    public GameObject rightwall;

    public Transform[] spawnPoints;            // 아이템 생성 위치들 (x축, y축을 미리 설정한 위치)
    public float spawnInterval = 5f;           // 아이템 생성 간격
    public int maxItems = 2;                   // 동시에 생성 가능한 최대 아이템 수

    private List<GameObject> activeItems = new List<GameObject>();  // 활성화된 아이템 목록

    // 각 스크립트 참조
    public heart heartScript;                  // heart.cs 스크립트 참조
    public charge_cb chargeScript;             // charge_cb 스크립트 참조
    //public Fire_Ball fireBallScript;           // fire_ball 스크립트 참조
    //public Item itemScript;                    // item.cs 스크립트 참조

    // 아이템 생성 확률
    public float itemSpawnChance = 0.33f;       // 아이템 생성 확률 (전체 확률)
    public float chargeSpeedUpDuration = 5f; 

    /*
    public bool active8cannon = false;
    public float active8cannonDuration = 5f;
    public GameObject showactive8cannonworking; //작동을 시각적으로 표현할 거 ************
    */

    private void Start()
    {
        // 정기적으로 아이템을 생성하도록 타이머 시작
        StartCoroutine(SpawnItems());
    }

    private IEnumerator SpawnItems()
    {
        while (true)
        {
            // 현재 생성된 아이템 수가 제한을 넘지 않는지 확인
            if (activeItems.Count < maxItems && Random.value < itemSpawnChance)
            {
                // 아이템 생성이 결정되면, 3개의 아이템 중 하나를 무작위로 선택
                SpawnRandomItem();
            }

            // 다음 아이템 생성까지 대기
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // 랜덤으로 아이템을 선택하여 생성
    private void SpawnRandomItem()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject selectedItem = null;

        // 무작위로 3개의 아이템 중 하나 선택
        while (selectedItem == null)
        {
            int randomIndex = Random.Range(0, 3);
            switch (randomIndex)
            {
                case 0:
                    if (heartScript.heartspawnedPrefabs.Count < 3)  // 하트 개수 제한
                    {
                        selectedItem = heartItemPrefab;
                    }
                    break;
                case 1:
                    selectedItem = chargeSpeedUpItemPrefab;
                    break;
                /*
                case 2:
                    selectedItem = cannonItemPrefab;
                    break;
                */
            }
        }

        // 선택된 아이템이 있으면 생성
            GameObject newItem = Instantiate(selectedItem, spawnPosition, Quaternion.identity);
            activeItems.Add(newItem);  // 활성화된 아이템 목록에 추가

            //아래 Imnumerator 를 통해 5초 뒤 아이템 자동 삭제
            StartCoroutine(DestroyAfterDelay(newItem, 5f)); 

            // 아이템 획득 시 처리할 로직을 부여
            ItemPickup itemPickup = newItem.GetComponent<ItemPickup>();
            itemPickup.Initialize(this, selectedItem.name);  // 아이템 이름으로 처리
    }

    //기본적으로 아이템은 5초 후 사라지게끔 설정함. 하지만, 5초가 되기전에 획득될 경우를 고려하여 if로 한 번 있는지 판단.
    IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            // 오브젝트 삭제
            Destroy(obj);
        }
        
        if (activeItems.Contains(obj))
        {
        // 리스트에서 해당 오브젝트 제거
        activeItems.Remove(obj);
        }
    }

    // 랜덤한 스폰 위치를 반환 (벽과 겹치지 않도록)
    private Vector3 GetRandomSpawnPosition()
    {
        float xMin = leftwall.transform.position.x + 1f; // 왼쪽 벽에서 조금 떨어진 위치
        float xMax = rightwall.transform.position.x - 1f; // 오른쪽 벽에서 조금 떨어진 위치
        float yMin = -2f; //Camera.main.ViewportToWorldPoint(new Vector3(0, 0.2f, 0)).y; // 카메라 하단 20% 지점
        float yMax = 6.1f; //Camera.main.ViewportToWorldPoint(new Vector3(0, 0.9f, 0)).y; // 카메라 상단 90% 지점

        float YexcludedMin = -0.4f;
        float XexcludedMin = -1.6f; // 제외할 범위의 시작
        float XexcludedMax = 1.6f; // 제외할 범위의 끝

        float x;
        float y;
        
        float randomValue = Random.value;

        // 세 구간 중 하나를 선택 / 대포 앞 쪽에 아이템 소환을 저지함.
        if (randomValue < 1f / 3f)
        {
            // 첫 번째 구간
            x = Random.Range(xMin, XexcludedMin);
            y = Random.Range(yMin, yMax);
        }
        else if (randomValue < 2f / 3f)
        {
            // 두 번째 구간
            x = Random.Range(XexcludedMin, XexcludedMax);
            y = Random.Range(YexcludedMin, yMax);
        }
        else
        {
            // 세 번째 구간
            x = Random.Range(XexcludedMax, xMax);
            y = Random.Range(yMin, yMax);
        }

        return new Vector3(x, y, 0f);
    }

    // 아이템 획득 시 호출되는 함수
    public void OnItemPickedUp(GameObject item, string itemType)
    {
        // 획득 시 해당 아이템에 맞는 로직 처리
        if (itemType == heartItemPrefab.name)
        {
            heartScript.Spawnheartprefab();  // 하트 생성
        }
        else if (itemType == chargeSpeedUpItemPrefab.name)
        {
            StartCoroutine(ActivateChargeSpeedUp());  // 차지 스피드업 효과 발동
        }
        /*
        else if (itemType == cannonItemPrefab.name)
        {
            StartCoroutine(ActivateCannonMode());  // 대포 발사 모드 발동
        }
        */

        // 획득 효과 생성
        Instantiate(acquireEffectPrefab, item.transform.position, Quaternion.identity);

        // 획득 시 아이템 파괴
        Destroy(item);
        activeItems.Remove(item);  // 활성화된 아이템 목록에서 제거
    }

    private IEnumerator ActivateChargeSpeedUp()
    {
        float originalChargeTime = chargeScript.chargetime;
        //UnityEngine.Color originalChargebarcolor = chargeScript.chargebarRenderer.color;
        chargeScript.chargetime = 1f; // 1초로 줄이기
        //chargeScript.chargebarRenderer.color = UnityEngine.Color.red;   // 색상 변경
        yield return new WaitForSeconds(chargeSpeedUpDuration);  // 발동 시간 동안 대기
        //chargeScript.chargetime = originalChargeTime; // 원래 시간으로 복구
        //chargeScript.chargebarRenderer.color = originalChargebarcolor;  // 색상 복구
    }
}

/*
    private IEnumerator ActivateCannonMode()
    {
        active8cannon = true;
        yield return new WaitForSeconds(active8cannonDuration); // 발동 시간 동안 대기
        active8cannon = false;
    }

*/


/*
목숨 채우는 거. 잠시 동안 충전 속도 증가?, 
-목숨 채우는 거 문제 x 충전 속도를 증가시키는 것도 간단히 변수 하나의 변경만으로 가능한지? -> 가능

아이템은 cb나 ball로 획득할 수 있도록. 그럼 코드는 ball과 cb 스크립트 안에 넣어야 하나?
그게 좋겠다. item들은 prefab으로 만들어서 정해진 범위 내에 랜덤 생성이 가능하도록 하고
범위에 대한 계산은... 해상도에 따라 상이할 수 있으므로 y는 카메라 기준으로. x는 wall 안 쪽에 오도록.
y도 절댓값 기준으로 해도 될 듯 이제.

if(2Dentdercollision) 을 통해서 other == "obeject name"; 으로 진행.
아니면 태그 설정을 통해 특정 태그에만 collision이 발생하도록 진행..
해당 태그는 ball과 cannonball에만 한하여 적용.


1. heart.cs 스크립트를 참조해서 해당 스크립트 내에 하트 개수를 가르키는 heartspawnedPrefabs.Count 가 3보다 작을 떄,
heart item 오브젝트를 게임하는 동안 일정 확률로 정해진 위치 중 랜덤으로 생성되게 하기. 해당 아이템을 획득하면 하트 1개가 생성. 하트를 하나 생성하는 매서드는
Spawnheartprefab() 이며, heart.cs에 선언되어 있음. 

2. 언제든지 게임하는 동안 일정 확률으로 charge speedup item 오브젝트를 정해진 위치 중 랜덤으로 생성되게 하기. 해당 아이템을 획득 시에는 charge_cb 스크립트를 참조하여 
해당 스크립트 내에 chargetime을 2초보다 빠르게 함. charge_bar Object의 색깔을 발동 시간 동안만 변경.
발동시간은 chargespeedupduration으로 관리.

3. 잠시동안 제한없이 대포를 쏠 수 이는 item 오브젝트를 게임하는 동안 일정 확률로 정해진 위치 중 랜덤으로 생성되게 하기. item.cs 내에서는
bool 형태의 active8cannon 변수를 사용하고 fire_ball.cs 스크립트에서 해당 active8cannon이 활성화된 경우에는 대포를 발사해도
chargecbsript.cbslot -= 1; 가 없도록 조작. 발동시간은 active8cannonduration 으로 관리.

4. 상술한 item 프리팹은 모두 ball 또는 cannonball 오브젝트와 부딪혔을 때만 획득되도록 하고 (2D collision 활용)
ball과 cannonball이 가진 공통적인 태그를 통해 이를 구분시킴.
획득 시에는 해당 item prefab을 파괴.
또한 부딪힐 때 이펙트로서 acquire_effect 연속된 스프라이트를 나열하여 애니메이션으로 만든 prefab을 해당 item과 같은 위치에 생성.

5. 또한 위의 item 프리팹을 생성할 위치는 y축 좌표로는 메인 카메라를 넘어가지 않으면서 아이템 오브젝트가 보이는 0.2~0.9 범위에서,
x축 좌표로는 게임 내 양 옆에 위치한 벽인 wall_l 과 wall_r 과 겹치지 않게 생성될 수 있도록 x축 좌표를 넣어 해당 범위 내에서 랜덤하게
생성되도록 코드 제작.

6. 해당 item 프리팹은 동 시간대에 최대 2개까지만 생성되도록 설정하여, 아이템이 한 번에 남발하지 않도록 제한하기.
같은 종류의 아이템은 동 시간대에 생길 수 없도록, 최대 한 번에 1개로 제한하기.

7. 언제든 새로은 item 종류를 개발할 수도 있으니 그에 적합하게 새로운 item을 추가하기 용이한 형태로 코드 제작.

목숨 늘려주는 거
충전 시간 빠르게 해주는 거
다음 대포알이 강력해짐.
*/