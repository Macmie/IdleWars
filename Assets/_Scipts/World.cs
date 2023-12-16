using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    [SerializeField] private NavMeshSurface _navMeshSurface;

    public NavMeshSurface NavSurface => _navMeshSurface;

    private void Awake()
    {
        if (Instance != null &&  Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
    }
    

}
