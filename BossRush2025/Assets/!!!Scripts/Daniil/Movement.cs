using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private enum State { Run, Ritual };
    private State _currentState = State.Run;    

    private Rigidbody2D _playerRb;
    [SerializeField] private float _speed;
    private bool _disable;
    [SerializeField] private InputActionReference _moveInput;
    [SerializeField] private float _hitStunTime;

    [Header("Dash Properties")]
    [SerializeField] private InputActionReference _dashKey;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime = 0.15f;
    [SerializeField] private float _dashDelay = 1f; 
    private bool _canDash = true;
    private float _defaultSpeed;

    [Header("Ritual Properties")]
    public Transform _center;
    public float _radius = 5.0f;
    public float _ritualSpeed = 5.0f;

    private float _currentAngle;

    private Knockback _knockBack;
    private HealthManager _healthManager;

    void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _healthManager = GetComponent<HealthManager>();
        _knockBack = GetComponent<Knockback>();
        
        _knockBack._onStartKnockback += ()=>{ _disable = true; _healthManager.DisableReceivingDamage(); };
        _knockBack._onFinishKnockback += ()=>{ _disable = false; _healthManager.EnableReceivingDamage(); };

        _dashKey.action.performed += Dash;

        _defaultSpeed = _speed;
    }

    private Vector2 Direction() => _moveInput.action.ReadValue<Vector2>();
    void FixedUpdate()
    {
        switch (_currentState)
        {
            case State.Run:
                if (!_disable)
                {
                    _playerRb.linearVelocity = Direction() * _speed;
                }
                RotatePlayer();
                break;

            case State.Ritual:
                _currentAngle += _ritualSpeed * Time.deltaTime * Direction().x;
                transform.position = (Vector2)_center.position + new Vector2(Mathf.Cos(_currentAngle), Mathf.Sin(_currentAngle)) * _radius;
                break;
        }
    }
    public void TakeHit()
    {
        if (!_disable)
            StartCoroutine(Stun(_hitStunTime));
    }
    private IEnumerator Stun(float time)
    {
        _disable = true;
        yield return new WaitForSeconds(_hitStunTime);
        _disable = false;
    }

    void RotatePlayer()
    {
        Vector2 direction = Direction();
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y);
        }
    }

    public void Dash(InputAction.CallbackContext callback)
    {
        if (!_canDash)
            return;

        _canDash = false;
        _speed = _dashSpeed;
        StartCoroutine(DashDelay());
    }

    IEnumerator DashDelay()
    {
        yield return new WaitForSeconds(_dashTime);
        _speed = _defaultSpeed;
        yield return new WaitForSeconds(_dashDelay);
        _canDash = true;
    }

    void OnDestroy()
    {
        _dashKey.action.performed -= Dash;
    }
}
