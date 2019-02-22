using UnityEngine;

[RequireComponent(typeof(ReflectionProbe))]
public class RealtimeReflection : BaseMonoBehaviour
{
    private ReflectionProbe probe;

    private void Awake()
    {
        probe = GetComponent<ReflectionProbe>();
    }

    private void Update()
    {
        Vector3 pos = Camera.main.transform.position;
        probe.transform.position = new Vector3(pos.x, pos.y * -1, pos.z);
        probe.RenderProbe();
    }
}
