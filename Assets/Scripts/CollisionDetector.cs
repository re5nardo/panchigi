using System;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public event Action<Collider> onTriggerEnter;
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }
}
