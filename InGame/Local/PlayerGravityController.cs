using Mirror;
using UnityEngine;

public class PlayerGravityController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController _controller;
    [SerializeField] Transform _groundCheck;
    [SerializeField] LayerMask _groundMask;

    [Header("Settings")]
    [SerializeField] float _jumpForce = 10f;
    [SerializeField] float _groundRadius = 0.5f;

    readonly float _gravity = Physics.gravity.y;
    Vector3 _velocity;

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
    }

    [ClientCallback]
    void OnEnable() => _Controls.Enable();
    [ClientCallback]
    void OnDisable() => _Controls.Disable();
    [ClientCallback]
    void Update() => ProcessGravity();

    [Client]
    void ProcessGravity()
    {
        if (IsGrounded() && _velocity.y < 0)
            _velocity.y = -2;
        if (_Controls.Player.Jump.triggered && IsGrounded())
            _velocity.y = Mathf.Sqrt(_jumpForce * -2 * _gravity);

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }



    [Client]
    public bool IsGrounded()
    {
        return Physics.CheckSphere(_groundCheck.position, _groundRadius, _groundMask);
    }

    public void ToggleScript(bool value) { enabled = value; }
}
