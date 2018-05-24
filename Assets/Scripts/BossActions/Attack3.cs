using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack3 : Attack
{
    private float timer;
    private GameObject attackObject;

    public Attack3(BossController boss) : base(boss)
    {
        this.timer = Time.time;
        this.attackObject = boss.transform.GetChild(1).gameObject;
    }

    public override void Action(float deltaTime)
    {
        if (!attackObject.active)
        {
            this.timer = Time.time;
            attackObject.SetActive(true);
            attackObject.transform.localScale += new Vector3(-attackObject.transform.localScale.x, 0, -attackObject.transform.localScale.z);
        }

        float currTime = Time.time;
        //float actualRatio = 0.5f * (currTime - timer);
        //Debug.Log(actualRatio);
        attackObject.transform.localScale += new Vector3(4f * deltaTime, 0, 4f * deltaTime);
    }
}
