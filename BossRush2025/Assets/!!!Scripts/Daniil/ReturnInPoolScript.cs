using UnityEngine;
using System.Collections;

public class ReturnInPoolScript : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private float _delay = 4f;
    private ParticleSystem _particleSystem;
    private PoolManager _poolManager;
    void Awake()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();
        _particleSystem = GetComponent<ParticleSystem>();
    }
    void Start()
    {
        StartCoroutine(ReturnInPool());
    }
    void OnEnable()
    {
        StartCoroutine(ReturnInPool());
    }
    private IEnumerator ReturnInPool()
    {
        _particleSystem.Play();
        yield return new WaitForSeconds(_delay);
        _poolManager.ReturnObject(gameObject, _name);
    }
}
