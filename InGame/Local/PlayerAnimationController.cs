using Mirror;
using UnityEngine;

public class PlayerAnimationController : NetworkBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] NetworkAnimator _networkAnimator;

    Vector2 _previousInput;
    Controls _controls;
    Controls _Controls
    {
        get
        {
            if (_controls != null)
                return _controls;
            return _controls = new Controls();
        }
    }

    public override void OnStartAuthority()
    {
        enabled = true;
        _Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        _Controls.Player.Move.canceled += ctx => ResetMovement();
    }


    [ClientCallback]
    void OnEnable() => _Controls.Enable();
    [ClientCallback]
    void OnDisable() => _Controls.Disable();
    [ClientCallback]
    void Update() => Animate();

    [Client]
    void SetMovement(Vector2 movement) => _previousInput = movement;
    [Client]
    void ResetMovement() => _previousInput = Vector2.zero;
    [Client]
    void Animate()
    {
        float right = _previousInput.x;
        float forward = _previousInput.y;
        _animator.SetFloat("horizontal", right);
        _animator.SetFloat("vertical", forward);
    }
}
