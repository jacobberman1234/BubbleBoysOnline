using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    void Awake() => NetworkPlayerSpawnSystem.AddSpawnPoint(transform);
    void OnDestroy() => NetworkPlayerSpawnSystem.RemoveSpawnPoint(transform);

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
}
