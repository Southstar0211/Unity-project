using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowmotion : MonoBehaviour
{

    public float slowMotionFactor = 0.3f;   // 슬로우 모션 비율 (0.5 = 절반 속도)
    public float slowMotionDuration = 2f;   // 슬로우 모션 지속 시간
    public float cooldownDuration = 10f;    // 쿨다운 시간
    private bool isSlowMotionReady = true;  // 슬로우 모션 사용 가능 여부

    // Start is called before the first frame update
    void Start()
    {
        //slowmotion 작동 버튼 애니메이션 불러와야 함.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ActivateSlowMotion()
    {
        // 슬로우모션 시작
        isSlowMotionReady = false;
        Time.timeScale = slowMotionFactor;  // 게임 속도 감소
        Time.fixedDeltaTime = Time.timeScale * 0.02f;  // 물리 업데이트 속도도 줄이기
        //0.02f는 원래 물리 업데이트를 1초에 50번 한다는 뜻임.
        //그냥 타임만 줄이면 물리적인 것은 적용 안 되기 때문에 물리 업데이트 속도도 줄여서 맞추기

        // 슬로우모션 지속 시간 대기 (2초)
        yield return new WaitForSecondsRealtime(slowMotionDuration);
        //대기 시간동안 매서드를 비동기 시키기 위해서 IEnumerator를 사용.

        //2초동안 진행될 animation 삽입. 차있던 게 내려가는 걸로 만들고... 
        //또는 가리고 있던 게 사라지는 걸로. 애니메이션 삽입 시에는 pivot을 이용하여서 버튼 위에 고정
        //아니면 버튼 안에 있는 어떤 애니메이션으로? 버튼 조작에 대해서 공부하기

        // 슬로우모션 해제 (시간 복원)
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;  // 물리 업데이트 속도 복원

        // 쿨다운 대기 (10초)
        yield return new WaitForSecondsRealtime(cooldownDuration);
        //위의 animation을 - 속도를 부여하면서 5배 느리게 적용하여서 아마도 -5속도 아니면.. -1/5 속도
        //반대로 구현하기

        //10초동안 진행될 animation 삽입.

        // 슬로우모션 다시 사용 가능
        isSlowMotionReady = true;
    }

        public void ActivateSlowMotionButton()
    {
        if (isSlowMotionReady)
        {
            StartCoroutine(ActivateSlowMotion());
        }
    }
}
