using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] float _speed;
    private PoolManager _poolManager;
    void Start()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();
    }
    void Update()
    {
        transform.Translate(Vector2.right * _speed * Time.deltaTime);
        //_poolManager.ReturnObject(gameObject, "Arrow");
    }
}
