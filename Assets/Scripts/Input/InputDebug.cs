using UnityEngine;

public class InputDebug : BaseMonoBehaviour
{
    [SerializeField]
    private Transform markerPrefab = null;

    [SerializeField]
    private int markerPoolSize = 64;

    [SerializeField]
    private string markerPoolParentName = "DebugMarkerPool";
    
    private Transform markerPoolParent;
    private TransformPool markerPool;

    private StateSubscriber<Vector3[]> inputWorldPositions;

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
        
        inputWorldPositions.Subscribe(State.Instance.Input.WorldPositions, HandleInputWorldPositionsChange);
    }

    private void OnDisable()
    {
        inputWorldPositions.Unsubscribe();
        markerPool.UnuseAll((obj) => obj.gameObject.SetActive(false));
    }

    private void HandleInputWorldPositionsChange(Vector3[] value)
    {
        markerPool.UnuseAll((obj) => obj.gameObject.SetActive(false));
        
        for (int i = 0; i < State.Instance.Input.WorldPositionsCount.Value; i++)
        {
            if (markerPool.TryUseRandom(out Transform marker))
            {
                marker.position = value[i];
                marker.gameObject.SetActive(true);
            }
        }
    }
}
