using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    private Vector2 targetPos;
    private Vector2 commandDirection;
    private Vector2 posOffset = Vector2.zero;

    public PlayerController player;
    [Range(1, 100)]
    public int movementSpeed = 2;
    public int patrolRadius = 5;
    public GameEnums.CommandTypes currentCommand = GameEnums.CommandTypes.FOLLOW;

    public int unitArmyIndex = 0;

    void Awake() {
        if (!rb) rb = GetComponent<Rigidbody2D>();    
    }

    // Start is called before the first frame update
    void Start()
    {
        unitArmyIndex = player.RegisterUnit(this);    
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
                ApplyMovement(player.GetPosition());
                break;
            case GameEnums.CommandTypes.ATTACK:
                // TODO: Make this a variable
                targetPos += commandDirection * 0.01f;
                ApplyMovement(targetPos);
                break;
            case GameEnums.CommandTypes.DEFEND:
                ApplyMovement(targetPos);
                break;
            case GameEnums.CommandTypes.PATROL:
                float timeDelta = Time.fixedTime / 1000;
                float angle = 90 * timeDelta;

                Vector2 target = new Vector2(
                    Mathf.Sin(angle * 10) * patrolRadius,
                    Mathf.Cos(angle * 10) * patrolRadius
                ) + targetPos;

                ApplyMovement(target);
                break;

            default:
                break;
        }
    }

    void ApplyMovement(Vector2 target) {
        Vector2 currentPos = rb.position;
        
        float realSpeed = 100 - movementSpeed;
        Vector2 newPos = currentPos + (target + posOffset - currentPos) / realSpeed;
        rb.MovePosition(newPos);

    }

    public void SetCommand(GameEnums.CommandTypes newCommand, Vector2 newTargetPos) {
        currentCommand = newCommand;
        targetPos = newTargetPos;

        commandDirection = (targetPos - player.GetPosition()).normalized;

        posOffset = BattleManager.Instance.GetUnitOffset(unitArmyIndex);
        if (newCommand == GameEnums.CommandTypes.FOLLOW) {
            posOffset -= player.GetPosition() - targetPos;
        }

    }
}
