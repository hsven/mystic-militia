using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    private List<Vector2> unitsPositions = new List<Vector2>();

    private PlayerController player;
    public GameEnums.EnemyTarget EnemyTarget = GameEnums.EnemyTarget.UNIT;

    void Start()
    {
        if (!player) player = BattleManager.Instance.player;
        BattleManager.Instance.enemies.Add(this);
    }

    private void Update() {
        unitsPositions = BattleManager.Instance.GetPlayerUnitPositions();
    }

    private void FixedUpdate()
    {
        if (!isAlive) KillEntity();
        
        Vector2 ownPosition = new Vector2(transform.position.x, transform.position.y);
        int targetIndex = 0;

        if (EnemyTarget == GameEnums.EnemyTarget.PLAYER)
        {
            targetPos = player.GetPosition();
        }
        else if (EnemyTarget == GameEnums.EnemyTarget.UNIT)
        {
            if (unitsPositions.Count == 0) return;

            float minDistance = Mathf.Infinity;

            for (int i = 0; i < unitsPositions.Count; i++)
            {
                float newDistance = Vector2.Distance(unitsPositions[i], ownPosition);
                if (newDistance < minDistance){
                    minDistance = newDistance;
                    targetIndex = i;
                    targetPos = unitsPositions[i];
                }
            }
        }

        if (unitData.shootingRange > 0 && IsWithinShootingRange(targetPos, ownPosition))
        {
            HandleRangedAttack(targetIndex);
        }
        else
        {
            Movement(targetPos, Vector2.zero);
        }
    }

    private bool IsWithinShootingRange(Vector2 targetPosition, Vector2 ownPosition)
    {
        float distance = Vector2.Distance(targetPosition, ownPosition);
        return distance < unitData.shootingRange;
    }

    private void HandleRangedAttack(int targetIndex)
    {
        if (timerRangedWeapon == 0)
        {
            LaunchRangedWeapon(BattleManager.Instance.totalPlayerUnits[targetIndex], unitData);
        }
        else
        {
            timerRangedWeapon--;
        }
    }
}
