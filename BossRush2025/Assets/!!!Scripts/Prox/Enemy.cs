using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool _canMove = true;
    private bool _haveKnockback = false;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (!_canMove)
            return;
    }

    public void PlayKnockBack(float strength, float time, Vector2 knockBackPoint)
    {
        if (_haveKnockback)
            return;

        _canMove = false;
        _haveKnockback = true;

        Vector2 position = transform.position;
        Vector2 direction = (position - knockBackPoint).normalized;

        _rb.AddForce(direction * strength, ForceMode2D.Impulse);
        StartCoroutine(KnockBackDelay(time));
    }

    IEnumerator KnockBackDelay(float time)
    {
        yield return new WaitForSeconds(time);
        _canMove = true;
        _haveKnockback = false;
    }
}
