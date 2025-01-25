using UnityEngine;
using System.Collections;

public class ReturnInPoolScript : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private float _delay = 4f;
    private PoolManager _poolManager;
    void Start()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();
        StartCoroutine(ReturnInPool());
    }
    private IEnumerator ReturnInPool()
    {
        yield return new WaitForSeconds(_delay);
        _poolManager.ReturnObject(gameObject, _name);
    }
}
