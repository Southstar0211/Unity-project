using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballbounce : MonoBehaviour
{
    Rigidbody2D rb;
    private float LRbouncepower = 1f; //왼, 오 바운스 힘
    private float UPbouncepower = 2f; //위로 바운스 힘
    private float heartbreakbounce = 2.5f;
    public float maxForce = 10f; // 최대 힘 제한
    public heart heartsript;

    // Start is called before the first frame update

    

    void Start()
    {
        GameObject HeartObject = GameObject.Find("Heart");
        heartsript = HeartObject.GetComponent<heart>(); 
        rb = GetComponent<Rigidbody2D>(); 
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; //빠른 공의 터널링 현상 최소화
    }

    public void AddForce(Vector2 force)
    {
        // 현재 힘의 크기 계산
        if (force.magnitude > maxForce)
        {
            force = force.normalized * maxForce; // 힘의 크기를 제한
        }

        // 힘 적용
        rb.AddForce(force);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        // 특정 이름을 가진 오브젝트와 충돌 시 실행
        // 
        if (other.gameObject.name == "wall_l")
        {
            //왼쪽 벽에 부딪혔을 때
            rb.velocity += Vector2.right * LRbouncepower;
            rb.velocity += Vector2.up * UPbouncepower;
        }
        
        //오른쪽 벽에 부딪혔을 때
        if (other.gameObject.name == "wall_r")
        {
            rb.velocity += Vector2.left * LRbouncepower;
            rb.velocity += Vector2.up * UPbouncepower;
        }

        if (other.gameObject.name == "bottom")
        {
            rb.velocity += Vector2.up * heartbreakbounce;
            // heart -= 1
            // 마지막 하트 prefab을 destory하는 방식으로 
            heartsript.DestroyLastPrefab();
            //무적시간 생각해서... Destroy 매서드 안에 넣어야겠네 
        }
    }
}
