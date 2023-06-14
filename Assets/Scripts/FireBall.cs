using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : RangedWeapon
{
    private SpriteRenderer spriteRenderer;
    private void Update()
    {
        if (target == null || !isAlive)
        {
            destroyWeapon();
            return;
        }

        targetPosition = target.transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        transform.Rotate(Vector3.forward, 100f * Time.deltaTime);

        HittingEnemy();
    }

    protected void destroyWeapon()
    {
        BattleManager.Instance.fireBalls.Remove((FireBall)this);
        Destroy(gameObject);
    }
}
