using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Extends player position path in response to input position changes.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerNavigation : BaseMonoBehaviour
{
    [SerializeField]
    private float maxPathfindDistance = 1f;

    [SerializeField]
    private int maxPathfindCorners = 2;

    [SerializeField]
    private float minMoveDistance = 0.001f;

    [SerializeField]
    private float maxMoveDistance = 0.1f;

    [SerializeField]
    private int maxMoveLoops = 10;

    [SerializeField]
    private float minPathNodeDistance = 0.001f;

    private NavMeshAgent agent;
    private NavMeshPath path;
    private readonly Vector3[] pathCorners = new Vector3[64];
    private float pathDistance;
    private int pathCornerCount;

    private StateSubscriber<bool> playerGrabbed;
    private StateSubscriber<Vector3[]> inputWorldPositions;
    
    private LinkedList<Vector3> WorldPositionPath => State.Instance.Player.WorldPositionPath.Value;
    private Vector3 WorldPosition => State.Instance.Player.WorldPosition.Value;

    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        path = new NavMeshPath();
        
        playerGrabbed.Subscribe(State.Instance.Player.Grabbed, HandlePlayerGrabbedChange);
        inputWorldPositions.Subscribe(State.Instance.Input.WorldPositions, HandleInputWorldPositionsChange);
    }

    private void OnDisable()
    {
        playerGrabbed.Unsubscribe();
        inputWorldPositions.Unsubscribe();
    }

    private void HandlePlayerGrabbedChange(bool value)
    {
        if (value)
        {
            agent.Warp(WorldPosition);

            WorldPositionPath.Clear();
            State.Instance.Player.WorldPositionPath.ForceUpdate();
        }
    }

    private void HandleInputWorldPositionsChange(Vector3[] value)
    {
        if (playerGrabbed.Value)
        {
            for (int i = 0; i < State.Instance.Input.WorldPositionsCount.Value; i++)
            {
                Vector3 targetPosition = value[i];

                if (TryPathfind(targetPosition) || TryMove(targetPosition)) { }
            }

            UpdatePlayerPath(agent.nextPosition);
        }
    }

    private bool TryPathfind(Vector3 targetPosition)
    {
        if (agent.CalculatePath(targetPosition, path))
        {
            pathDistance = path.CalculateDistance(pathCorners, out pathCornerCount);

            if (pathDistance <= maxPathfindDistance || pathCornerCount <= maxPathfindCorners)
            {
                agent.Warp(targetPosition);
                return true;
            }
        }

        return false;
    }

    private bool TryMove(Vector3 targetPosition)
    {
        Vector3 currentPosition;
        
        for (int i = 0; i < maxMoveLoops; i++)
        {
            currentPosition = agent.nextPosition;
            Vector3 moveVector = (targetPosition - currentPosition).normalized * maxMoveDistance;
            agent.Move(moveVector);
            
            if (Vector3.Distance(currentPosition, agent.nextPosition) < minMoveDistance)
            {
                break;
            }
        }
        
        return true;
    }

    private void UpdatePlayerPath(Vector3 targetPosition)
    {
        Vector3 currentPosition = WorldPositionPath.Last != null ? WorldPositionPath.Last.Value : WorldPosition;
        
        if (Vector3.Distance(currentPosition, targetPosition) >= minPathNodeDistance)
        {
            agent.Warp(currentPosition);
            agent.CalculatePath(targetPosition, path);
            agent.Warp(targetPosition);

            pathCornerCount = path.GetCornersNonAlloc(pathCorners);
            for (int i = 0; i < pathCornerCount; i++)
            {
                WorldPositionPath.AddLast(pathCorners[i]);
            }
            State.Instance.Player.WorldPositionPath.ForceUpdate();
        }
    }
}
