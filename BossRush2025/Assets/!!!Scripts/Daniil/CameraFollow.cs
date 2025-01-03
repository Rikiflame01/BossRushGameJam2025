using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private Vector3 _offset;
    [SerializeField] private int _speed;
    void Start()
    {
        _offset = transform.position - _player.position;
    }
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _player.position + _offset, _speed * Time.fixedDeltaTime);
    }
}
