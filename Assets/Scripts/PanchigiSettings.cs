using UnityEngine;

public enum ForceFalloffType
{
    Linear,         // ���� ����
    Quadratic,      // ���� ���� (���� ���)
    Exponential,    // ���� ����
    Custom          // Ŀ���� Ŀ��
}

public class PanchigiSettings : MonoBehaviour
{
    [Header("Simple Visualization")]
    [Tooltip("�� ǥ�� Ȱ��ȭ")]
    public bool showForceVisualization = true;

    [Tooltip("�� ǥ�� ���� �ð� (��)")]
    public float displayDuration = 1.0f;

    [Tooltip("�ؽ�Ʈ ũ��")]
    public float textSize = 0.1f;

    [Header("Contact Point Visualization")]
    [Tooltip("�浹 ������ ǥ��")]
    public bool showContactPoints = true;

    [Tooltip("������ ǥ�� ũ��")]
    public float contactPointSize = 0.03f;

    [Header("Grid Raycast Debug")]
    [Tooltip("�׸��� ����ĳ��Ʈ ����� Ȱ��ȭ")]
    public bool enableGridRaycast = false;

    [Tooltip("�׸��� ���� �� (N x M)")]
    public Vector2Int gridDivisions = new Vector2Int(100, 160);

    [Tooltip("����ĳ��Ʈ �Ÿ�")]
    public float raycastDistance = 3.0f;

    [Tooltip("�������� �Ǵ��� �Ÿ� �Ӱ谪")]
    public float touchThreshold = 0.01f;

    [Header("Force Settings")]
    [Tooltip("�� ���� Ÿ���� �����մϴ�.")]
    public ForceFalloffType forceFalloffType = ForceFalloffType.Quadratic;

    [Tooltip("�� ������. ���� �������� ���� �а� ������, �������� ���� ������ ���ߵ˴ϴ�.")]
    public float forceFalloffRate = 1.0f;

    [Tooltip("�ִ� ���� ����Ǵ� �Ÿ� (�� �Ÿ� �������� �ִ� ���� �����)")]
    public float maxForceDistance = 0.1f;

    [Tooltip("���� ������ ������� �Ÿ�")]
    public float minForceDistance = 5.0f;

    [Tooltip("Ŀ���� ���� Ŀ��. X���� ����ȭ�� �Ÿ�(0~1), Y���� �� ����(0~1)")]
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
    /// �Ÿ��� ���� �� ���� ������ ����մϴ�.
    /// </summary>
    /// <param name="distance">���� �߽������κ����� �Ÿ�</param>
    /// <returns>0~1 ������ �� ����</returns>
    public float CalculateForceFalloff(float distance)
    {
        // �ִ� �� �Ÿ� �������� �ִ� �� ����
        if (distance <= maxForceDistance)
        {
            return 1.0f;
        }

        // �ּ� �� �Ÿ��� ������ ���� 0
        if (distance >= minForceDistance)
        {
            return 0.0f;
        }

        // ����ȭ�� �Ÿ� ��� (0~1)
        float normalizedDistance = (distance - maxForceDistance) / (minForceDistance - maxForceDistance);

        switch (forceFalloffType)
        {
            case ForceFalloffType.Linear:
                return 1.0f - normalizedDistance;

            case ForceFalloffType.Quadratic:
                // ���� ������ ������ ����
                return 1.0f / (1.0f + forceFalloffRate * normalizedDistance * normalizedDistance);

            case ForceFalloffType.Exponential:
                // ���� ���� (���������� �� ������)
                return Mathf.Exp(-forceFalloffRate * normalizedDistance);

            case ForceFalloffType.Custom:
                // Ŀ���� Ŀ�� ���
                return customFalloffCurve.Evaluate(normalizedDistance);

            default:
                return 1.0f - normalizedDistance;
        }
    }
}
