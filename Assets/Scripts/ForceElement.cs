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
    /// �浹 ������ ������� ��ø ������ ����ϰ� ����ĳ��Ʈ�� ���� �������� ������Ʈ�մϴ�.
    /// </summary>
    private void UpdateContactPoints(Collision collision)
    {
        if (!contactPointMap.TryGetValue(collision.gameObject, out var points))
        {
            points = new List<Vector3>();
            contactPointMap[collision.gameObject] = points;
        }
        points.Clear();

        // 1. �浹�� �� �ݶ��̴��� ���(Bounds)�� �����ɴϴ�.
        Bounds elementBounds = boxCollider.bounds;
        Bounds colliderBounds = collision.collider.bounds;

        // 2. �� ��谡 ��ġ�� ����(XZ ��� ����)�� ���� ��ǥ�� ����մϴ�.
        float minX = Mathf.Max(elementBounds.min.x, colliderBounds.min.x);
        float maxX = Mathf.Min(elementBounds.max.x, colliderBounds.max.x);
        float minZ = Mathf.Max(elementBounds.min.z, colliderBounds.min.z);
        float maxZ = Mathf.Min(elementBounds.max.z, colliderBounds.max.z);

        // 3. ��ġ�� ������ ��ȿ�� ��쿡�� ����ĳ��Ʈ�� �����մϴ�.
        if (minX < maxX && minZ < maxZ)
        {
            int gridX = PanchigiSettings.Instance.gridDivisions.x;
            int gridY = PanchigiSettings.Instance.gridDivisions.y;

            // �׸��� �� ĭ�� ũ�� ��� (���� ���� 1�� ���� ���� ó�� ����)
            float stepX = (gridX > 1) ? (maxX - minX) / (gridX - 1) : 0;
            float stepZ = (gridY > 1) ? (maxZ - minZ) / (gridY - 1) : 0;

            // ����ĳ��Ʈ ���� ���� (ForceElement ��ܺ��� �ణ ��)
            float rayOriginY = boxCollider.bounds.max.y + -0.001f;

            for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridY; j++)
                {
                    // 1. ���� ��ǥ�迡�� �׸��� ���� �� ������ ���
                    float worldX = minX + i * stepX;
                    float worldZ = minZ + j * stepZ;
                    Vector3 worldRayOrigin = new Vector3(worldX, rayOriginY, worldZ);

                    // 2. ����ĳ��Ʈ ���� ���� (�Ʒ� ����)
                    Vector3 rayDirection = Vector3.up;

                    // 3. ����ĳ��Ʈ ����
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
