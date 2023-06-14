using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : RangedWeapon
{
    private SpriteRenderer spriteRenderer;
    private int arrowConsistance = 100;

    private void Update()
    {
        if (!isAlive) {
            destroyWeapon();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (arrowConsistance == 100) HittingTarget();

        if (Vector3.Distance(targetPosition, transform.position) < 0.1f)
        {
            if (arrowConsistance == 0) destroyWeapon();
            else arrowConsistance--;
            return;
        }
    }

    protected void destroyWeapon()
    {
        BattleManager.Instance.arrows.Remove((Arrow)this);
        Destroy(gameObject);
    }
}
