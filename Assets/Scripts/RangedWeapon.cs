using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    public float speed;
    protected EntityController archery;
    protected EntityController target;
    protected Vector3 startPosition;
    protected Vector3 targetPosition;
    protected int power;
    protected bool isAlive = true;

    public void Initialize(EntityController archer, EntityController targetEntity)
    {
        archery = archer;
        target = targetEntity;
        power = archer.unitData.damage;
        targetPosition = targetEntity.transform.position;

        Vector3 arrowSize = transform.lossyScale;

        SpriteRenderer playerSpriteRenderer = archer.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        SpriteRenderer spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();

        float playerWidth = playerSpriteRenderer.bounds.size.x;
        float playerHeight = playerSpriteRenderer.bounds.size.y;

        float fireballWidth = spriteRenderer.bounds.size.x;
        float fireballHeight = spriteRenderer.bounds.size.y;

        float offsetX = (playerWidth - fireballWidth) / 2f;
        float offsetY = (playerHeight - fireballHeight) / 2f;

        Vector3 fireballPosition = new Vector3(archer.transform.position.x + offsetX, archer.transform.position.y + offsetY, transform.position.z);

        transform.position = fireballPosition;

        Vector3 direction = target.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        float distance = Vector3.Distance(transform.position, target.transform.position);
        float timeToReachTarget = distance / 5f;
        speed = distance / timeToReachTarget;
    }

    protected void Start()
    {
        startPosition = transform.position;
    }

    protected void HittingTarget(float distance)
    {
        float minDistance = 10;
        if (archery is EnemyController)
        {
            foreach (UnitController unit in BattleManager.Instance.totalPlayerUnits)
            {
                if (Vector3.Distance(unit.GetPosition(), transform.position) < distance)
                {
                    unit.TakeDamage(power);
                    isAlive = false;
                    return;
                }
            }
        }
        else if (archery is UnitController)
        {
            foreach (EnemyController enemy in BattleManager.Instance.enemies)
            {
                if (Vector3.Distance(enemy.GetPosition(), transform.position) < distance)
                {
                    enemy.TakeDamage(power);
                    isAlive = false;
                    return;
                }
            }
        }
    }
}
