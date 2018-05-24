using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1 : Attack
{
    private float timer;
    private Object bulletPrefab;

    public Attack1(BossController boss) : base(boss)
    {
        this.timer = 0;
        this.bulletPrefab = Resources.Load("Prefabs/Bullet");
    }

    public override void Action(float deltaTime)
    {
        float currTime = Time.time;

        if (currTime - timer >= boss.attackCooldown)
        {
            timer = currTime;

            GameObject bullet = (GameObject)GameObject.Instantiate(bulletPrefab);
            bullet.GetComponent<BulletController>().owner = boss.gameObject;
            bullet.transform.position = boss.gameObject.transform.position;
            bullet.tag = "BossAttack";
            bullet.GetComponent<Rigidbody>().velocity = (-boss.transform.right + new Vector3(Random.Range(-0.6f, 0.6f), 0, Random.Range(-0.6f, 0.6f))) * boss.attackSpeed;
        }
    }
}
