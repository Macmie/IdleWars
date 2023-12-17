using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitPooler;


public enum UnitType
{
    PlayerUnit,
    EnemyUnit
}


public class UnitPooler : MonoBehaviour
{
    [System.Serializable]
    internal class Pool
    {
        public UnitType UnitType;
        public Unit UnitPrefab;
        public int PoolSize;
    }

    public static UnitPooler Instance;

    [SerializeField] private List<Pool> _poolList = new List<Pool>();

    private Dictionary<UnitType, Queue<Unit>> _poolDictionary = new Dictionary<UnitType, Queue<Unit>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
    }

    void Start()
    {
        foreach (Pool pool in _poolList)
        {
            Queue<Unit> _units = new Queue<Unit>();

            for (int i = 0; i < pool.PoolSize; i++)
            {
                Unit unit = Instantiate(pool.UnitPrefab);
                unit.gameObject.SetActive(false);
                _units.Enqueue(unit);
            }

            _poolDictionary.Add(pool.UnitType, _units);
        }
    }

    public Unit SpawnFromPool(UnitType type, Vector3 position)
    {
        if (_poolDictionary[type].Count == 1)
            AddUnitToPool(type);

        var unit = _poolDictionary[type].Dequeue();
        unit.gameObject.SetActive(true);
        unit.transform.position = position;
        return unit;
    }

    public void PutUnitBackToPool(UnitType type, Unit unit)
    {
        unit.gameObject.SetActive(false);
        _poolDictionary[type].Enqueue(unit);
    }

    private void AddUnitToPool(UnitType type)
    {
        Pool pool = _poolList.Find(x => x.UnitType == type);
        Unit unit = Instantiate(pool.UnitPrefab);
        unit.gameObject.SetActive(false);
        _poolDictionary[type].Enqueue(unit);
    }
}
