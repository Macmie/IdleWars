using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    public override void OnSpawnedObject()
    {
        World.Instance.AddPlayerUnitToList();
        base.OnSpawnedObject();
    }

    public override void GetDamage(float dmg)
    {
        base.GetDamage(dmg);
        if (_health <= 0)
        {
            Die();
        }
    }

    protected override void Die()
    {
        World.Instance.RemovePlayerUnitFromList();
        base.Die();
    }
}
