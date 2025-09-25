using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ForceElement : MonoBehaviour
{
    public Dictionary<GameObject, CollisionInfo> collisionMap { get; } = new Dictionary<GameObject, CollisionInfo>();

    private List<Vector3> contactPoints = new List<Vector3>();

    public Dictionary<GameObject, List<Vector3>> contactPointMap { get; } = new Dictionary<GameObject, List<Vector3>>();


    private BoxCollider boxCollider;
    private Transform _transform;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        _transform = transform;
    }

    public void ResetForceElement()
    {
        collisionMap.Clear();
        contactPointMap.Clear();
        contactPoints.Clear();
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisionMap.Add(collision.gameObject, new CollisionInfo(collision));
        UpdateContactPoints(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        collisionMap[collision.gameObject] = new CollisionInfo(collision);
        UpdateContactPoints(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        contactPoints.Clear();

        contactPointMap[collision.gameObject].Clear();
        collisionMap.Remove(collision.gameObject);
    }

    /// <summary>
    /// 충돌 정보를 기반으로 중첩 영역을 계산하고 레이캐스트를 통해 접촉점을 업데이트합니다.
    /// </summary>
    private void UpdateContactPoints(Collision collision)
    {
        if (!contactPointMap.TryGetValue(collision.gameObject, out var points))
        {
            points = new List<Vector3>();
            contactPointMap[collision.gameObject] = points;
        }
        points.Clear();

        // 1. 충돌한 두 콜라이더의 경계(Bounds)를 가져옵니다.
        Bounds elementBounds = boxCollider.bounds;
        Bounds colliderBounds = collision.collider.bounds;

        // 2. 두 경계가 겹치는 영역(XZ 평면 기준)을 월드 좌표로 계산합니다.
        float minX = Mathf.Max(elementBounds.min.x, colliderBounds.min.x);
        float maxX = Mathf.Min(elementBounds.max.x, colliderBounds.max.x);
        float minZ = Mathf.Max(elementBounds.min.z, colliderBounds.min.z);
        float maxZ = Mathf.Min(elementBounds.max.z, colliderBounds.max.z);

        // 3. 겹치는 영역이 유효한 경우에만 레이캐스트를 수행합니다.
        if (minX < maxX && minZ < maxZ)
        {
            int gridX = PanchigiSettings.Instance.gridDivisions.x;
            int gridY = PanchigiSettings.Instance.gridDivisions.y;

            // 그리드 한 칸의 크기 계산 (분할 수가 1일 때의 예외 처리 포함)
            float stepX = (gridX > 1) ? (maxX - minX) / (gridX - 1) : 0;
            float stepZ = (gridY > 1) ? (maxZ - minZ) / (gridY - 1) : 0;

            // 레이캐스트 시작 높이 (ForceElement 상단보다 약간 위)
            float rayOriginY = boxCollider.bounds.max.y + -0.001f;

            for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridY; j++)
                {
                    // 1. 월드 좌표계에서 그리드 상의 각 시작점 계산
                    float worldX = minX + i * stepX;
                    float worldZ = minZ + j * stepZ;
                    Vector3 worldRayOrigin = new Vector3(worldX, rayOriginY, worldZ);

                    // 2. 레이캐스트 방향 설정 (아래 방향)
                    Vector3 rayDirection = Vector3.up;

                    // 3. 레이캐스트 수행
                    if (Physics.Raycast(worldRayOrigin, rayDirection, out var hit, PanchigiSettings.Instance.raycastDistance))
                    {
                        points.Add(hit.point);
                    }
                }
            }
        }
    }

    public void AddForce(float force, Vector3 forcePos)
    {
        foreach (var kvp in contactPointMap)
        {
            foreach (var point in kvp.Value)
            {
                float distance = Vector3.Distance(new Vector3(point.x, 0, point.z), new Vector3(forcePos.x, 0, forcePos.z));

                float falloff = 1f / (1f + PanchigiSettings.Instance.forceFalloffRate * distance * distance);
                //float falloff = PanchigiSettings.Instance.CalculateForceFalloff(distance);
                float intensity = force * PanchigiSettings.Instance.forceMultiplier * falloff;

                kvp.Key.GetComponent<Rigidbody>()?.AddExplosionForce(intensity, point, 1f, 0.9f, ForceMode.Impulse);
            }
        }
    }

    public void AddForce(Vector3 force, Vector3 forcePos)
    {
        foreach (var kvp in contactPointMap)
        {
            foreach (var point in kvp.Value)
            {
                float distance = Vector3.Distance(new Vector3(point.x, 0, point.z), new Vector3(forcePos.x, 0, forcePos.z));

                float falloff = 1f / (1f + PanchigiSettings.Instance.forceFalloffRate * distance * distance);
                //float falloff = PanchigiSettings.Instance.CalculateForceFalloff(distance);
                float y = force.y * PanchigiSettings.Instance.forceMultiplier * falloff;
                float x = force.x * PanchigiSettings.Instance.horizontalForceMultiplier * falloff;
                float z = force.z * PanchigiSettings.Instance.horizontalForceMultiplier * falloff;

                kvp.Key.GetComponent<Rigidbody>()?.AddForceAtPosition(new Vector3(x, y, z), forcePos);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (PanchigiSettings.Instance.showContactPoints)
        {
            DrawContactPoints();
        }
    }

    private void DrawContactPoints()
    {
        Color contactColor = Color.green;
        float sphereSize = PanchigiSettings.Instance.contactPointSize;

        Gizmos.color = contactColor;

        foreach (var kvp in contactPointMap)
        {
            foreach (var point in kvp.Value)
            {
                Gizmos.DrawSphere(point, sphereSize);
            }
        }
    }
}
