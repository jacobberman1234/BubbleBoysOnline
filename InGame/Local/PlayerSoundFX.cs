using Mirror;
using UnityEngine;

public class PlayerSoundFX : NetworkBehaviour
{
    [SerializeField] AudioSource _source;

    public override void OnStartAuthority() => enabled = true;

    public void PlayClip()
    {
        _source.Play();
    }
}
