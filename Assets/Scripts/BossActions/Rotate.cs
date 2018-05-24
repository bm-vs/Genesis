using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate {
	private BossController boss;
	private Vector3 direction;
	private Vector3 ledgeNormal;

	public Rotate(BossController boss) {
		this.boss = boss;
	}

	public void Action(float deltaTime)
    {
        boss.transform.Rotate(new Vector3(0, boss.rotateSpeed, 0) * deltaTime);
    }
}
