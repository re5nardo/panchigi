using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private const int MouseId = -1; // 마우스 입력을 위한 고정 ID

    [Header("Settings")]
    public Camera targetCamera;
    public LayerMask clickableLayers = -1;

    [Header("Thresholds")]
    [SerializeField] private float dragThreshold = 10f; // 드래그로 판정할 최소 거리
    [SerializeField] private float holdThreshold = 0.5f; // 홀드로 판정할 최소 시간

    // 현재 입력 상태
    private Dictionary<int, InputData> currentInputMap = new Dictionary<int, InputData>();

    public System.Action<InputData> OnInputEnd;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        OnInputEnd += HandleInputEnd;
    }

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#elif UNITY_IOS || UNITY_ANDROID
        HandleTouchInput();
#endif
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartInput(MouseId, Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            UpdateInput(MouseId, Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndInput(MouseId, Input.mousePosition);
        }
    }

    private void HandleTouchInput()
    {
        foreach (var touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartInput(touch.fingerId, touch.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    UpdateInput(touch.fingerId, touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    EndInput(touch.fingerId, touch.position);
                    break;
            }
        }
    }

    private void StartInput(int id, Vector2 screenPosition)
    {
        InputData inputData = new InputData { id = id };
        inputData.inputStartTime = Time.time;
        inputData.state = InputState.Begin;
        inputData.screenPosition = screenPosition;

        Ray ray = targetCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, clickableLayers))
        {
            inputData.targetObject = hit.collider.gameObject;
            inputData.hitInfo = hit;
            inputData.startPosition = hit.point;
            inputData.currentPosition = hit.point;
        }

        currentInputMap.Add(id, inputData);
    }

    private void UpdateInput(int id, Vector2 screenPosition)
    {
        currentInputMap.TryGetValue(id, out var inputData);

        inputData.state = inputData.screenPosition == screenPosition ? InputState.Hold : InputState.Move;
        inputData.screenPosition = screenPosition;

        Ray ray = targetCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, clickableLayers))
        {
            inputData.targetObject = hit.collider.gameObject;
            inputData.hitInfo = hit;
            inputData.currentPosition = hit.point;
        }
        else
        {
            inputData.targetObject = null;
        }
    }

    private void EndInput(int id, Vector2 screenPosition)
    {
        currentInputMap.TryGetValue(id, out var inputData);

        inputData.state = InputState.End;
        inputData.screenPosition = screenPosition;
        Ray ray = targetCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, clickableLayers))
        {
            inputData.targetObject = hit.collider.gameObject;
            inputData.hitInfo = hit;
            inputData.currentPosition = hit.point;
        }
        else
        {
            inputData.targetObject = null;
        }
        inputData.holdTime = Time.time - inputData.inputStartTime;
        inputData.state = InputState.End;

        currentInputMap.Remove(id);

        OnInputEnd?.Invoke(inputData);
    }

    private void HandleInputEnd(InputData data)
    {
        Debug.Log($"Touch End: {data.targetObject?.name ?? "none"} (time: {data.holdTime:F2}sec)");

        if (data.targetObject == null || !data.targetObject.TryGetComponent<ForceElement>(out var forceElement))
        {
            return;
        }

        Vector3 direction = data.currentPosition - data.startPosition;
        forceElement.AddForce(new Vector3(direction.x, data.holdTime, direction.z), data.hitInfo.point);

        TextUI.ShowText($"{data.holdTime:F2}", Camera.main.WorldToScreenPoint(data.currentPosition), 1f, 8f);
    }
}
