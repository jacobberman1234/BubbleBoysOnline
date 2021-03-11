using Mirror;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int _health = 100;

    public bool IsAlive { get; private set; }

    [Client]
    void Start() => IsAlive = true;

    [Client]
    public void DealDamage(int damage)
    {
        if (_health <= 0 && IsAlive)
        {
            IsAlive = false;
            return;
        }
        if (_health <= 0)
            return;
        _health -= damage;
        Debug.Log("Damaged player");
    }
}
