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

    [SerializeField] private InputActionReference _setFireKey;
    [SerializeField] private Image _fireProgress;
    [SerializeField] private float _fireChargeTime;
    [SerializeField] private string _burnParticles;
    
    private Coroutine _fireCoroutine;
    private float _ritualRadius;
    private Transform _ritualCenter;
    private float _maxFloatProportion;

    [Space]
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
    private PoolManager _poolManager;
    private Knockback _knockBack;
    private HealthManager _healthManager;

    void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _healthManager = GetComponent<HealthManager>();
        _knockBack = GetComponent<Knockback>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _poolManager = FindAnyObjectByType<PoolManager>();

        _knockBack._onStartKnockback += ()=>{ _disable = true; _healthManager.DisableReceivingDamage(); };
        _knockBack._onFinishKnockback += ()=>{ _disable = false; _healthManager.EnableReceivingDamage(); };

        _dashKey.action.performed += Dash;
        _ritualKey.action.started += StartRitual;
        _ritualKey.action.canceled += StopRitual;
        _setFireKey.action.started += StartSettingFire;
        _setFireKey.action.canceled += EndSettingFire;

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
                        _circleNumber++;
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
        _setFireKey.action.started -= StartSettingFire;
        _setFireKey.action.canceled -= EndSettingFire;
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

        GameObject _currentParticles = _poolManager.GetObject(_burnParticles);
        if (_currentParticles.TryGetComponent<RitualParticlesScript>(out RitualParticlesScript particleScript))
        {
            particleScript.SetCircleNumber(_circleNumber);
        }

        _gameManager.RitualEnd(_circleNumber);
        _circleNumber = 0;
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
            _currentState = State.Ritual;
            _currentAngle = Mathf.Atan2(transform.position.y, transform.position.x);
        }
    }
}
