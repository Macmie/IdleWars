using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private UnitType _unitType;
    [SerializeField] private TextMeshProUGUI _timerUI;

    [SerializeField] private bool _isEnemySpawner;
    [SerializeField] private Unit _unitToSpawn;
    [SerializeField] private float _strenghtMulti;
    [SerializeField] private float _healthMulti;

    [SerializeField] private float _timer;
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _spawnedUnitLayerMask;

    private bool _checking;
    private UnitPooler _pooler;

    Vector3 _checkingPoint = new Vector3();


    IEnumerator Start()
    {
        _pooler = UnitPooler.Instance;
        while (true)
        {
            if (World.Instance.CanSpawn(_isEnemySpawner))
            {
                var time = _timer;
                while (time > 0)
                {
                    time -= Time.deltaTime;
                    SetTimer(time);
                    yield return null;
                }

                Vector3 spawnPoint = new Vector3();
                var canSpawn = TryGetSpawnPoint(out spawnPoint);

                if (!canSpawn)
                {
                    _checking = true;
                    while (!canSpawn)
                    {
                        canSpawn = TryGetSpawnPoint(out spawnPoint);
                        yield return null;
                    }
                    _checking = false;
                }

                var unit = _pooler.SpawnFromPool(_unitType, spawnPoint);
                unit.InitializeUnit(_strenghtMulti, _healthMulti, _pooler, _unitType);
                unit.OnSpawnedObject();
            }
            else
            {
                SetTimer(_timer);
                yield return null;
            }
        }
    }

    private bool TryGetSpawnPoint(out Vector3 spawnPoint)
    {
        spawnPoint = new Vector3();
        var point = UnityEngine.Random.insideUnitCircle.normalized * _radius;
        spawnPoint = new Vector3(point.x, 0, point.y);
        spawnPoint = transform.position + spawnPoint;

        if (_checking )
        {
            _checkingPoint = spawnPoint;
        }

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
            return true;
        }

        foreach (Collider collider in colliders)
        {
            var dist = Vector3.Distance(point, collider.transform.position);

            if ( dist < 1.2f)
            {
                return false;
            }
        }
        return true;
    }

    private void SetTimer(float time)
    {
        _timerUI.text = $"Time to next unit spawn: {time:F}";
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

    public void IncreaseStrModifier() => _strenghtMulti *= 1.1f;

    public void IncreaseHealthMulti() => _healthMulti *= 1.1f;

}
