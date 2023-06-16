using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitController : EntityController
{
    private Vector2 commandDirection;
    private Vector2 posOffset;
    private bool enemyInSight = false;
    private GameObject enemyObj;
    public EnemyController targetEnemy;

    [Header("Unit specific data")]

    public PlayerController player;

    public CircleCollider2D spotRadius;

    public int patrolRadius = 1;
    public GameEnums.CommandTypes currentCommand = GameEnums.CommandTypes.FOLLOW;

    public int unitArmyIndex = 0;
    public Vector2Int unitSquadIndex = new Vector2Int(-1, -1);

    [Header("Sprite characteristics")]

    public bool isBorderActive = false;

    [SerializeField]
    private SpriteRenderer border1Sprite;

    [SerializeField]
    private SpriteRenderer border2Sprite;

    [SerializeField]
    private SpriteRenderer border3Sprite;

    [SerializeField]
    private SpriteRenderer border4Sprite;

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
        if (!isAlive) KillEntity();
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
                if (enemyInSight)
                {
                    if (!enemyObj)
                    {
                        enemyInSight = false;
                        break;
                    }
                    Movement(enemyObj.transform.position, Vector2.zero);
                }
                else
                {
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

        if (unitData.shootingRange > 0)
        {
            Vector2 ownPosition = new Vector2(transform.position.x, transform.position.y);

            if (timerRangedWeapon == 0)
            {
                targetEnemy = BattleManager.Instance.enemies
                    .OrderBy(enemy => Vector2.Distance(ownPosition, enemy.GetPosition()))
                    .FirstOrDefault();
            }

            if (targetEnemy != null && (Vector2.Distance(targetEnemy.GetPosition(), this.GetPosition()) < unitData.shootingRange || timerRangedWeapon != 0))
            {
                if (timerRangedWeapon == 0)
                {
                    LaunchRangedWeapon(targetEnemy);
                }
                else
                {
                    timerRangedWeapon--;
                }
            }
            else if (timerRangedWeapon > 0)
            {
                timerRangedWeapon--;
            }
            else
            {
                Movement(targetPos, Vector2.zero);
            }
        }
    }

    public void SetCommand(GameEnums.CommandTypes newCommand, Vector2 newTargetPos, Vector2 offset) {
        currentCommand = newCommand;
        targetPos = newTargetPos;

        commandDirection = (targetPos - player.GetPosition()).normalized;

        posOffset = offset;
        if (newCommand == GameEnums.CommandTypes.FOLLOW) {
            posOffset -= player.GetPosition() - targetPos;
        }

        enemyInSight = false;
        enemyObj = null;
    }

    // TODO: Note that, after a new command is set, the targetting disapears until a reentry
    // I think this is fine (units are not glued to a target on successive attack commands), but may lead to bugs in the future 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Enemy"))
        {
            enemyInSight = true;
            enemyObj = other.gameObject;
        }
    }

    public void SetUnitBorders(bool isActive)
    {
        border1Sprite.gameObject.SetActive(isActive);
        border2Sprite.gameObject.SetActive(isActive);
        border3Sprite.gameObject.SetActive(isActive);
        border4Sprite.gameObject.SetActive(isActive);
    }
}
