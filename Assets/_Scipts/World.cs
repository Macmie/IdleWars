using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    [SerializeField] private TextMeshProUGUI _canPlayerSpawnUI;
    [SerializeField] private TextMeshProUGUI _canEnemySpawnUI;

    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private int _maxSummonedUnits;
    [SerializeField] private int _maxEnemyUnits;

    public NavMeshSurface NavSurface => _navMeshSurface;

    private int _playerUnits, _enemyUnits;
    public bool CanSpawnFurther {  get; private set; }
    public bool CanEnemySpawnFurther {  get; private set; }


    private void Awake()
    {
        if (Instance != null &&  Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
        SetIfCanSpawn(true, true);
        SetIfCanSpawn(false, true);
    }
    
    public void AddPlayerUnitToList()
    {
        _playerUnits++;
        if (_playerUnits >= _maxSummonedUnits)
        {
            SetIfCanSpawn(true, false);
        }
    }

    public void RemovePlayerUnitFromList()
    {
        _playerUnits--;
        SetIfCanSpawn(true, true);
    }

    public void AddEnemyToList()
    {
        _enemyUnits++;
        if (_enemyUnits >= _maxEnemyUnits)
        {
            SetIfCanSpawn(false, false);
        }

    }

    public bool CanSpawn(bool isEnemySpawner)
    {
        if (isEnemySpawner)
            return CanEnemySpawnFurther;
        else
            return CanSpawnFurther;
    }

    public void RemoveEnemyFromList()
    {
        _enemyUnits--;
        SetIfCanSpawn(false, true);
    }

    private void SetIfCanSpawn(bool player, bool canSpawn)
    {
        if (player)
        {
            CanSpawnFurther = canSpawn;
            _canPlayerSpawnUI.text = "Can Player Spawn: " + canSpawn;
        }
        else
        {
            CanEnemySpawnFurther = canSpawn;
            _canEnemySpawnUI.text = "Can Enemy Spawn: " + canSpawn;
        }
    }
}
