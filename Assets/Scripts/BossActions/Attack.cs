using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack
{
    protected BossController boss;

    protected Attack(BossController boss)
    {
        this.boss = boss;
    }
    abstract public void Action(float deltaTime);
}
