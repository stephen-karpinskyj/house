using UnityEngine;

public class InputController : BaseMonoBehaviour
{
    [SerializeField, Tooltip("Minimum world-space distance that pointer needs to move for input to be detected.")]
    private float minDistance = 0.001f;
    
    [SerializeField, Tooltip("Maximum world-space distance that pointer can move before it is divided into a segment.")]
    private float maxDistance = 0.1f;

    [SerializeField]
    private float maxRaycastDistance = 15f;

    [SerializeField]
    private LayerMask playerLayerMask = 0;

    [SerializeField]
    private LayerMask floorLayerMask = 0;

    private Vector3 lastWorldPosition;
    private Vector3 worldPosition;

    // TODO: Use mouse cursor or first touch only as pointer

    private bool Pointing => Input.GetMouseButton(0);

    private Vector3 PointerPosition => Input.mousePosition;

    private bool PointerDown => Input.GetMouseButtonDown(0);

    private bool PointerUp => Input.GetMouseButtonUp(0);

    private int WorldPositionsCount
    {
        set => State.Instance.Input.WorldPositionsCount.Value = value;
    }

    private Vector3[] WorldPositions => State.Instance.Input.WorldPositions.Value;

    private Vector3 WorldPositionPlayerOffset
    {
        get => State.Instance.Input.WorldPositionPlayerOffset.Value;
        set => State.Instance.Input.WorldPositionPlayerOffset.Value = value;
    }

    private Vector3 PlayerWorldPosition => State.Instance.Player.WorldPosition.Value;

    private bool PlayerGrabbed
    {
        get => State.Instance.Player.Grabbed.Value;
        set => State.Instance.Player.Grabbed.Value = value;
    }
    
    private void Update()
    {
        if (Pointing)
        {
            Ray ray = Camera.main.ScreenPointToRay(PointerPosition);
            RaycastHit hit;
            
            if (PointerDown)
            {
                if (Physics.Raycast(ray, out hit, maxRaycastDistance, playerLayerMask))
                {
                    PlayerGrabbed = true;
                }
            }
            
            if (PlayerGrabbed)
            {
                if (Physics.Raycast(ray, out hit, maxRaycastDistance, floorLayerMask))
                {
                    lastWorldPosition = worldPosition;
                    worldPosition = hit.point;
                    
                    if (PointerDown)
                    {
                        WorldPositionPlayerOffset = hit.point - PlayerWorldPosition;
                        lastWorldPosition = hit.point;
                    }

                    int count = UpdateWorldPositions();
                    
                    if (count <= 0)
                    {
                        worldPosition = lastWorldPosition;
                    }
                }
            }
        }

        if (PointerUp)
        {
            PlayerGrabbed = false;
        }
    }
    
    private int UpdateWorldPositions()
    {
        int count = 0;
        
        for (Vector3 pos = lastWorldPosition; Vector3.Distance(pos, worldPosition) >= minDistance; pos = Vector3.MoveTowards(pos, worldPosition, maxDistance))
        {
            WorldPositions[count] = pos;
            count++;
        }

        if (count > 0)
        {
            WorldPositionsCount = count;
            State.Instance.Input.WorldPositions.ForceUpdate();
        }

        return count;
    }
}
