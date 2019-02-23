using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Extends player position path in response to input position changes.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerNavigation : BaseMonoBehaviour
{
    [SerializeField, Tooltip("Pathfind will succeed if path is less or equal to this length.")]
    private float maxPathfindDistance = 1f;

    [SerializeField, Tooltip("Pathfind will succeed if it has less or equal to this number of path corners.")]
    private int maxPathfindCorners = 2;

    [SerializeField, Tooltip("Maximum number of times a direct move to target position is attempted each frame.")]
    private int maxMoveLoops = 10;

    [SerializeField, Tooltip("Maximum distance of a direct move iteration.")]
    private float maxMoveDistance = 0.1f;

    [SerializeField, Tooltip("Minimum distance that a direct move iteration can be before iterations stop.")]
    private float minMoveDistance = 0.001f;

    [SerializeField, Tooltip("Node will be added to path indiscriminantly if it has less than this number of nodes.")]
    private int minPathNodes = 1;
    
    [SerializeField, Tooltip("Node will be added to path indiscriminantly if it is at least this distance from end of path.")]
    private float minPathNodeDistance = 0.001f;

    [SerializeField, Tooltip("Path end must be at least this direct distance from the start for the path to be simplified (ie. re-pathfinded).")]
    private float minSimplifyPathDistance = 1f;
    
    [SerializeField, Tooltip("All nodes after first node that is at least this direct distance to end of path will be simplified (ie. re-pathfinded).")]
    private float maxSimplifyPathDistance = 0.5f;

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
            bool moved = false;
            
            for (int i = 0; i < State.Instance.Input.WorldPositionsCount.Value; i++)
            {
                Vector3 targetPosition = value[i];

                if (TryPathfind(targetPosition) || TryMove(targetPosition))
                {
                    moved = true;
                }
            }

            if (moved && ExtendPlayerPath(agent.nextPosition))
            {
                SimplifyPlayerPath();
                State.Instance.Player.WorldPositionPath.ForceUpdate();
            }
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

    private bool ExtendPlayerPath(Vector3 targetPosition)
    {
        Vector3 currentPosition = WorldPositionPath.Last != null ? WorldPositionPath.Last.Value : WorldPosition;
        float targetDistance = Vector3.Distance(currentPosition, targetPosition);

        if (WorldPositionPath.Count < minPathNodes || targetDistance >= minPathNodeDistance)
        {
            agent.Warp(currentPosition);
            agent.CalculatePath(targetPosition, path);
            agent.Warp(targetPosition);

            pathCornerCount = path.GetCornersNonAlloc(pathCorners);
            for (int i = 0; i < pathCornerCount; i++)
            {
                WorldPositionPath.AddLast(pathCorners[i]);
            }
            
            return true;
        }

        return false;
    }

    private bool SimplifyPlayerPath()
    {
        bool simplify = false;
        LinkedListNode<Vector3> simplifyLimit = null;
        Vector3 pathEndPosition = WorldPositionPath.Last.Value;

        for (LinkedListNode<Vector3> node = WorldPositionPath.First; node != null; node = node.Next)
        {
            float distanceToEnd = Vector3.Distance(WorldPositionPath.Last.Value, node.Value);

            if (!simplify && distanceToEnd >= minSimplifyPathDistance)
            {
                simplify = true;
            }
            
            if (distanceToEnd <= maxSimplifyPathDistance)
            {
                simplifyLimit = node;
                break;
            }
        }

        if (simplify && simplifyLimit != null)
        {
            while (simplifyLimit != null)
            {
                LinkedListNode<Vector3> node = simplifyLimit.Next;
                WorldPositionPath.Remove(simplifyLimit);
                simplifyLimit = node;
            }

            return ExtendPlayerPath(pathEndPosition);
        }

        return false;
    }
}
