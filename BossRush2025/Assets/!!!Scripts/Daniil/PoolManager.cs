using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public List<Pool> Pools = new List<Pool>();
    private Dictionary<string, Pool> _poolDictionary;

    public static PoolManager _instance;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        _poolDictionary = new Dictionary<string, Pool>();
        foreach (Pool currentPool in Pools)
        {
            currentPool.InitializePool();
            _poolDictionary.Add(currentPool.poolPrefab.name, currentPool);
        }
    }
    public GameObject GetObject(string poolName)
    {
        if (_poolDictionary.ContainsKey(poolName))
        {
            Pool currentPool = _poolDictionary[poolName];
            return currentPool.GetObjectFromPool();
        }
        else
        {
            Debug.LogWarning($"Pool with name '{poolName}' not found!");
            return null;
        }
    }

    public void ReturnObject(GameObject obj, string poolName)
    {
        if (_poolDictionary.ContainsKey(poolName))
        {
            Pool currentPool = _poolDictionary[poolName];
            currentPool.ReturnObjectInPool(obj);
        }
        else
        {
            Debug.LogWarning($"No pool found for '{poolName}'");
        }
    }

    [System.Serializable]
    public struct Pool
    {
        public GameObject poolPrefab;
        [SerializeField] private int initialSize;
        private Queue<GameObject> poolQueue;

        public void InitializePool()
        {
            poolQueue = new Queue<GameObject>();
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(poolPrefab);
                obj.SetActive(false);
                poolQueue.Enqueue(obj);
            }
        }

        public GameObject GetObjectFromPool()
        {
            if (poolQueue.Count > 0)
            {
                GameObject obj = poolQueue.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = Instantiate(poolPrefab);
                return obj;
            }
        }
        public void ReturnObjectInPool(GameObject obj)
        {
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }
}

