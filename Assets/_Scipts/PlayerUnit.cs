using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    private void Awake()
    {
        World.Instance.AddPlayerUnitToList();
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
