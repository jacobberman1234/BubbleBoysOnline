using Mirror;
using UnityEngine;

public class PlayerBubbleController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] PlayerActivateBubble _activateBubble;

    [Header("Settings")]
    [SerializeField] float _baseBubbleTimer = 100f;
    [SerializeField] float _baseTimerRefreshSpeed = 50f;

    float _bubbleTimer, _refreshTimer;

    public override void OnStartAuthority() => enabled = true;

    [ClientCallback]
    void Start() => ResetTimers();
    [ClientCallback]
    void Update() => BubbleTimer();

    [Client]
    void ResetTimers()
    {
        _bubbleTimer = _baseBubbleTimer;
        _refreshTimer = _baseTimerRefreshSpeed;
    }

    [Client]
    void BubbleTimer()
    {
        float adjustedDeltaTime = Time.deltaTime * 25f;
        if (!_activateBubble.UsingBubble())
        {
            if (_refreshTimer > 0)
                _refreshTimer -= adjustedDeltaTime;
            else
            {
                if (_bubbleTimer < _baseBubbleTimer)
                    _bubbleTimer += adjustedDeltaTime;
                else
                    _bubbleTimer = _baseBubbleTimer;
            }

        }
        else
        {
            _refreshTimer = _baseBubbleTimer;
            _bubbleTimer -= adjustedDeltaTime;
        }
    }

    [Client]
    public void DamageBubble(int damage)
    {
        _bubbleTimer -= damage;
    }

    [Client]
    public float ReturnBubbleTime() { return _bubbleTimer; }
}
