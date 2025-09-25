using UnityEngine;

public enum ForceFalloffType
{
    Linear,         // 선형 감쇠
    Quadratic,      // 제곱 감쇠 (현재 방식)
    Exponential,    // 지수 감쇠
    Custom          // 커스텀 커브
}

public class PanchigiSettings : MonoBehaviour
{
    [Header("Simple Visualization")]
    [Tooltip("힘 표시 활성화")]
    public bool showForceVisualization = true;

    [Tooltip("힘 표시 지속 시간 (초)")]
    public float displayDuration = 1.0f;

    [Tooltip("텍스트 크기")]
    public float textSize = 0.1f;

    [Header("Contact Point Visualization")]
    [Tooltip("충돌 접촉점 표시")]
    public bool showContactPoints = true;

    [Tooltip("접촉점 표시 크기")]
    public float contactPointSize = 0.03f;

    [Header("Grid Raycast Debug")]
    [Tooltip("그리드 레이캐스트 디버깅 활성화")]
    public bool enableGridRaycast = false;

    [Tooltip("그리드 분할 수 (N x M)")]
    public Vector2Int gridDivisions = new Vector2Int(100, 160);

    [Tooltip("레이캐스트 거리")]
    public float raycastDistance = 3.0f;

    [Tooltip("접촉으로 판단할 거리 임계값")]
    public float touchThreshold = 0.01f;

    [Header("Force Settings")]
    [Tooltip("힘 감쇠 타입을 선택합니다.")]
    public ForceFalloffType forceFalloffType = ForceFalloffType.Quadratic;

    [Tooltip("힘 감쇠율. 값이 낮을수록 힘이 넓게 퍼지고, 높을수록 좁은 영역에 집중됩니다.")]
    public float forceFalloffRate = 1.0f;

    [Tooltip("최대 힘이 적용되는 거리 (이 거리 내에서는 최대 힘이 적용됨)")]
    public float maxForceDistance = 0.1f;

    [Tooltip("힘이 완전히 사라지는 거리")]
    public float minForceDistance = 5.0f;

    [Tooltip("커스텀 감쇠 커브. X축은 정규화된 거리(0~1), Y축은 힘 배율(0~1)")]
    public AnimationCurve customFalloffCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    public float forceMultiplier = 1f;

    public float horizontalForceMultiplier = 0.1f;

    private static PanchigiSettings instance;
    public static PanchigiSettings Instance
    {
        get
        {
            if (!Application.isPlaying)
            {
                return null;
            }

            if (instance == null)
            {
                GameObject obj = new GameObject("PanchigiSettings");
                instance = obj.AddComponent<PanchigiSettings>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// 거리에 따른 힘 감쇠 배율을 계산합니다.
    /// </summary>
    /// <param name="distance">힘의 중심점으로부터의 거리</param>
    /// <returns>0~1 사이의 힘 배율</returns>
    public float CalculateForceFalloff(float distance)
    {
        // 최대 힘 거리 내에서는 최대 힘 적용
        if (distance <= maxForceDistance)
        {
            return 1.0f;
        }

        // 최소 힘 거리를 넘으면 힘이 0
        if (distance >= minForceDistance)
        {
            return 0.0f;
        }

        // 정규화된 거리 계산 (0~1)
        float normalizedDistance = (distance - maxForceDistance) / (minForceDistance - maxForceDistance);

        switch (forceFalloffType)
        {
            case ForceFalloffType.Linear:
                return 1.0f - normalizedDistance;

            case ForceFalloffType.Quadratic:
                // 기존 공식을 개선한 버전
                return 1.0f / (1.0f + forceFalloffRate * normalizedDistance * normalizedDistance);

            case ForceFalloffType.Exponential:
                // 지수 감쇠 (물리적으로 더 현실적)
                return Mathf.Exp(-forceFalloffRate * normalizedDistance);

            case ForceFalloffType.Custom:
                // 커스텀 커브 사용
                return customFalloffCurve.Evaluate(normalizedDistance);

            default:
                return 1.0f - normalizedDistance;
        }
    }
}
