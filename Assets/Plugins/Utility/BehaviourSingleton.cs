using UnityEngine;

public abstract class BehaviourSingleton<T> : BaseMonoBehaviour where T : BaseMonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<T>();

                if (!instance)
                {
                    instance = GameObjectUtility.InstantiateComponent<T>();
                }

                Debug.Assert(instance != null, instance);
                DontDestroyOnLoad(instance);
            }

            return instance;
        }
    }

    public static bool Exists => instance;
}
