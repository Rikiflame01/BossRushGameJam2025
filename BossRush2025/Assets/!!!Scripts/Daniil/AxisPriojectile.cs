using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AxisPriojectile : Projectile
{
    private GameManager _gameManager;
    private Collider2D _collider;
    private Animator _animator;

    private int _appearAnim = Animator.StringToHash("appear");

    [SerializeField] private float _maxRadiusSpeed;
    [SerializeField] private float _angleSpeed = 0.25f;
    [SerializeField] private float _maxRadius;
    [SerializeField] private float _acceleration;

    [SerializeField] private float _lifeTime;

    private Vector2 _center;
    private float _radius;
    private float _currentSpeed;
    private float _currentAngle;
    private float _currentRadius;

    private bool _isFar = true;
    private bool _canMove = false;

    void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }
    protected override void Initialize() { }
    void OnEnable()
    {
        StartCoroutine(StartNewCycle());
    }
    void Update()
    {
        if (!_canMove) return;
        _currentAngle += _angleSpeed * Time.deltaTime;
        _currentSpeed += _acceleration * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, -_maxRadiusSpeed, _maxRadiusSpeed);
        if (_currentRadius > _maxRadius + _radius && _isFar || _currentRadius < _radius - _maxRadius && !_isFar)
        {
            _acceleration *= -1;
            _isFar = !_isFar;
        }
        _currentRadius += _currentSpeed * Time.deltaTime;
        transform.position = _center + new Vector2(Mathf.Cos(_currentAngle), Mathf.Sin(_currentAngle)) * _currentRadius;
    }
    private IEnumerator LifeCoroutine()
    {
        yield return new WaitForSeconds(_lifeTime);

        _canMove = false;
        _collider.enabled = false;

        _animator.SetBool(_appearAnim, false);
        yield return new WaitForSeconds(0.3f);

        ReturnInPool();
    }
    private IEnumerator StartNewCycle()
    {
        _collider.enabled = false;

        _center = _gameManager.RitualCenter.position;
        _radius = _gameManager.RitualCircleRadius;
        if (Random.Range(0, 2) == 0) _angleSpeed = -_angleSpeed;
        _currentSpeed = 0f;

        _currentAngle = Random.Range(0f, Mathf.PI * 2f);
        _currentRadius = _radius;
        transform.position = _center + new Vector2(Mathf.Cos(_currentAngle), Mathf.Sin(_currentAngle)) * _currentRadius;

        _animator.SetBool(_appearAnim, true);
        yield return new WaitForSeconds(1.3f);

        _canMove = true;
        _collider.enabled = true;

        StartCoroutine(LifeCoroutine());
    }
}
