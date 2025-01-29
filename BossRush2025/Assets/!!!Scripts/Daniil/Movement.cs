using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private enum State { Run, Ritual , Disable};
    private State _currentState = State.Run;    

    private Rigidbody2D _playerRb;
    [SerializeField] private float _speed;
    private bool _disable;
    private Vector2 _lastDirection;
    [SerializeField] private InputActionReference _moveInput;
    [SerializeField] private float _hitStunTime;
    private float _originalScale;

    [SerializeField] private List<Component> _componentsToDisable;

    [Header("Steps Properties")]
    [SerializeField] float _stepDelay;
    private float _stepTime;
    private bool _playStep = true;
    private int _currentStep = 1;

    [Header("Dash Properties")]
    [SerializeField] private InputActionReference _dashKey;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime = 0.15f;
    [SerializeField] private float _dashDelay = 1f; 
    private bool _canDash = true;
    private bool _isDash  = false;
    private float _defaultSpeed;

    [Header("Ritual Properties")]
    [SerializeField] private InputActionReference _ritualKey;
    [SerializeField] private Image _ritualProgress;
    [SerializeField] private float _ritualSpeed = 5.0f;

    [SerializeField] private InputActionReference _setFireKey;
    [SerializeField] private Image _fireProgress;
    [SerializeField] private float _fireChargeTime;
    
    private Coroutine _fireCoroutine;
    private float _ritualRadius;
    private Transform _ritualCenter;
    private float _maxFloatProportion;

    [Space]
    [SerializeField] private TextMeshProUGUI _circleTxt;

    [SerializeField] private string _sakuraLeaves;
    [SerializeField] private float _minLeafSpawnDelay;
    private List<GameObject> _currentLeavesList = new List<GameObject>();
    private float _currentTime;

    private float _currentAngle;
    private float _startRitualAngle;
    private float _targetRitualAngle;

    private bool _isRitual = false;
    private bool _clockwise = false;
    private int _circleNumber = 0;
    private RectTransform _progressTransform;

    private GameManager _gameManager;
    private AudioManager _audioManager;
    private PoolManager _poolManager;
    private Knockback _knockBack;
    private HealthManager _healthManager;

    void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _healthManager = GetComponent<HealthManager>();
        _knockBack = GetComponent<Knockback>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _audioManager = FindAnyObjectByType<AudioManager>();
        _poolManager = FindAnyObjectByType<PoolManager>();

        _knockBack._onStartKnockback += ()=>{ _disable = true; _healthManager.DisableReceivingDamage(); };
        _knockBack._onFinishKnockback += ()=>{ _disable = false; _healthManager.EnableReceivingDamage(); };

        _dashKey.action.performed += Dash;
        _ritualKey.action.started += StartRitual;
        _ritualKey.action.canceled += StopRitual;
        _setFireKey.action.started += StartSettingFire;
        _setFireKey.action.canceled += EndSettingFire;

        _gameManager.PlayerDie += Die;

        _defaultSpeed = _speed;

        _ritualRadius = _gameManager.RitualCircleRadius;
        _ritualCenter = _gameManager.RitualCenter;
        _progressTransform = _ritualProgress.GetComponent<RectTransform>();
        _originalScale = transform.localScale.x;

    }

    private Vector2 Direction() => _moveInput.action.ReadValue<Vector2>();
    void FixedUpdate()
    {
        switch (_currentState)
        {
            case State.Run:
                if (!_disable)
                {
                    if (!_isDash) 
                    {
                        _lastDirection = Direction();
                        RotatePlayer();
                    }
                    _playerRb.linearVelocity = _lastDirection * _speed;
                    if (!_isDash)
                    {
                        _playStep = _playerRb.linearVelocity.x != 0f || _playerRb.linearVelocity.y != 0f;
                    }
                }
                if (_playStep)
                {
                    _stepTime += Time.deltaTime;
                    if(_stepTime >= _stepDelay)
                    {
                        _stepTime = 0;
                        AudioManager._instance.PlaySFX("Step " + _currentStep);
                        _currentStep++;
                        if (_currentStep > 4) _currentStep = 1;
                    }
                }
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
                if (_isRitual)
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
                    if (_maxFloatProportion < currentFloat / targetFloat)
                    {
                        _maxFloatProportion = currentFloat / targetFloat;
                        _currentTime += Time.deltaTime;
                        if(_currentTime >= _minLeafSpawnDelay)
                        {
                            _currentTime = 0f;
                            GameObject currentLeaf = _poolManager.GetObject(_sakuraLeaves);
                            currentLeaf.transform.position = (Vector2)_ritualCenter.position + new Vector2(Mathf.Cos(_currentAngle), Mathf.Sin(_currentAngle)) * (_ritualRadius + Random.Range(-0.3f, 0.3f));
                            currentLeaf.transform.Rotate(Vector3.forward * Random.Range(0, 360));
                            _currentLeavesList.Add(currentLeaf);
                        }
                    }
                    if (currentFloat > targetFloat)
                    {
                        _startRitualAngle = _targetRitualAngle;
                        _targetRitualAngle = _startRitualAngle + (_clockwise ? -2 : 2) * Mathf.PI;
                        if(_circleNumber < 3) _circleNumber++;
                        _circleTxt.text = _circleNumber.ToString();
                        ClearLeafs();
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
            transform.localScale = new Vector3(_originalScale, transform.localScale.y);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-_originalScale, transform.localScale.y);
        }
    }
    #region Dash
    public void Dash(InputAction.CallbackContext callback)
    {
        if (!_canDash || (Direction().x == 0f && Direction().y == 0))
            return;
        _canDash = false;
        _isDash = true;
        _speed = _dashSpeed;
        _audioManager.PlaySFX("Dash");
        StartCoroutine(DashDelay());
    }

    IEnumerator DashDelay()
    {
        _playStep = false;
        yield return new WaitForSeconds(_dashTime);
        _speed = _defaultSpeed;
        _playStep = true;
        _isDash = false;
        yield return new WaitForSeconds(_dashDelay);
        _canDash = true;
    }
    #endregion
    void OnDestroy()
    {
        _dashKey.action.performed -= Dash;
        _ritualKey.action.started -= StartRitual;
        _ritualKey.action.canceled -= StopRitual;
        _setFireKey.action.started -= StartSettingFire;
        _setFireKey.action.canceled -= EndSettingFire;
    }

    #region Rutual
    private void StartRitual(InputAction.CallbackContext callback)
    {
        if (_currentState == State.Ritual)
        {
            _isRitual = true;
            _startRitualAngle = _currentAngle;
            ChangeRitualDirection(false);
            _progressTransform.eulerAngles = new Vector3(_progressTransform.eulerAngles.x, _progressTransform.eulerAngles.y, _startRitualAngle * Mathf.Rad2Deg);
            _ritualProgress.enabled = true;
        }
    }
    private void StopRitual(InputAction.CallbackContext callback)
    {
        ClearLeafs();
        _isRitual = false;
        _ritualProgress.enabled = false;
    }
    private void StartSettingFire(InputAction.CallbackContext callback)
    {
        if (_circleNumber > 0)
        {
            _fireCoroutine = StartCoroutine(FireRitual());
        }
    }
    private void EndSettingFire(InputAction.CallbackContext callback)
    {
        if (_fireCoroutine == null) return;
        StopCoroutine(_fireCoroutine);
        _fireCoroutine = null;
        _fireProgress.enabled = false;
    }
    private IEnumerator FireRitual()
    {
        _fireProgress.enabled = true;
        float startTime = Time.time;
        while(Time.time - startTime < _fireChargeTime)
        {
            _fireProgress.fillAmount = (Time.time - startTime) / _fireChargeTime;
            yield return null;
        }
        ClearLeafs();
        _isRitual = false;
        _ritualProgress.enabled = false;

        _gameManager.RitualEnd(_circleNumber);
        EnableComponents();
        _circleNumber = 0;
        _circleTxt.enabled = false;
        _fireProgress.enabled = false;
        _currentTime = 0;
        _currentState = State.Run;
    }
    private void ChangeRitualDirection(bool Clockwise)
    {
        _targetRitualAngle = _startRitualAngle + (Clockwise ? -2 : 2) * Mathf.PI;
        _clockwise = Clockwise;
        _ritualProgress.fillClockwise = Clockwise;
        ClearLeafs();
    }
    private void ClearLeafs()
    {
        foreach(GameObject leaf in _currentLeavesList)
        {
            _poolManager.ReturnObject(leaf, _sakuraLeaves);
        }
        _currentLeavesList.Clear();
        _maxFloatProportion = 0;
        _currentTime = 0;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ritual"))
        {
            other.enabled = false;
            _currentAngle = Mathf.Atan2(transform.position.y, transform.position.x);
            DisableComponents();
            if(Vector2.Distance(transform.position, _gameManager.RitualCenter.position) < _gameManager.RitualCircleRadius - 1f)
            {
                StartCoroutine(MoveToCircle());
            }
            else
            {
                _currentState = State.Ritual;
            }
            _circleTxt.enabled = true;
            _circleTxt.text = _circleNumber.ToString();
        }
    }
    private IEnumerator MoveToCircle()
    {
        _currentState = State.Disable;
        transform.DOMove((Vector2)_ritualCenter.position + new Vector2(Mathf.Cos(_currentAngle), Mathf.Sin(_currentAngle)) * _ritualRadius, 0.25f);
        yield return new WaitForSeconds(0.25f);
        _currentState = State.Ritual;
    }
    #endregion

    private void EnableComponents()
    {
        foreach (Behaviour component in _componentsToDisable)
        {
            component.enabled = true;
        }
    }
    private void DisableComponents()
    {
        foreach (Behaviour component in _componentsToDisable)
        {
            component.enabled = false;
        }
    }
    private void Die()
    {
        DisableComponents();
        _dashKey.action.performed -= Dash;
        _ritualKey.action.started -= StartRitual;
        _ritualKey.action.canceled -= StopRitual;
        _setFireKey.action.started -= StartSettingFire;
        _setFireKey.action.canceled -= EndSettingFire;
        this.enabled = false;
    }
}
