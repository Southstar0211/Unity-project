using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class heart : MonoBehaviour
{
    private bool candamaged = true;
    // Start is called before the first frame update
    void Start()
    {
        heartspawnedPrefabs = new List<GameObject>();
        SpawnMethodRepetition(3);
        spriteRenderer = GetComponent<SpriteRenderer>();
        candamaged = true;
    }
        void SpawnMethodRepetition(int times)
    {
        for (int i = 0; i < times; i++)
        {
            Spawnheartprefab(); // 반복할 메서드 호출
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (heartspawnedPrefabs.Count == 0) //하트가 모두 사라졌을 때.
        {
            //scenemanager 발동해서 게임오버 진행. 아니면 버튼을 가지고 있는 창을 띄우기.
        }
        
    }
    public GameObject heartprefab; 
    public Transform[] heartspawnPoints;      // 5개의 위치를 저장할 배열
    public List<GameObject> heartspawnedPrefabs; // 생성된 프리팹을 관리할 리스트

    public void Spawnheartprefab()
    {
         // 현재 리스트의 크기가 스폰 포인트의 범위를 초과하지 않는지 확인
        if (heartspawnedPrefabs.Count >= heartspawnPoints.Length)
        {
            Debug.LogError("모든 스폰 포인트에 프리팹이 이미 생성되었습니다.");
            return;
        }

        // 현재 리스트의 크기를 인덱스로 사용
        int index = heartspawnedPrefabs.Count;

        // 특정 인덱스 위치에 프리팹을 생성
        GameObject newPrefab = Instantiate(heartprefab, heartspawnPoints[index].position, heartspawnPoints[index].rotation);

        if (newPrefab != null)
        {
            heartspawnedPrefabs.Add(newPrefab);  // 리스트에 추가
        }
        else
        {
            Debug.LogError("Prefab instantiation failed.");
        }
    }

    public void DestroyLastPrefab()
    {
        if (candamaged == true && heartspawnedPrefabs.Count > 1) // 무적시간이 아닌 상황 + 리스트에 프리팹이 있을 경우
        {
            //무적이 되도록 설정
            candamaged = false;
            // 2초 뒤에 맞을 수 있도록 설정 = 2초 동안만 무적시간
            invintime(); 
            // 마지막 하트 프리팹 가져오기
            int lastHeart = heartspawnedPrefabs.Count - 1;
            GameObject prefabToDestroy = heartspawnedPrefabs[lastHeart];

            // 깜빡이다가 프리팹 파괴하도록 설정.
            StartCoroutine(BlinkAndDestroy(prefabToDestroy));
        }
        //마지막 하트가 부숴질 땐 깜빡임 없이 바로.
        if(candamaged == true && heartspawnedPrefabs.Count == 1)
        {
            int lastHeart = heartspawnedPrefabs.Count - 1;
            GameObject prefabToDestroy = heartspawnedPrefabs[lastHeart];

            //리스트에서 없애고, 오브젝트 파괴
            heartspawnedPrefabs.Remove(prefabToDestroy);
            Destroy(prefabToDestroy);
        }
        //이 외의 상황은 candamaged 가 false 인  상황이니 무적 상태임
        else
        {
            Debug.Log($"it's invin time. candamaged: {candamaged}, heart count: {heartspawnedPrefabs.Count}");
        }
    }

    private void invintime()
    {
        Invoke("Enabledamaged", 2f); //2초 동안 무적.
    }

    private void Enabledamaged()
    {
        candamaged = true;
        Debug.Log("invin time is ended.");
    }

    public SpriteRenderer spriteRenderer; // SpriteRenderer로 하트 모양을 깜빡이게 함.
    public float blinkDuration = 2f;       // 하트 2초 동안 깜빡이기.
    public float blinkInterval = 0.4f;     // 하트 깜빡이는 간격.

        private IEnumerator BlinkAndDestroy(GameObject heart)
    {
        SpriteRenderer spriteRenderer = heart.GetComponent<SpriteRenderer>();
        float elapsedTime = 0f;

        // 깜빡거리는 동안 반복
        while (elapsedTime < blinkDuration)
        {
            // 하트를 보이게/안 보이게 토글
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // 깜빡이는 간격만큼 대기
            yield return new WaitForSeconds(blinkInterval);

            // 경과 시간 업데이트
            elapsedTime += blinkInterval;
        }

        // 리스트에서 마지막 하트 제거
        heartspawnedPrefabs.Remove(heart);

        // 하트 오브젝트 파괴
        Destroy(heart);
    }

}

