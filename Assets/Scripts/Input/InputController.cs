using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class InputController : BaseMonoBehaviour
{
    [SerializeField]
    private float minMoveDistance = 0.001f;

    [SerializeField]
    private float minTurnDistance = 0.02f;

    [SerializeField]
    private float raycastDistance = 15f;

    [SerializeField]
    private LayerMask playerLayerMask = 0;
    
    [SerializeField]
    private LayerMask floorLayerMask = 0;

    private NavMeshAgent navMeshAgent;

    private Vector3 lastTurnWorldPosition;
    private Vector3 inputWorldPosition;
    private Vector3 inputWorldPositionOffset;

    // TODO: Use mouse cursor or first touch only as pointer

    private bool Pointing => Input.GetMouseButton(0);

    private Vector3 PointerPosition => Input.mousePosition;

    private bool PointerDown => Input.GetMouseButtonDown(0);

    private bool PointerUp => Input.GetMouseButtonUp(0);

    private Vector3 PlayerWorldPosition
    {
        get => State.Instance.Player.WorldPosition.Value;
        set => State.Instance.Player.WorldPosition.Value = value;
    }

    private Quaternion PlayerWorldRotation
    {
        set => State.Instance.Player.WorldRotation.Value = value;
    }

    private bool PlayerDragging
    {
        get => State.Instance.Player.Dragging.Value;
        set => State.Instance.Player.Dragging.Value = value;
    }
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
    }

    private void Start()
    {
        PlayerWorldPosition = transform.localPosition;
        PlayerWorldRotation = transform.localRotation;
        navMeshAgent.Warp(transform.localPosition);
    }
    
    private void Update()
    {
        if (HandleInput())
        {
            Move();
            Turn();
        }
    }

    private bool HandleInput()
    {
        if (Pointing)
        {
            Ray ray = Camera.main.ScreenPointToRay(PointerPosition);
            RaycastHit hit;
            
            if (PointerDown)
            {
                if (Physics.Raycast(ray, out hit, raycastDistance, playerLayerMask))
                {
                    PlayerDragging = true;
                }
            }

            if (PlayerDragging)
            {
                if (Physics.Raycast(ray, out hit, raycastDistance, floorLayerMask))
                {
                    if (PointerDown)
                    {
                        inputWorldPositionOffset = hit.point - PlayerWorldPosition;
                    }

                    inputWorldPosition = hit.point - inputWorldPositionOffset;
                    
                    if (PointerDown)
                    {
                        lastTurnWorldPosition = inputWorldPosition;
                    }
                }

                return true;
            }
        }
        
        if (PointerUp)
        {
            PlayerDragging = false;
        }

        return false;
    }
    
    private void Move()
    { 
        Vector3 currWorldPosition = PlayerWorldPosition;
        
        if (Vector3.Distance(inputWorldPosition, currWorldPosition) >= minMoveDistance)
        {
            Vector3 nextWorldPosition = Vector3.MoveTowards(currWorldPosition, inputWorldPosition, Time.deltaTime * navMeshAgent.speed);
            Vector3 moveVector = nextWorldPosition - currWorldPosition;
            navMeshAgent.Move(moveVector);
            PlayerWorldPosition = navMeshAgent.nextPosition;

            inputWorldPositionOffset = Vector3.MoveTowards(inputWorldPositionOffset, Vector3.zero, moveVector.magnitude);
        }
    }

    private void Turn() 
    {
        if (Vector3.Distance(inputWorldPosition, lastTurnWorldPosition) >= minTurnDistance)
        {
            Vector3 lookVector = (inputWorldPosition - lastTurnWorldPosition).normalized;
            PlayerWorldRotation = Quaternion.LookRotation(lookVector);

            lastTurnWorldPosition = inputWorldPosition;
        }
    }
}
