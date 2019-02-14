using UnityEngine;

/// <summary>
/// Converts user input to player movement state changes.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed = 10f;

    [SerializeField]
    private float minDistance = 0.001f;

    [SerializeField]
    private LayerMask inputHitLayerMask = 0;

    private Vector3 prevPointerPosition;
    private Vector3 prevInputPosition;

    private void Start()
    {
        prevInputPosition = GetPlayerPosition();
    }
    
    private void Update()
    {
        Vector3 nextPosition = Vector3.Lerp(GetPlayerPosition(), GetInputPosition(), Time.smoothDeltaTime * maxSpeed);

        if (Vector3.Distance(GetPlayerPosition(), nextPosition) >= minDistance)
        {
            SetPlayerPosition(nextPosition);
        }
    }
    
    private Vector3 GetPlayerPosition()
    {
        return State.Instance.PlayerPosition.Value;
    }

    private void SetPlayerPosition(Vector3 value)
    {
        State.Instance.PlayerPosition.Value = value;
    }

    private Vector3 GetInputPosition()
    {   
        if (Input.GetMouseButton(0) && prevPointerPosition != Input.mousePosition)
        {
            prevPointerPosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(prevPointerPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 10f, inputHitLayerMask))
            {
                prevInputPosition = hit.point;
            }
        }
        
        return prevInputPosition;
    }
}
