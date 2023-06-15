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
            Destroy(gameObject);
            return;
        }

        targetPosition = target.transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        transform.Rotate(Vector3.forward, 100f * Time.deltaTime);

        HittingEnemy();
    }
}
