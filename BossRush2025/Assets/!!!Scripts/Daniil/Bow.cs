using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    [SerializeField] private Slider _bowSlider;
    [SerializeField] private GameObject _arrow;
    [SerializeField] Vector3 _offset;

    [SerializeField] private float _chargeTime;
    private float _startTime;
    private bool _isChargeNow;

    [SerializeField] private float _targetMaxDistance;
    public Transform _target;

    [SerializeField] private InputActionReference _bowKey;
    void Start()
    {
        _bowKey.action.started += StartCharge;
        _bowKey.action.canceled += ReleaseArrow;
        _bowSlider.maxValue = _chargeTime;
    }
    void Update()
    {
        if (_isChargeNow)
        {
            _bowSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + _offset);
            float currentTime = Time.time - _startTime;
            _bowSlider.value = Mathf.Clamp(currentTime, 0f, _chargeTime);
            if (currentTime > _chargeTime)
            {
                //indicate full charge
            }
        }
    }
    void StartCharge(InputAction.CallbackContext context)
    {
        _isChargeNow = true;
        _startTime = Time.time;
        _bowSlider.gameObject.SetActive(true);
        _bowSlider.value = 0f;
    }
    void ReleaseArrow(InputAction.CallbackContext context)
    {
        _isChargeNow = false;
        if (Time.time - _startTime > _chargeTime)
        {
            _target = SetTarget();
            if (_target != null)
            {
                float x = transform.position.x - _target.position.x;
                float y = transform.position.y - _target.position.y;
                // will fix this
                Instantiate(_arrow, transform.position, Quaternion.identity).transform.eulerAngles = new Vector3(0f, 0f, -90 + (x >= 0 ? -1 : 1) * Mathf.Atan2(Mathf.Abs(x), y) * Mathf.Rad2Deg);
            }
        }
        _bowSlider.gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        _bowKey.action.started -= StartCharge;
        _bowKey.action.canceled -= ReleaseArrow;
    }
    private Transform SetTarget()
    {
        float minDistance = _targetMaxDistance + 1f;
        Transform _currentTarget = null;
        List<RaycastHit2D> collidingObjects = Physics2D.CircleCastAll(transform.position, _targetMaxDistance, Vector2.zero).ToList();
        foreach (RaycastHit2D hit in collidingObjects)
        {
            Transform currentHit = hit.collider.transform;
            if (currentHit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (minDistance > Vector2.Distance(transform.position, currentHit.position))
                {
                    minDistance = Vector2.Distance(transform.position, currentHit.position);
                    _currentTarget = currentHit;
                }
            }
        }
        return _currentTarget;
    }
}
