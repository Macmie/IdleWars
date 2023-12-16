using System.Collections;
using UnityEngine;

public class EnemyUnit : Unit
{
    private void Awake()
    {
        World.Instance.AddEnemyToList();
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
