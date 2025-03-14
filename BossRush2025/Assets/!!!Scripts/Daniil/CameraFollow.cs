using UnityEngine;
using DG.Tweening;
using System.Collections;
public class CameraFollow : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private float _maxX, _maxY;
    [SerializeField] private float _maxPlayerDistance;

    private Transform _player;
    [SerializeField] private int _speed;
    private Vector3 _offset;

    [SerializeField] private float _specialMovementDuration = 0.5f;
    private bool _canMove = true;

    public static CameraFollow _instance;
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _offset = new Vector3(0f, 0f, -10f);
        _player = FindAnyObjectByType<Movement>().transform;
        _gameManager = FindAnyObjectByType<GameManager>();
        StartCoroutine(StartAnim());
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
    public void ToCenter(float duration = 1.5f)
    {
        transform.DOMove(_gameManager.RitualCenter.position + _offset, _specialMovementDuration);
        StartCoroutine(DisableForTime(duration));
    }
    private IEnumerator StartAnim()
    {
        StartCoroutine(DisableForTime(4.5f));
        yield return new WaitForSeconds(1f);
        transform.DOMove(new Vector3(0, -_maxY, -10), 3.5f);
    }
    private IEnumerator DisableForTime(float time)
    {
        _canMove = false;
        yield return new WaitForSeconds(time);
        _canMove = true;
    }
}
