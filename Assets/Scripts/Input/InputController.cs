using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed = 10f;

    [SerializeField]
    private float minDistance = 0.001f;

    [SerializeField]
    private float raycastDistance = 15f;
    
    [SerializeField]
    private LayerMask inputHitLayerMask = 0;

    private Vector3 pointerPosition;
    private Vector3 inputWorldPosition;
    private Vector2 inputViewportPosition;

    private void Start()
    {
        InitialiseInputPositions();
    }
    
    private void Update()
    {
        UpdateInputPositions();
        UpdatePlayerPositions();
    }
    
    private void InitialiseInputPositions()
    {
        inputWorldPosition = GetPlayerWorldPosition();
        inputViewportPosition = CalculateViewportPosition(inputWorldPosition);
        SetPlayerViewportPosition(inputViewportPosition);
    }

    private void UpdateInputPositions()
    {
        if (Input.GetMouseButton(0) && pointerPosition != Input.mousePosition)
        {
            pointerPosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, inputHitLayerMask))
            {
                inputWorldPosition = hit.point;
                inputViewportPosition = CalculateViewportPosition(inputWorldPosition);
            }
        }
    }
    
    private void UpdatePlayerPositions()
    {
        Vector3 nextWorldPosition = Vector3.Lerp(GetPlayerWorldPosition(), inputWorldPosition, Time.smoothDeltaTime * maxSpeed);

        if (Vector3.Distance(GetPlayerWorldPosition(), nextWorldPosition) >= minDistance)
        {
            SetPlayerWorldPosition(nextWorldPosition);
            SetPlayerViewportPosition(CalculateViewportPosition(nextWorldPosition));
        }
    }

    private Vector2 CalculateViewportPosition(Vector3 worldPosition)
    {
        return Camera.main.WorldToViewportPoint(worldPosition);
    }

    private Vector3 GetPlayerWorldPosition()
    {
        return State.Instance.Player.WorldPosition.Value;
    }

    private void SetPlayerWorldPosition(Vector3 value)
    {
        State.Instance.Player.WorldPosition.Value = value;
    }
    
    private void SetPlayerViewportPosition(Vector2 value)
    {
        State.Instance.Player.ViewportPosition.Value = value;
    }
}
