using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : MonoBehaviour
{
    public GameObject explosionPrefab;
    private int poolSize = 5;
    private List<Explosion> pool;
    private GameObject poolParent;

    private void Awake()
    {
        poolParent = new GameObject("ExplosionPool");
        pool = new List<Explosion>();

        // 풀 사이즈만큼 미리 폭발 오브젝트 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject explosionObject = Instantiate(explosionPrefab);
            Explosion explosion = explosionObject.AddComponent<Explosion>();
            explosion.SetInPool(true);
            explosionObject.SetActive(false);
            explosionObject.transform.SetParent(poolParent.transform);
            pool.Add(explosion);
        }

        // Cannonball의 OnExplode 이벤트에 구독
        bomb.OnExplode += HandleExplosionEvent;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        bomb.OnExplode -= HandleExplosionEvent;
    }

    // 폭발 이벤트가 발생했을 때 호출될 메서드
    private void HandleExplosionEvent(Vector3 position, float radius) //float radius 에는 explosionRadius가 들어갈 예정
    {
        GameObject explosionEffect = GetExplosion(position);
        if (explosionEffect != null)
        {
            explosionEffect.transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
        }
    }

    // 풀에서 폭발 오브젝트 가져오기
    public GameObject GetExplosion(Vector3 position)
    {
        foreach (Explosion explosion in pool)
        {
            if (!explosion.gameObject.activeInHierarchy)
            {
                explosion.transform.position = position;
                explosion.SetInPool(false);
                explosion.gameObject.SetActive(true);

                StartCoroutine(ReturnAfterAnimation(explosion.gameObject));
                return explosion.gameObject;
            }
        }

        // 풀에 사용 가능한 오브젝트가 없는 경우 새로 생성
        GameObject newExplosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        Explosion newExplosionComponent = newExplosion.AddComponent<Explosion>();
        newExplosionComponent.SetInPool(false);

        newExplosion.transform.SetParent(poolParent.transform);
        pool.Add(newExplosionComponent);

        StartCoroutine(ReturnAfterAnimation(newExplosion));
        return newExplosion;
    }

    // 애니메이션 재생 후 풀로 반환하는 코루틴
    private IEnumerator ReturnAfterAnimation(GameObject explosion)
    {
        Animator animator = explosion.GetComponent<Animator>();
        if (animator != null)
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }

        ReturnExplosion(explosion);
    }

    private void ReturnExplosion(GameObject explosion)
    {
        Explosion explosionComponent = explosion.GetComponent<Explosion>();
        if (explosionComponent != null)
        {
            explosionComponent.SetInPool(true);
            explosion.SetActive(false);
        }
    }
}

public class Explosion : MonoBehaviour
{
    private bool isInPool;

    public bool IsInPool()
    {
        return isInPool;
    }

    public void SetInPool(bool value)
    {
        isInPool = value;
    }
}