using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour 
{
    private ItemManager itemManager;
    private string itemType;

    // 아이템 타입을 초기화하는 함수
    public void Initialize(ItemManager manager, string type)
    {
        itemManager = manager;
        itemType = type;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트가 공 또는 대포인지 확인
        if (collision.CompareTag("cannonball"))  //collision.CompareTag("ball") || 
        {
            // 아이템을 획득하고 효과를 발동
            itemManager.OnItemPickedUp(gameObject, itemType);
        }
    }
}
