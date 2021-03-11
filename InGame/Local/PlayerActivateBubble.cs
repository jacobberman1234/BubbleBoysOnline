using Mirror;
using UnityEngine;

public class PlayerActivateBubble : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController _controller;
    [SerializeField] PlayerGravityController _gravity;
    [SerializeField] PlayerBubbleController _bubble;

    [Header("Settings")]
    [SerializeField] float _bubbleForce = 3f;

    Vector3 _velocity;
    bool _usingBubble;
    float _previousInput;

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
        _Controls.Player.Bubble.performed += ctx => BubbleStarted(ctx.ReadValue<float>());
        _Controls.Player.Bubble.canceled += ctx => ResetBubble();
    }

    [ClientCallback]
    void OnEnable() => _Controls.Enable();
    [ClientCallback]
    void OnDisable() => _Controls.Disable();
    [ClientCallback]
    void Update() => UseBubble();

    [Client]
    void BubbleStarted(float movement) => _previousInput = movement;
    [Client]
    void ResetBubble() => _previousInput = 0f;

    [Client]
    void UseBubble()
    {
        if(_previousInput != 0 && _bubble.ReturnBubbleTime() > 0)
        {
            _gravity.ToggleScript(false);
            _usingBubble = true;
            _velocity.y = _bubbleForce;
            _controller.Move(_velocity * Time.deltaTime);
        }
        else
        {
            _gravity.ToggleScript(true);
            _velocity = Vector3.zero;
            _usingBubble = false;
        }
    }

    [Client]
    public bool UsingBubble() { return _usingBubble; }

}
