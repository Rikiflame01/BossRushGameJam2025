using UnityEngine;
using System.Collections;

public class AxisPriojectile : MonoBehaviour
{
    private GameManager _gameManager;
    private PoolManager _poolManager;
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
    void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _poolManager = FindAnyObjectByType<PoolManager>();
        StartNewCycle();
    }
    void OnEnable()
    {
        StartNewCycle();
    }
    void Update()
    {
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
        Disappear();
    }
    private void Disappear()
    {
        _poolManager.ReturnObject(gameObject, "AxisProjectile");
    }
    void StartNewCycle()
    {
        _center = _gameManager.RitualCenter.position;
        _radius = _gameManager.RitualCircleRadius;
        if (Random.Range(0, 2) == 0) _angleSpeed = -_angleSpeed;
        Vector2 currentPos = (Vector2)transform.position - _center;
        _currentAngle = Random.Range(0f, Mathf.PI * 2f); ;
        _currentSpeed = 0f;
        _currentRadius = _radius;
        StartCoroutine(LifeCoroutine());
    }
}
