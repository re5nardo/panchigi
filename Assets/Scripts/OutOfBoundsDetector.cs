using System;
using UnityEngine;

public class OutOfBoundsDetector : MonoBehaviour
{
    [SerializeField] private CollisionDetector[] collisionDetectors;
    public event Action<Collider> onOutOfBoundsDetected;
    private void Awake()
    {
        foreach (var detector in collisionDetectors)
        {
            detector.onTriggerEnter += HandleTriggerEnter;
        }
    }

    private void HandleTriggerEnter(Collider other)
    {
        if (other.GetComponentInChildren<Rigidbody>() == null)
        {
            return;
        }

        onOutOfBoundsDetected?.Invoke(other);
    }
}
