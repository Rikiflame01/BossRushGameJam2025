using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private InputActionReference _ritualKey;
    [SerializeField] private Image _ritualProgress;
    [SerializeField] private float _ritualSpeed = 5.0f;
    private float _ritualRadius;
    private Transform _ritualCenter;

    private float _currentAngle;
    private float _startRitualAngle;
    private float _targetRitualAngle;
    private bool _isRitual = false;
    private bool _clockwise = false;
    private int _circleNumber = 0;
    private RectTransform _progressTransform;

    private GameManager _gameManager;
    private Knockback _knockBack;
    private HealthManager _healthManager;

    void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _healthManager = GetComponent<HealthManager>();
        _knockBack = GetComponent<Knockback>();
        _gameManager = FindAnyObjectByType<GameManager>();

        _knockBack._onStartKnockback += ()=>{ _disable = true; _healthManager.DisableReceivingDamage(); };
        _knockBack._onFinishKnockback += ()=>{ _disable = false; _healthManager.EnableReceivingDamage(); };

        _dashKey.action.performed += Dash;
        _ritualKey.action.started += StartRitual;
        _ritualKey.action.canceled += StopRitual;

        _defaultSpeed = _speed;

        _ritualRadius = _gameManager.RitualCircleRadius;
        _ritualCenter = _gameManager.RitualCenter;
        _progressTransform = _ritualProgress.GetComponent<RectTransform>();
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
        }
    }
    void Update()
    {
        switch (_currentState)
        {
            case State.Ritual:
                _currentAngle += _ritualSpeed * Time.deltaTime * Direction().x;
                transform.position = (Vector2)_ritualCenter.position + new Vector2(Mathf.Cos(_currentAngle), Mathf.Sin(_currentAngle)) * _ritualRadius;
                if (!_isRitual)
                {
                    if (_currentAngle > 2 * Mathf.PI)
                    {
                        _currentAngle -= 2 * Mathf.PI;
                    }
                    else if (_currentAngle < 0)
                    {
                        _currentAngle += 2 * Mathf.PI;
                    }
                }
                else
                {
                    if (_currentAngle > _startRitualAngle && _clockwise)
                    {
                        ChangeRitualDirection(false);
                    }
                    else if (_currentAngle < _startRitualAngle && !_clockwise)
                    {
                        ChangeRitualDirection(true);
                    }

                    float targetFloat = Mathf.Abs(_targetRitualAngle - _startRitualAngle);
                    float currentFloat = Mathf.Abs(_currentAngle - _startRitualAngle);
                    if (currentFloat > targetFloat)
                    {
                        _startRitualAngle = _targetRitualAngle;
                        _targetRitualAngle = _startRitualAngle + (_clockwise ? -2 : 2) * Mathf.PI;
                        _circleNumber++;
                    }
                    _ritualProgress.fillAmount = currentFloat / targetFloat;
                }
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
        _ritualKey.action.started -= StartRitual;
        _ritualKey.action.canceled -= StopRitual;
    }
    private void StartRitual(InputAction.CallbackContext callback)
    {
        _isRitual = true;
        _startRitualAngle = _currentAngle;
        ChangeRitualDirection(false);
        _progressTransform.eulerAngles = new Vector3(_progressTransform.eulerAngles.x, _progressTransform.eulerAngles.y, _startRitualAngle * Mathf.Rad2Deg);
        _ritualProgress.enabled = true;
    }
    private void StopRitual(InputAction.CallbackContext callback)
    {
        _isRitual = false;
        _ritualProgress.enabled = false;
        _gameManager.RitualEnd(_circleNumber);
        _circleNumber = 0;
    }
    private void ChangeRitualDirection(bool Clockwise)
    {
        _targetRitualAngle = _startRitualAngle + (Clockwise ? -2 : 2) * Mathf.PI;
        _clockwise = Clockwise;
        _ritualProgress.fillClockwise = Clockwise;
    }
}
