using Mirror;
using UnityEngine;

public class PlayerMovementController : NetworkBehaviour
{   
    [SerializeField] float _movementSpeed = 5f;
    [SerializeField] CharacterController _controller;

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
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        _Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        _Controls.Player.Move.canceled += ctx => ResetMovement();
    }

    [ClientCallback]
    void OnEnable() => _Controls.Enable();
    [ClientCallback]
    void OnDisable() => _Controls.Disable();
    [ClientCallback]
    void Update() => Move();

    [Client]
    void SetMovement(Vector2 movement) => _previousInput = movement;
    [Client]
    void ResetMovement() => _previousInput = Vector2.zero;
    [Client]
    void Move()
    {
        Vector3 right = _controller.transform.right;
        Vector3 forward = _controller.transform.forward;
        right.y = 0;
        forward.y = 0;
        Vector3 movement = right.normalized * _previousInput.x + forward.normalized * _previousInput.y;
        _controller.Move(movement * _movementSpeed * Time.deltaTime);
    }
}
