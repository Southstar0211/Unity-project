using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using forpooling; (interface 방식)
using System;


public class bomb : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject explosionPrefab; // 폭발 이펙트 프리팹 (오브젝트 풀링하면서 필요없어짐.)
    public float explosionRadius = 1.2f; // 폭발 반경
    public float explosionForce = 8f; // 폭발 힘
    //private ICannonballPool cannonballPool; (interface 방식)

    public static event Action<GameObject> OnCannonballCollision;
    public static event Action<Vector3, float> OnExplode;

    private float ExplosionNoShowHeight = 8.3f; //최적화를 위함(특정 높이 이상에서는 굳이 폭발 이펙트를 풀링할 필요 X)

    /* (interface 방식)
    private void Start()
    {
        // 주입된 ICannonballPool을 통해 순환 참조 방지
        cannonballPool = FindObjectOfType<CannonballPool>();

                if (cannonballPool == null)
        {
            Debug.LogError("CannonballPool 컴포넌트를 찾지 못했습니다! 현재 씬에 CannonballPool이 있는지 확인하세요.");
        }
        else
        {
            Debug.Log("CannonballPool 컴포넌트를 성공적으로 찾았습니다.");
        }
    }
    */

    void OnCollisionEnter2D(Collision2D collision) 
    {
            Explode(collision);
    }

    void Explode(Collision2D collision)
    {
        // 충돌 시 이벤트 호출 (풀에 반환할 오브젝트를 전달) //event 방식
        OnCannonballCollision?.Invoke(gameObject);

        //특정 높이 이상에서는 프리팹 호출 X
        if (transform.position.y <= ExplosionNoShowHeight) 
        {
            OnExplode?.Invoke(transform.position, explosionRadius); //explosionRadius를 폭발 프리팹 크기에 참고할 수 있게 전송
        }
        /*
        // 폭발 이펙트 생성 필요
        float diameter = explosionRadius*2;
        GameObject explosionEffect = explosionpool.GetExplosion(transform.position, Quaternion.identity);
        explosionEffect.transform.localScale = new Vector3(diameter, diameter, 1f);
        */
        
        //폭발 반경 내의 모든 오브젝트에 힘 적용하기 위해 collider2D[] 리스트에 hitcolliders를 넣고(폭발 반경 내 모든 오브젝트)
        //아래 foreach를 통해 [] 리스트 내에 각 오브젝트에 동일한 내용을 반복함.
        //explosionRadius 라는 값을 반지름으로 하는 원 안에 물리를 적용함.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius); 
        foreach (var hitCollider in hitColliders)  //foreach 때문에 최적화에 문제가 있는 듯... 이거 해결 *****
        {
            if (hitCollider.CompareTag("ball"))
            {
                Rigidbody2D rb = hitCollider.GetComponent<Rigidbody2D>();
        
                if (rb != null) // && hitCollider.gameObject.name != "ball"
                {
                    // 폭발 중심과의 거리 계산
                    Vector2 explosionPosition = new Vector2(transform.position.x, transform.position.y);
                    Vector2 direction = hitCollider.transform.position - (Vector3)explosionPosition;
                    //magnitude는 백터의 길이를 뜻하는 듯.

                    // 방향 정규화 
                    direction.Normalize();
                    // 백터 길이를 1로 만들어서 완벽하게 방향만 남게 만드는 것.

                    // 폭발 힘 적용 (거리에 비례하여 힘 조절)
                    rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                    
                }
            }
        }
        
    //cannonballPool.ReturnCannonball(gameObject); (interface방식)
    }

/*

    void OnDrawGizmos()
{
    // 폭발 반경을 빨간색으로 표시
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, explosionRadius);
}
*/
    }

