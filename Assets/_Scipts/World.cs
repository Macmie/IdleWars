using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private int _maxSummonedUnits;

    public NavMeshSurface NavSurface => _navMeshSurface;

    private List<PlayerUnit> _playerUnits = new List<PlayerUnit>();

    public bool CanSpawnFurther {  get; private set; }

    private void Awake()
    {
        if (Instance != null &&  Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
        CanSpawnFurther = true;
    }
    
    public void AddPlayerUnitToList(PlayerUnit playerUnit)
    {
        _playerUnits.Add(playerUnit);
        if (_playerUnits.Count >= _maxSummonedUnits)
        {
            CanSpawnFurther = false;
        }
    }

    public void RemovePlayerUnitFromList(PlayerUnit playerUnit)
    {
        _playerUnits.Remove(playerUnit);
        CanSpawnFurther = true;
    }
}
