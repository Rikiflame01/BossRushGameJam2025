using UnityEngine;
using DG.Tweening;
using System.Collections;
public class CameraFollow : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private float _maxX, _maxY;
    [SerializeField] private float _maxPlayerDistance;

    [SerializeField] private Transform _player;
    [SerializeField] private int _speed;
    private Vector3 _offset;

    [SerializeField] private float _specialMovementDuration = 0.5f;
    private bool _canMove = true;

    void Start()
    {
        _offset = transform.position - _player.position;
        _gameManager = FindAnyObjectByType<GameManager>();
        _gameManager.RitualFinished += ToCenter;
    }
    void FixedUpdate()
    {
        if (!_canMove) return;

        Vector3 vectorToMove = transform.position - _offset;
        Vector3 currentOffset = _player.position - transform.position;

        if (currentOffset.x > _maxPlayerDistance)
            vectorToMove.x += currentOffset.x - _maxPlayerDistance;
        else if(currentOffset.x < -_maxPlayerDistance)
            vectorToMove.x += currentOffset.x + _maxPlayerDistance;

        if (currentOffset.y > _maxPlayerDistance)
            vectorToMove.y += currentOffset.y - _maxPlayerDistance;
        else if(currentOffset.y < -_maxPlayerDistance)
            vectorToMove.y += currentOffset.y + _maxPlayerDistance;

        if (vectorToMove.x < -_maxX) vectorToMove.x = -_maxX;
        else if(vectorToMove.x > _maxX) vectorToMove.x = _maxX;

        if(vectorToMove.y < -_maxY) vectorToMove.y = -_maxY;
        else if(vectorToMove.y > _maxY) vectorToMove.y = _maxY;

        transform.position = Vector3.Lerp(transform.position, vectorToMove + _offset, _speed * Time.fixedDeltaTime);
    }
    void ToCenter()
    {
        transform.DOMove(_gameManager.RitualCenter.position + _offset, _specialMovementDuration);
        StartCoroutine(DisableForTime(1.5f));
    }
    private IEnumerator DisableForTime(float time)
    {
        _canMove = false;
        yield return new WaitForSeconds(time);
        _canMove = true;
    }
}
