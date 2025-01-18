using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    [SerializeField] private PoolManager _poolManager;
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
                Vector2 direction = _target.position - transform.position;
                GameObject _currentArrow = _poolManager.GetObject(_arrow.name);
                _currentArrow.transform.position = transform.position;
                _currentArrow.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                /*Vector2 direction = _target.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                int helpInt = 1;
                for (int i = 0; i < 5; i++)
                {
                    angle += i * helpInt * 5;
                    GameObject currentProjectile = _poolManager.GetObject(_arrow.name);
                    currentProjectile.transform.position = transform.position;
                    currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, angle);
                    helpInt *= -1;
                }*/
            }
        }
        _bowSlider.gameObject.SetActive(false);
    }
    void OnDisable()
    {
        _isChargeNow = false;
        _bowSlider.gameObject.SetActive(false);
        _bowSlider.value = 0f;
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
            if (hit.collider.CompareTag("Enemy"))
            {
                Transform currentHit = hit.collider.transform;
                if (minDistance > Vector2.Distance(transform.position, currentHit.position))
                {
                    minDistance = Vector2.Distance(transform.position, currentHit.position);
                    _currentTarget = currentHit;
                }
            }
        }
        return _currentTarget;
    }
    /*
    private IEnumerator ArcShooting(int currentProjectileCount)
    {
        Vector2 direction = _target.position - transform.position;
        float startAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angleCof = 0f;
        if (currentProjectileCount % 2 == 0)
        {
            angleCof = 0.5f;
        }
        int board = currentProjectileCount / 2;
        for (int i = -board; i < board + currentProjectileCount % 2; i++)
        {
            GameObject currentProjectile = _poolManager.GetObject(_arrow.name);
            currentProjectile.transform.position = transform.position;
            float currentAngle = startAngle + (i + angleCof) * 10f;
            if (currentAngle >= 360f) currentAngle -= 360f;
            else if (currentAngle < 0f) currentAngle += 360f;
            currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, startAngle + (i + angleCof) * 10f);
            yield return new WaitForSeconds(0.3f);
        }
    }*/
}
