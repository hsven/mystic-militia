using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : EntityController
{
    private Vector2 commandDirection;
    private Vector2 posOffset;

    [Header("Unit specific data")]

    public PlayerController player;
    public int patrolRadius = 1;
    public GameEnums.CommandTypes currentCommand = GameEnums.CommandTypes.FOLLOW;

    public int unitArmyIndex = 0;
    public Vector2Int unitSquadIndex = new Vector2Int(-1, -1);

    // Start is called before the first frame update
    void Start()
    {
        // unitArmyIndex = BattleManager.Instance.RegisterPlayerUnit(this);   
        unitSquadIndex = BattleManager.Instance.GetSquadIndex(this);

        if (!player) player = BattleManager.Instance.player;
        targetPos = player.GetPosition();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UnitMovement();
    }

    void UnitMovement() {
        switch (currentCommand)
        {
            case GameEnums.CommandTypes.FOLLOW:
                Movement(player.GetPosition(), posOffset);
                break;
            case GameEnums.CommandTypes.ATTACK:
                // TODO: Make this a variable
                targetPos += commandDirection * 0.01f;
                Movement(targetPos, posOffset);
                break;
            case GameEnums.CommandTypes.DEFEND:
                Movement(targetPos, posOffset);
                break;
            case GameEnums.CommandTypes.PATROL:
                float timeDelta = Time.fixedTime / 1000;
                float angle = 90 * timeDelta;

                Vector2 newTarget = new Vector2(
                    Mathf.Sin(angle * 10) * patrolRadius,
                    Mathf.Cos(angle * 10) * patrolRadius
                ) + targetPos;
                Movement(newTarget, posOffset);
                break;

            default:
                break;
        }
    }

    public void SetCommand(GameEnums.CommandTypes newCommand, Vector2 newTargetPos) {
        currentCommand = newCommand;
        targetPos = newTargetPos;

        commandDirection = (targetPos - player.GetPosition()).normalized;

        posOffset = BattleManager.Instance.GetUnitOffset(unitArmyIndex, unitSquadIndex);
        if (newCommand == GameEnums.CommandTypes.FOLLOW) {
            posOffset -= player.GetPosition() - targetPos;
        }
    }
}
