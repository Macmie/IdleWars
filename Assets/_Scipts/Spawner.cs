using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Unit _unitToSpawn;
    [SerializeField] private float _strenghtMulti;
    [SerializeField] private float _healthMulti;

    [SerializeField] private float _timer;
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _spawnedUnitLayerMask;

    private bool _checking;

    Vector3 _checkingPoint = new Vector3();

    IEnumerator Start()
    {
        while (true)
        {
            Vector3 spawnPoint = new Vector3();
            var canSpawn = TryGetSpawnPoint(out spawnPoint);

            if (!canSpawn)
            {
                _checking = true;
                Debug.Log("Point is invalid");
                while (!canSpawn)
                {
                    canSpawn = TryGetSpawnPoint(out spawnPoint);
                    yield return null;
                }
                Debug.Log("Found valid point");
                _checking = false;
            }

            var unit = Instantiate(_unitToSpawn, spawnPoint, Quaternion.identity);
            unit.InitializeUnit(_strenghtMulti, _healthMulti);

            Debug.Log($"Should Spawn unit, wait {_timer} seconds");
            yield return new WaitForSeconds(_timer);
        }
    }

    private bool TryGetSpawnPoint(out Vector3 spawnPoint)
    {
        spawnPoint = new Vector3();
        var point = Random.insideUnitCircle.normalized * _radius;
        spawnPoint = new Vector3(point.x, 0, point.y);
        spawnPoint = transform.position + spawnPoint;

        if (_checking )
        {
            _checkingPoint = spawnPoint;
        }

        Debug.Log($"Checking point: {spawnPoint}");

        if (CanSpawnAtPoint(spawnPoint))
            return true;
        else
            return false;
    }

    private bool CanSpawnAtPoint(Vector3 point)
    {
        var colliders = Physics.OverlapSphere(point, .3f, _spawnedUnitLayerMask);

        if (colliders.Length == 0)
        {
            Debug.Log("No colliders on point: " + point);
            return true;
        }

        foreach (Collider collider in colliders)
        {
            var dist = Vector3.Distance(point, collider.transform.position);
            Debug.Log($"Found: {collider.gameObject.name} in distance: {dist}");

            if ( dist < 1.2f)
            {
                return false;
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        if (_checking)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_checkingPoint, .3f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
