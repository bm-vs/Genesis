using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack2 : Attack
{
    private GameObject attackObject;
    private float rotateSpeed = 300.0f;

    public Attack2(BossController boss) : base(boss)
    {
        this.attackObject = boss.transform.GetChild(0).gameObject;
    }

    public override void Action(float deltaTime)
    {
        if (!attackObject.active)
        {
            attackObject.SetActive(true);
        }
        boss.transform.Rotate(new Vector3(0, rotateSpeed, 0) * deltaTime);
    }
}
