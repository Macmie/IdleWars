using System.Collections;
using UnityEngine;

public class EnemyUnit : Unit
{
    public override void OnSpawnedObject()
    {
        World.Instance.AddEnemyToList();
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
        World.Instance.RemoveEnemyFromList();
        base.Die();
    }
}
