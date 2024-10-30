using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ViewportHandler : MonoBehaviour
{
    #region FIELDS
    public Color wireColor = Color.white;
    public float UnitsSize = 1; // 기본 크기
    public float MinVisibleWidth = 2f; // 가로로 보여야 하는 최소 범위
    public float MinVisibleHeight = 3f; // 세로로 보여야 하는 최소 범위
    public Constraint constraint = Constraint.Portrait;
    public static ViewportHandler Instance;
    public new Camera camera;

    private float _width;
    private float _height;

    // 화면 각 지점
    private Vector3 _bl;
    private Vector3 _bc;
    private Vector3 _br;
    private Vector3 _ml;
    private Vector3 _mc;
    private Vector3 _mr;
    private Vector3 _tl;
    private Vector3 _tc;
    private Vector3 _tr;
    #endregion

    #region PROPERTIES
    public float Width { get { return _width; } }
    public float Height { get { return _height; } }

    // 화면 좌표들
    public Vector3 BottomLeft { get { return _bl; } }
    public Vector3 BottomCenter { get { return _bc; } }
    public Vector3 BottomRight { get { return _br; } }
    public Vector3 MiddleLeft { get { return _ml; } }
    public Vector3 MiddleCenter { get { return _mc; } }
    public Vector3 MiddleRight { get { return _mr; } }
    public Vector3 TopLeft { get { return _tl; } }
    public Vector3 TopCenter { get { return _tc; } }
    public Vector3 TopRight { get { return _tr; } }
    #endregion

    #region METHODS
    private void Awake()
    {
        camera = GetComponent<Camera>();
        Instance = this;
        ComputeResolution();
    }

    private void ComputeResolution()
    {
        float targetAspect = camera.aspect;

        // 우선 세로 기준으로 orthographicSize 설정
        camera.orthographicSize = MinVisibleHeight / 2f;

        // 세로 기준으로 설정된 카메라의 가로 크기 계산
        float calculatedWidth = camera.orthographicSize * 2 * targetAspect;

        // 가로가 최소 가로 크기보다 작으면 가로를 기준으로 다시 설정
        if (calculatedWidth < MinVisibleWidth)
        {
            camera.orthographicSize = MinVisibleWidth / (2f * targetAspect);
        }

        // 계산된 카메라 높이와 가로
        _height = 2f * camera.orthographicSize;
        _width = _height * targetAspect;

        float cameraX = camera.transform.position.x;
        float cameraY = camera.transform.position.y;

        float leftX = cameraX - _width / 2;
        float rightX = cameraX + _width / 2;
        float topY = cameraY + _height / 2;
        float bottomY = cameraY - _height / 2;

        // 화면의 각 지점들 업데이트
        _bl = new Vector3(leftX, bottomY, 0);
        _bc = new Vector3(cameraX, bottomY, 0);
        _br = new Vector3(rightX, bottomY, 0);
        _ml = new Vector3(leftX, cameraY, 0);
        _mc = new Vector3(cameraX, cameraY, 0);
        _mr = new Vector3(rightX, cameraY, 0);
        _tl = new Vector3(leftX, topY, 0);
        _tc = new Vector3(cameraX, topY, 0);
        _tr = new Vector3(rightX, topY, 0);
    }

    private void Update()
    {
        #if UNITY_EDITOR
        ComputeResolution();
        #endif
    }

    void OnDrawGizmos()
    {
        Gizmos.color = wireColor;

        Matrix4x4 temp = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        if (camera.orthographic)
        {
            float spread = camera.farClipPlane - camera.nearClipPlane;
            float center = (camera.farClipPlane + camera.nearClipPlane) * 0.5f;
            Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, spread));
        }
        else
        {
            Gizmos.DrawFrustum(Vector3.zero, camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
        }
        Gizmos.matrix = temp;
    }
    #endregion

    public enum Constraint { Landscape, Portrait }

    public float fixedBottomHeight = 0f; // 하단 끝을 고정할 Y 좌표

    void LateUpdate()
    {
        if (camera.orthographic)
        {
            // 정사각형 카메라의 경우
            float cameraHeight = camera.orthographicSize * 2; // 카메라의 전체 높이
            float newY = fixedBottomHeight + cameraHeight / 2; // 새로운 Y 위치 계산
            transform.position = new Vector3(transform.position.x, newY, transform.position.z); // 카메라 위치 조정
        }
        else
        {
            // 원근 카메라의 경우
            float cameraHeight = 2.0f * camera.nearClipPlane * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad); // 카메라의 높이 계산
            float newY = fixedBottomHeight + cameraHeight / 2; // 새로운 Y 위치 계산
            transform.position = new Vector3(transform.position.x, newY, transform.position.z); // 카메라 위치 조정
        }
    }
}