using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBall : RangedWeapon
{
    private SpriteRenderer spriteRenderer;
    private void Update()
    {
        if (archery.unitData.movementSpeed == 0) FocusLowest();

        if (target == null || !isAlive)
        {
            destroyWeapon();
            return;
        }

        targetPosition = target.transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        transform.Rotate(Vector3.forward, 100f * Time.deltaTime);

        HittingTarget();
    }

    protected void destroyWeapon()
    {
        BattleManager.Instance.magicBalls.Remove((MagicBall)this);
        Destroy(gameObject);
    }

    protected void HittingTarget()
    {
        SpriteRenderer unitSpriteRenderer = target.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        SpriteRenderer rangedWeapon = transform.Find("Sprite").GetComponent<SpriteRenderer>();

        if (unitSpriteRenderer.bounds.Intersects(rangedWeapon.bounds))
        {
            target.TakeDamage(power);
            isAlive = false;
            return;
        }
    }

    protected void FocusLowest()
    {
        foreach (UnitController unit in BattleManager.Instance.totalPlayerUnits)
        {
            if (unit.currentHealth / unit.unitData.healthPoints < target.currentHealth / target.unitData.healthPoints)
            {
                target = unit;
            }
        }
    }
}
