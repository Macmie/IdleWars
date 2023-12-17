using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Unit : MonoBehaviour, IPooledObject
{
    [SerializeField] protected float _checkRadius;
    [SerializeField] protected float _attackDistance;
    [SerializeField] private SphereCollider _checkCollider;
    [SerializeField] private LayerMask _enemyLayer;

    [Header("Basic skills")]
    [SerializeField] protected float _strenght;
    [SerializeField] protected float _maxHealth;

    private UnitPooler _pooler;
    private UnitType _unitType;

    protected List<Unit> _spottedEnemies = new List<Unit>();
    protected Unit _targetUnit;
    protected bool _isChasing, _isScouting, _isFighting;
    protected NavMeshAgent _agent;
    protected float _health;
    protected bool _initialized;

    protected Coroutine _actionCoroutine;

    public float Health => _health;

    public void InitializeUnit(float strMulti, float healthMulti, UnitPooler pooler, UnitType type)
    {
        _agent = GetComponent<NavMeshAgent>();
        _strenght *= strMulti;
        _maxHealth *= healthMulti;
        _checkCollider.radius = _checkRadius;
        _health = _maxHealth;
        _pooler = pooler;
        _unitType = type;
        _initialized = true;
    }

    public virtual void OnSpawnedObject()
    {
        Scout();
    }

    public virtual void GetDamage(float dmg)
    {
        _health -= dmg;
    }

    protected virtual void Die()
    {
        StopAllCoroutines();
        CurrencyManager.Instance.UnitDeath();
        _pooler.PutUnitBackToPool(_unitType, this);
    }

    protected IEnumerator Wander()
    {
        _isScouting = true;
        var point = GetRandomPointFromSurface();
        point.y = transform.position.y;
        _agent.SetDestination(point);
        
        while (_targetUnit == null)
        {
            var dist = Vector3.Distance(transform.position, point);
            while (dist > 2f && _targetUnit == null)
            {
                dist = Vector3.Distance(transform.position, point);
                CheckForEnemyToChase();
                yield return null;
            }   
            point = GetRandomPointFromSurface();
            point.y = transform.position.y;
            _agent.SetDestination(point);           
        }
        _isScouting = false;
        Chase(_targetUnit);
    }

    private Vector3 GetRandomPointFromSurface()
    {
        var surface = World.Instance.NavSurface;
        var bounds = surface.navMeshData.sourceBounds;

        float x = UnityEngine.Random.Range(bounds.min.x + 3, bounds.max.x - 3);
        float z = UnityEngine.Random.Range(bounds.min.z + 3, bounds.max.z - 3);

        return new Vector3(x, transform.position.y, z);

    }

    private void CheckForEnemyToChase()
    {
        var enemies = Physics.OverlapSphere(transform.position, _checkRadius, _enemyLayer);
        _spottedEnemies = new List<Unit>();
        if (enemies.Length == 0)
        {
            _targetUnit = null;
            return;
        }

        foreach (var enemy in enemies)
        {
            Unit enemyUnit = enemy.GetComponent<Unit>();
            if (!_spottedEnemies.Contains(enemyUnit))
            {
                _spottedEnemies.Add(enemyUnit);
            }        
        }

        if (_targetUnit == null)
        {
            if (_spottedEnemies.Count == 1)
            {
                _targetUnit = _spottedEnemies[0];
                return;
            }

            Unit _nearestUnit = null;
            float _nearestDistance = float.PositiveInfinity;

            foreach (Unit enemy in _spottedEnemies)
            {
                var dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < _nearestDistance)
                {
                    _nearestDistance = dist;
                    _nearestUnit = enemy;
                }
            }

            _targetUnit = _nearestUnit;
        }
    }

    private void Chase(Unit targetUnit)
    {
        StopActionRoutine();
        _actionCoroutine = StartCoroutine(RunAfterUnit(targetUnit));
    }

    IEnumerator RunAfterUnit(Unit unit)
    {
        var distance = Vector3.Distance(transform.position, unit.transform.position);
        _isChasing = true;
        while (_targetUnit != null && distance > _attackDistance * .9f)
        {
            _agent.SetDestination(unit.transform.position);
            distance = Vector3.Distance(transform.position, unit.transform.position);
            yield return null;
            yield return null; //checking it every two frames as it is not as dynamic I guess
        }
        _isChasing = false;

        if (!_targetUnit.gameObject.activeSelf)
            _targetUnit = null;

        if (_targetUnit == null )
            Scout();
        else
            Fight();
        yield break;
    }

    private void Fight()
    {
        StopActionRoutine();
        _actionCoroutine = StartCoroutine(FightWithEnemy());
    }

    IEnumerator FightWithEnemy()
    {
        _isFighting = true;
        _agent.isStopped = true;
        while (_targetUnit != null && _targetUnit.Health > 0)
        {
            _targetUnit.GetDamage(_strenght);
            yield return new WaitForSeconds(1);
        }

        _agent.isStopped = false;
        _isFighting = false;
        CheckForEnemyToChase();

        if (_targetUnit == null)
            Scout();
        else
        {
            Chase(_targetUnit);
        }
        yield break;
    }

    private void Scout()
    {
        StopActionRoutine();
        _actionCoroutine = StartCoroutine(Wander());
    }

    private void StopActionRoutine()
    {
        _isChasing = false;
        _isFighting = false;
        _isScouting = false;
        if (_actionCoroutine == null)
            return;
        StopCoroutine(_actionCoroutine);
        _actionCoroutine = null;
    }
}

