using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private float _health = 10f;
    [SerializeField] private string _takeDamageSFX;
    [SerializeField] private string _dieParticles;

    [Header("Properties")]
    [SerializeField] private bool _immortal = false;

    private AudioManager _audioManager;
    private PoolManager _poolManager;

    public float _maxHealth { get; private set; }
    private bool _isAlive = true;
    private bool _receivceDamage = true;
    public event Action<float> _onHit;
    public event Action _onDie;

    void Start()
    {
        _maxHealth = _health;
        if (_takeDamageSFX != "")
        {
            _audioManager = FindAnyObjectByType<AudioManager>();
        }
        if (_dieParticles != "")
        {
            _poolManager = FindAnyObjectByType<PoolManager>();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!_isAlive || !_receivceDamage)
            return;

        if (!_immortal)
            _health = Math.Clamp(_health - damage, 0, _maxHealth);

        _onHit?.Invoke(damage);
        if(_audioManager!= null)
            _audioManager.PlaySFX(_takeDamageSFX);
        if (_health == 0)
        {
            if (_poolManager != null)
            {
                GameObject _currentParticles = _poolManager.GetObject(_dieParticles);
                _currentParticles.transform.position = transform.position;
            }
            if (_onDie != null)
                _onDie.Invoke();
            else
                Destroy(gameObject);
        }
    }
    public void AddHealth(float health)
    {
        if (!_isAlive)
            return;

        _health = Math.Clamp(_health + health, 0, _maxHealth);
    }

    public void SetHealth(float health)
    {
        if (!_isAlive)
            return;

        _health = Math.Clamp(health, 0, _maxHealth);
    }

    public float GetHealth()
    {
        return _health;
    }

    public void EnableReceivingDamage()
    {
        _receivceDamage = true;
    }

    public void DisableReceivingDamage()
    {
        _receivceDamage = false;
    }
}
