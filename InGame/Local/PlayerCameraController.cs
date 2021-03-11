using Cinemachine;
using Mirror;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] Vector2 _maxFollowOffset = new Vector2(-1f, 6f);
    [SerializeField] Vector2 _cameraVelocity = new Vector2(4f, 0.25f);
    [SerializeField] Transform _playerTransform;
    [SerializeField] CinemachineVirtualCamera _virtualCamera;

    CinemachineTransposer _transposer;
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
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _virtualCamera.gameObject.SetActive(true);
        enabled = true;

        _Controls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
    }

    [ClientCallback]
    void OnEnable() => _Controls.Enable();
    [ClientCallback]
    void OnDisable() => _Controls.Disable();

    void Look(Vector2 lookAxis)
    {
        float deltaTime = Time.deltaTime;
        float followOffset = Mathf.Clamp(
            _transposer.m_FollowOffset.y - (lookAxis.y * _cameraVelocity.y * deltaTime),
            _maxFollowOffset.x,
            _maxFollowOffset.y);
        _transposer.m_FollowOffset.y = followOffset;
        _playerTransform.Rotate(0f, lookAxis.x * _cameraVelocity.x * deltaTime, 0f);
    }
}
