using System.Collections.Generic;
using UnityEngine;

/*
순환참조: 오브젝트 풀링을 위해서 캐논볼을 풀링하려함.
때문에 해당 스크립트에는 캐논볼프리팹을 참조하도록 하였으나,
문제는 캐논볼이 충돌했을 때, 해당 프리팹을 풀링에서 비활성화하기 위해서 bomb 스크립트를
캐논볼 프리팹 안에다가 넣어놓음
근데 그 bomb 스크립트에서 오브젝트 풀링을 하고자하니 해당 스크립트를 참조해야 함.
때문에 순환 참조 발생.

풀링 스크립트(a)가 프리팹(b)을 참조하고,
해당 프리팹(b)에 포함된 다른 스크립트(c)가 또 풀링 스크립트(a)를 참조하려고 하니까 생긴 문제
a-> b -> c -> a
*/

//namespace forpooling //순환 참조 해결을 위해(interface 방식)
//{

/*
        public interface ICannonballPool
    {
        void ReturnCannonball(GameObject cannonball);
    }
    */

public class CannonballPool : MonoBehaviour//, ICannonballPool
{
    public GameObject cannonballPrefab; // 캐논볼 프리팹
    private int poolSize = 8;           // 초기 풀 크기
    private List<Cannonball> pool;      // Cannonball 객체 리스트
    private GameObject poolParent;
    private void Awake()
    {
        poolParent = new GameObject("Cannonballpool"); //게임 오브젝트 생성
        pool = new List<Cannonball>();

        //풀사이즈 만큼 미리 프리팹 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject cannonballObject = Instantiate(cannonballPrefab);
            Cannonball cannonball = cannonballObject.AddComponent<Cannonball>(); //cannonballObject를 cannonball로서 선언
            cannonball.SetInPool(true); // 초기에는 풀에 포함된 상태로 설정
            cannonballObject.SetActive(false);

            // 생성된 캐논볼을 부모 오브젝트 아래에 넣기
            cannonballObject.transform.SetParent(poolParent.transform);

            pool.Add(cannonball);
        }
        bomb.OnCannonballCollision += ReturnCannonball;
    }

    private void OnDestroy() //ondestroy()는 자동적으로 사용되는 것으로서, 씬이 끝나거나 하면 자동적으로 구독을 해제하여 메모리 절약
    {
        // 씬이 종료될 때 이벤트 구독 해제
        bomb.OnCannonballCollision -= ReturnCannonball;
    }


    public GameObject GetCannonball(Vector3 position, Quaternion rotation)
    {
        foreach (Cannonball cannonball in pool) //반복문 foreach
        {
            if (!cannonball.gameObject.activeInHierarchy) //활성화되어 있는 오브젝트가 아니면... 풀에서 제거하고 활성화
            {
                cannonball.transform.position = position;
                cannonball.transform.rotation = rotation;
                cannonball.SetInPool(false); // 풀에서 제거된 상태로 설정
                cannonball.gameObject.SetActive(true);
                return cannonball.gameObject;
            }
        }

        //비활성화된 캐논볼이 없는 경우 새로운 캐논볼을 생성합니다. 
        GameObject newCannonball = Instantiate(cannonballPrefab, position, rotation);
        Cannonball newCannonballComponent = newCannonball.AddComponent<Cannonball>();
        newCannonballComponent.SetInPool(false); // 새로 생성된 캐논볼은 풀에서 제거된 상태로 설정

        newCannonball.transform.SetParent(poolParent.transform);
        
        pool.Add(newCannonballComponent);
        return newCannonball;
    }

    public void ReturnCannonball(GameObject cannonball)
    {
        Cannonball cannonballComponent = cannonball.GetComponent<Cannonball>();
        if (cannonballComponent != null)
        {
            cannonballComponent.SetInPool(true); // 풀에 반환된 상태로 설정
            cannonball.SetActive(false);
        }
    }

    public bool IsInPool(GameObject cannonball)
    {
        Cannonball cannonballComponent = cannonball.GetComponent<Cannonball>();
        return cannonballComponent != null && cannonballComponent.IsInPool();
        //캐논볼이 컴포넌트로서 있으며, 풀에 존재하면 true를 반환.
    }
}

//} (interface 방식)

public class Cannonball : MonoBehaviour
{
    private bool isInPool; // 오브젝트가 풀에 있는지 상태를 표시

    public bool IsInPool()
    {
        return isInPool;
    }

    public void SetInPool(bool value)
    {
        isInPool = value; // 상태를 업데이트
    }
}