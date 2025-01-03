using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private bool _canMove = true;
    private bool _haveKnockback = false;
    private Rigidbody2D _rb;
    private NavMeshAgent _navMeshAgent;
    private Transform _target;
    private float _speed;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _speed = _navMeshAgent.speed;

        _target = GameObject.FindAnyObjectByType<Movement>().transform;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (!_canMove)
            return;

        _navMeshAgent.SetDestination(_target.position);
    }

    public void PlayKnockBack(float strength, float time, Vector2 knockBackPoint)
    {
        if (_haveKnockback)
            return;

        DisableMovement();
        _haveKnockback = true;

        Vector2 position = transform.position;
        Vector2 direction = (position - knockBackPoint).normalized;

        _rb.AddForce(direction * strength, ForceMode2D.Impulse);
        StartCoroutine(KnockBackDelay(time));
    }

    IEnumerator KnockBackDelay(float time)
    {
        yield return new WaitForSeconds(time);
        EnableMovement();
        _haveKnockback = false;
    }

    public void DisableMovement()
    {
        _canMove = false;
        _navMeshAgent.speed = 0;
    }

    public void EnableMovement()
    {
        _canMove = true;
        _navMeshAgent.speed = _speed;
    }
}
