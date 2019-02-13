using UnityEngine;

public static class GameObjectUtility
{
    public static GameObject InstantiateGameObject(string name = null, Transform parent = null)
    {
        GameObject go = new GameObject(name);

        if (parent)
        {
            go.transform.SetParent(parent, true);
        }

        return go;
    }

    public static T InstantiateComponent<T>(Transform parent = null) where T : Component
    {
        return InstantiateGameObject(typeof(T).Name, parent).AddComponent<T>();
    }

    public static T InstantiatePrefab<T>(T prefab, Transform parent = null, bool worldPositionStays = true) where T : Component
    {
        Debug.Assert(prefab != null, prefab);

        T t = Object.Instantiate(prefab);

        Debug.Assert(t != null, prefab);

        if (parent)
        {
            t.transform.SetParent(parent, worldPositionStays);
        }
        
        return t;
    }
}
