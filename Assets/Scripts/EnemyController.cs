using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    private List<Vector2> unitsPositions = new List<Vector2>();

    private PlayerController player;
    
    public GameEnums.EnemyTarget enemyTarget = GameEnums.EnemyTarget.UNIT;

    void Start()
    {
        if (!player) player = BattleManager.Instance.player;
        BattleManager.Instance.enemies.Add(this);
        
        Transform arrowTransform = transform.Find("Arrow");
        arrowInitialLocalPosition = arrowTransform.localPosition;
        arrowInitialLocalRotation = arrowTransform.localRotation;
    }

    private void Update()
    {
        unitsPositions = BattleManager.Instance.GetPlayerUnitPositions();
    }

    private void FixedUpdate()
    {
        if (!isAlive) killEntity();
        Vector2 ownPosition = new Vector2(transform.position.x, transform.position.y);
        int targetIndex = 0;

        if (enemyTarget == GameEnums.EnemyTarget.PLAYER)
        {
            targetPos = player.GetPosition();
        }
        else if (enemyTarget == GameEnums.EnemyTarget.UNIT)
        {
            if (unitsPositions.Count == 0) return;

            float minDistance = Mathf.Infinity;

            for (int i = 0; i < unitsPositions.Count; i++)
            {
                float newDistance = Vector2.Distance(unitsPositions[i], ownPosition);
                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    targetIndex = i;
                    targetPos = unitsPositions[i];
                }
            }
        }

        if (entityType == GameEnums.EntityType.DISTANCE && (Vector2.Distance(targetPos, ownPosition) < shootingRange || timerArrow != 0))
        {
            if (timerArrow == 0)
            {
               launchArrow(BattleManager.Instance.totalPlayerUnits[targetIndex]);
            }
            else if (timerArrow < 0)
            {
                flyingArrow();
            }
            else
            {
                timerArrow--;
            }
        }
        else
        {
            Movement(targetPos, Vector2.zero);
        }
    }
}
