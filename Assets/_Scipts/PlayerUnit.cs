using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    private List<EnemyUnit> _spottedEnemies = new List<EnemyUnit>();
    private EnemyUnit _targetUnit;
    private bool _isChasing, _isScouting, _isFighting;
    
    private Coroutine _actionCoroutine;

    private void Update()
    {
        if ( !_initialized)
        {
            return;
        }
        CheckForEnemyToChase();

        if (_targetUnit == null && !_isScouting)
            Scout();
        else if (!_isChasing && !_isFighting)
            Chase(_targetUnit);
    }

    private void CheckForEnemyToChase()
    {
        if (_spottedEnemies.Count == 0)
        {
            if (_targetUnit != null)
            {
                _targetUnit = null;
                Scout();
            }
            else
            {
                return;
            }
        }

        else if (_targetUnit == null)
        {
            if (_spottedEnemies.Count == 1)
            {
                _targetUnit = _spottedEnemies[0];
                Chase(_targetUnit);
                return;
            }


            EnemyUnit _nearestUnit = null;
            float _nearestDistance = float.PositiveInfinity;

            foreach (EnemyUnit enemy in _spottedEnemies)
            {
                var dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < _nearestDistance)
                {
                    _nearestDistance = dist;
                    _nearestUnit = enemy;
                }
            }

            Chase(_nearestUnit);
        }
    }

    private void Chase(EnemyUnit targetUnit)
    {
        StopActionRoutine();
        _actionCoroutine = StartCoroutine(RunAfterUnit(targetUnit));
    }

    IEnumerator RunAfterUnit(EnemyUnit unit)
    {
        var distance = Vector3.Distance(transform.position, unit.transform.position);
        _isChasing = true;
        while (distance > _attackDistance)
        {
            _agent.SetDestination(unit.transform.position);
            yield return null;
            yield return null; //checking it every two frames as it is not as dynamic I guess
        }

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
        while (_targetUnit.Health > 0)
        {
            _targetUnit.GetDamage(_strenght);
            yield return new WaitForSeconds(1);
        }

        CheckForEnemyToChase();
        yield break;
    }

    private void Scout()
    {
        StopActionRoutine();
        _actionCoroutine = StartCoroutine(Wander());
        _isScouting = true;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyUnit enemy))
            TryAddEnemyToSpottedList(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out EnemyUnit enemy))
            TryRemoveEnemyFromSpottedList(enemy);
    }

    private void TryAddEnemyToSpottedList(EnemyUnit enemy)
    {
        if (!_spottedEnemies.Contains(enemy))
            _spottedEnemies.Add(enemy);
    }

    private void TryRemoveEnemyFromSpottedList(EnemyUnit enemy)
    {
        if (_spottedEnemies.Contains(enemy))
            _spottedEnemies.Remove(enemy);
    }
}
