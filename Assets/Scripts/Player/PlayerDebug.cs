using System.Collections.Generic;
using UnityEngine;

public class PlayerDebug : BaseMonoBehaviour
{
    [SerializeField]
    private Transform markerPrefab = null;

    [SerializeField]
    private int markerPoolSize = 256;

    [SerializeField]
    private string markerPoolParentName = "DebugMarkerPool";

    private Transform markerPoolParent;
    private TransformPool markerPool;

    private StateSubscriber<LinkedList<Vector3>> playerWorldPositionPath;

    private void Awake()
    {
        markerPoolParent = GameObjectUtility.InstantiateGameObject(markerPoolParentName, transform).transform;

        for (int i = 0; i < markerPoolSize; i++)
        {
            Transform marker = GameObjectUtility.InstantiatePrefab(markerPrefab, markerPoolParent);
            marker.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        markerPoolParent = transform.Find(markerPoolParentName);
        markerPool = new TransformPool(markerPoolParent);

        playerWorldPositionPath.Subscribe(State.Instance.Player.WorldPositionPath, HandlePlayerWorldPositionPathChange);
    }

    private void OnDisable()
    {
        ResetMarkers();
        
        playerWorldPositionPath.Unsubscribe();
    }

    private void ResetMarkers()
    {
        markerPool.UnuseAll((obj) => obj.gameObject.SetActive(false));
    }

    private void HandlePlayerWorldPositionPathChange(LinkedList<Vector3> value)
    {
        ResetMarkers();

        LinkedListNode<Vector3> curr = value.Last;
        
        for (int i = 0; i < Mathf.Min(value.Count, markerPool.AvailableCount); i++)
        {
            if (markerPool.TryUseRandom(out Transform marker))
            {
                marker.position = curr.Value;
                marker.gameObject.SetActive(true);
            }

            curr = curr.Previous;
        }
    }
}
