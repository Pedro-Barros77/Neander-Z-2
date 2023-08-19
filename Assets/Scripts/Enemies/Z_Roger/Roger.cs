using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roger : BaseEnemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        MovementSpeed = 0.5f;
        AccelerationSpeed = 1f;
        Health = 28f;
        Damage = 15f;
        HeadshotDamageMultiplier = 2f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
