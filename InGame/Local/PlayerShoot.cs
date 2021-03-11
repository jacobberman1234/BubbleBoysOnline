using Mirror;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{

    public override void OnStartAuthority() => enabled = true;

    [ClientCallback]
    void Update() => Shoot(10);

    [Client]
    void Shoot(int damage)
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject rayHit = HelperBBO.ShootRay(100, Camera.main);
            if (rayHit == null)
                return;
            if(rayHit.GetComponent<PlayerHealth>() != null)
                rayHit.GetComponent<PlayerHealth>().DealDamage(damage);
            if (rayHit.GetComponent<Bubble>())
                rayHit.transform.parent.GetComponent<PlayerBubbleController>().DamageBubble(damage);
        }
    }
}
