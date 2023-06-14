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

        if (arrowConsistance == 100) hitingTarget(0.3f);

        if (Vector3.Distance(targetPosition, transform.position) < 0.1f)
        {
            if (arrowConsistance == 0) destroyWeapon();
            else arrowConsistance--;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    protected void destroyWeapon()
    {
        BattleManager.Instance.arrows.Remove((Arrow)this);
        Destroy(gameObject);
    }
}
