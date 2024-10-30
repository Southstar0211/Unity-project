using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class heightindicator : MonoBehaviour
{
    public GameObject indicator;        // indicator 오브젝트 참조
    public GameObject ball;             // ball 오브젝트 참조 (높이를 추적할 오브젝트)
    public TextMeshProUGUI heightText;  // UI 텍스트 (TextMeshProUGUI를 사용)
    public Vector3 offset;              // 텍스트의 위치 오프셋 (indicator와의 상대적 위치)


    void Update()
    {
        // ball의 높이를 실시간으로 가져와 텍스트에 표시
        float ballHeight = ball.transform.position.y;
        heightText.text = ballHeight.ToString("F2");

        // indicator의 월드 좌표를 화면 좌표로 변환하여 텍스트 위치 업데이트
        Vector3 indicatorScreenPosition = Camera.main.WorldToScreenPoint(indicator.transform.position);
        heightText.transform.position = indicatorScreenPosition;
    }

    public void ShowText()
    {
        heightText.gameObject.SetActive(true); // 텍스트를 보이게 함
    }

    public void HideText()
    {
        heightText.gameObject.SetActive(false); // 텍스트를 안 보이게 함
    }
}
