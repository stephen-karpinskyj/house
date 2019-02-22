using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves player along position path.
/// </summary>
public class PlayerMovement : BaseMonoBehaviour
{
    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private float acceleration = 0.2f;

    [SerializeField]
    private float minPathNodeDistance = 0.01f;

    private float currentSpeed;
    
    private LinkedList<Vector3> WorldPositionPath => State.Instance.Player.WorldPositionPath.Value;

    private Vector3 WorldPosition
    {
        get => State.Instance.Player.WorldPosition.Value;
        set => State.Instance.Player.WorldPosition.Value = value;
    }

    private Quaternion WorldRotation
    {
        set => State.Instance.Player.WorldRotation.Value = value;
    }

    private void Start()
    {
        WorldPosition = transform.localPosition;
        WorldRotation = transform.localRotation;
    }

    private void Update()
    {
        if (State.Instance.Player.Grabbed.Value)
        {
            Accelerate();
            
            bool modifiedPath = false;
            float distanceLeft = currentSpeed * Time.unscaledDeltaTime;
            float distanceTravelled = 0f;
            Vector3 position = WorldPosition;

            while (WorldPositionPath.Count > 0)
            {
                LinkedListNode<Vector3> currNode = WorldPositionPath.First;

                Vector3 nextPosition = Vector3.MoveTowards(position, currNode.Value, distanceLeft);
                float nextDistance = Vector3.Distance(position, nextPosition);
                distanceLeft -= nextDistance;
                distanceTravelled += nextDistance;
                position = nextPosition;

                if (Vector3.Distance(nextPosition, currNode.Value) < minPathNodeDistance)
                {
                    WorldPositionPath.RemoveFirst();
                    modifiedPath = true;
                }
                else
                {
                    break;
                }
            }

            currentSpeed = distanceTravelled / Time.unscaledDeltaTime;

            if (distanceTravelled > 0f)
            {
                transform.localPosition = WorldPosition = position;
            }

            if (modifiedPath)
            {
                State.Instance.Player.WorldPositionPath.ForceUpdate();
            }
        }
        else
        {
            currentSpeed = 0f;
        }
    }
    
    
    private void Accelerate()
    {
        currentSpeed = Mathf.Min(speed, currentSpeed + acceleration);
    }
}
