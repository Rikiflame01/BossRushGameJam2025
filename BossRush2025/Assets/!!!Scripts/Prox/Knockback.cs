using System;
using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour
{
    [Header("KnockBack Properties")]
    [SerializeField] private float _knockBackStrength = 6f;
    [SerializeField] private float _knockBackTime = 1f;

    private Rigidbody2D _rb;
    private bool _haveKnockback = false;
    public event Action _onStartKnockback;
    public event Action _onFinishKnockback;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void PlayKnockBack(Vector2 knockBackPoint)
    {
        if (_haveKnockback)
            return;

        _onStartKnockback?.Invoke();
        _haveKnockback = true;

        Vector2 position = transform.position;
        Vector2 direction = (position - knockBackPoint).normalized;

        _rb.AddForce(direction * _knockBackStrength, ForceMode2D.Impulse);
        StartCoroutine(KnockBackDelay(_knockBackTime));
    }

    public void PlayKnockBack(Vector2 knockBackPoint, float strength, float time)
    {
        float deffaultStrentgh = _knockBackStrength;
        float deffaultTime = _knockBackTime;

        _knockBackStrength = strength;
        _knockBackTime = time;

        PlayKnockBack(knockBackPoint);

        _knockBackStrength = deffaultStrentgh;
        _knockBackTime = deffaultTime;
    }

    IEnumerator KnockBackDelay(float time)
    {
        yield return new WaitForSeconds(time);
        _onFinishKnockback?.Invoke();
        _haveKnockback = false;
    }
}
