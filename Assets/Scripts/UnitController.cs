using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : EntityController
{
    private Vector2 commandDirection;
    private Vector2 posOffset;
    private bool enemyInSight = false;
    private GameObject enemyObj;

    [Header("Unit specific data")]

    public PlayerController player;

    public CircleCollider2D spotRadius;

    public int patrolRadius = 1;
    public GameEnums.CommandTypes currentCommand = GameEnums.CommandTypes.FOLLOW;

    public int unitArmyIndex = 0;
    public Vector2Int unitSquadIndex = new Vector2Int(-1, -1);

    public GameObject Enemy1;
    public GameObject Enemy2;

    // Start is called before the first frame update
    public void Setup(int totalPosIndex, Vector2Int squadIndex)
    {
        unitArmyIndex = totalPosIndex;
        unitSquadIndex = squadIndex;
        if (!player) player = BattleManager.Instance.player;
        targetPos = player.GetPosition();

        spotRadius.radius = patrolRadius;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UnitMovement();
    }

    void UnitMovement()
    {
        if (player == null)
        {
            player = BattleManager.Instance.player;
        }
        switch (currentCommand)
        {
            case GameEnums.CommandTypes.FOLLOW:
                Movement(player.GetPosition(), posOffset);
                break;
            case GameEnums.CommandTypes.ATTACK:
                if (enemyInSight) {
                    if(!enemyObj) {
                        enemyInSight = false;
                        break;
                    }
                    Movement(enemyObj.transform.position, Vector2.zero);
                }
                else {
                    // TODO: Make this a variable
                    targetPos += commandDirection * 0.01f;
                    Movement(targetPos, posOffset);
                }

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

        enemyInSight = false;
        enemyObj = null;
    }

    // TODO: Note that, after a new command is set, the targetting disapears until a reentry
    // I think this is fine (units are not glued to a target on successive attack commands), but may lead to bugs in the future 
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.root.CompareTag("Enemy")) {
            enemyInSight = true;
            enemyObj = other.gameObject;
        }
    }
}
