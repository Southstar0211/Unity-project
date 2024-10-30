using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outball : MonoBehaviour
{
    public GameObject ball;            // 공 오브젝트
    public GameObject indicator;       // 공 위치를 표시할 스프라이트 오브젝트
    public heightindicator HIScript;

    // 월드 좌표에서 공이 벗어났다고 판단할 Y 값을 설정
    private float yThreshold = 7.17f;   // 기본값: 월드 좌표에서 Y = 10.0f
    private float indicatorYOffset = -0.95f;  // 인디케이터의 Y 위치를 조정하는 오프셋 (기본값은 yThreshold 기준으로 0.5f 위에 표시)

    void Start()
    {
        // 인디케이터를 처음에는 비활성화
        if (indicator != null)
        {
            indicator.SetActive(false);
            HIScript.HideText();
            Debug.Log("Indicator 초기 상태: 비활성화");
        }
        else
        {
            Debug.LogError("Indicator 오브젝트가 설정되지 않았습니다!");
        }
    }

    void Update()
    {
        if (ball == null)
        {
            Debug.LogError("Ball 오브젝트가 설정되지 않았습니다!");
            return;
        }

        // 공의 월드 좌표를 얻음
        Vector3 ballWorldPos = ball.transform.position;

        // 공의 Y 좌표가 설정된 yThreshold 값을 넘었는지 확인
        if (ballWorldPos.y > yThreshold)
        {
            // 인디케이터의 위치를 yThreshold 기준으로 설정하고, X축은 공의 X축과 동일하게 유지
            Vector3 indicatorPosition = new Vector3(ballWorldPos.x, yThreshold + indicatorYOffset, 0);

            // 인디케이터의 위치를 업데이트
            indicator.transform.position = indicatorPosition;

            // 인디케이터를 활성화 (보이도록 설정)
            if (!indicator.activeSelf)
            {
                indicator.SetActive(true);
                HIScript.ShowText();
            }
        }
        else
        {
            // 공이 yThreshold보다 낮으면 인디케이터를 숨김
            if (indicator.activeSelf)
            {
                indicator.SetActive(false);
                HIScript.HideText();
            }
        }
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outball : MonoBehaviour
{
    public GameObject ball;           // 공 오브젝트
    public GameObject indicator;      // 공 위치를 표시할 스프라이트 오브젝트
    private Camera mainCamera;        // 메인 카메라
    public heightindicator HIScript;

    void Start()
    {
        // 메인 카메라를 찾음 (Inspector에서 설정할 필요가 없게 자동으로 할당)
        mainCamera = Camera.main;

        // 인디케이터를 처음에는 비활성화
        if (indicator != null)
        {
            indicator.SetActive(false);
            HIScript.HideText();
            Debug.Log("Indicator 초기 상태: 비활성화");
        }
        else
        {
            Debug.LogError("Indicator 오브젝트가 설정되지 않았습니다!");
        }

        // 카메라 확인
        if (mainCamera == null)
        {
            Debug.LogError("메인 카메라를 찾지 못했습니다. Camera.main을 확인하세요.");
        }
    }

    void Update()
    {
        if (ball == null)
        {
            Debug.LogError("Ball 오브젝트가 설정되지 않았습니다!");
            return;
        }

        // 공의 월드 좌표를 뷰포트 좌표로 변환 (Viewport 좌표는 0~1의 값)
        Vector3 ballViewportPos = mainCamera.WorldToViewportPoint(ball.transform.position);

        // 공이 화면의 위쪽을 벗어났는지 확인
        if (ballViewportPos.y > 1)
        {

            // 공의 X축을 유지한 채 카메라 상단에서 인디케이터의 위치를 설정
            Vector3 indicatorPosition = mainCamera.ViewportToWorldPoint(new Vector3(ballViewportPos.x, 0.95f, mainCamera.nearClipPlane + 1));

            // 인디케이터의 Z축을 0으로 설정 (카메라 앞에 위치시킴)
            indicatorPosition.z = 0;

            // 인디케이터의 위치를 업데이트
            indicator.transform.position = indicatorPosition;

            // 인디케이터를 활성화 (보이도록 설정)
            if (!indicator.activeSelf)
            {
                indicator.SetActive(true);
                HIScript.ShowText();
            }
        }
        else
        {
            // 공이 화면 안에 있으면 인디케이터를 숨김
            if (indicator.activeSelf)
            {
                indicator.SetActive(false);
            }
        }
    }

}
*/