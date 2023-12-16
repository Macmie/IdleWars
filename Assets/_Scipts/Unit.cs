using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class Unit : MonoBehaviour
{
    [SerializeField] private float _checkRadius;
    [SerializeField] protected float _attackDistance;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private SphereCollider _checkCollider;

    protected NavMeshAgent _agent;
    protected float _strenght, _maxHealth, _health;
    protected bool _initialized;

    public float Health => _health;

    public void InitializeUnit(float strMulti, float healthMulti)
    {
        _agent = GetComponent<NavMeshAgent>();
        _strenght *= strMulti;
        _maxHealth *= healthMulti;
        _checkCollider.radius = _checkRadius;
        _health = _maxHealth;

        _initialized = true;
    }

    public void GetDamage(float dmg)
    {
        _health -= dmg;
        if (_health < 0 )
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        StopAllCoroutines();
        Destroy(this);
    }

    protected IEnumerator Wander()
    {
        var point = GetRandomPointFromSurface();
        _agent.SetDestination(point);

        while (true)
        {
            while (Vector3.Distance(transform.position, point) > .1f)
                yield return null;
            point = GetRandomPointFromSurface();
        }

    }

    private Vector3 GetRandomPointFromSurface()
    {
        var surface = World.Instance.NavSurface;
        var bounds = surface.navMeshData.sourceBounds;

        float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, transform.position.y, z);

    }
}

