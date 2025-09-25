using UnityEngine;

public enum InputState
{
    None,
    Begin,
    Hold,
    Move,
    End
}

[System.Serializable]
public class InputData
{
    public int id;
    public InputState state;
    public Vector3 startPosition;
    public Vector3 currentPosition;
    public Vector2 screenPosition;
    public float holdTime;
    public float inputStartTime;

    public float totalDistance => Vector3.Distance(startPosition, currentPosition);
    public GameObject targetObject;
    public RaycastHit hitInfo;

    public void Reset()
    {
        state = InputState.None;
        startPosition = Vector3.zero;
        currentPosition = Vector3.zero;
        holdTime = 0f;
        inputStartTime = 0f;
        targetObject = null;
    }
}

public class CollisionInfo
{
    public ContactPoint[] contacts;
    public Rigidbody rigidbody;
    public GameObject gameObject;

    public CollisionInfo(Collision collision)
    {
        this.contacts = collision.contacts;
        this.rigidbody = collision.rigidbody;
        this.gameObject = collision.gameObject;
    }
}

public enum CoinFormation
{
    FourInLine = 0,
    SixInLine = 1,
    FourByTwo = 2,
    SixByTwo = 3,
}
