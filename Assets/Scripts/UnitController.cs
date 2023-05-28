using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : EntityController
{
    private Vector2 commandDirection;

    public PlayerController player;
    public int patrolRadius = 5;
    public GameEnums.CommandTypes currentCommand = GameEnums.CommandTypes.FOLLOW;

    // Start is called before the first frame update
    void Start()
    {
        player.RegisterUnit(this);    
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
                Movement(player.GetPosition());
                break;
            case GameEnums.CommandTypes.ATTACK:
                // TODO: Make this a variable
                targetPos += commandDirection * 0.01f;
                Movement(targetPos);
                break;
            case GameEnums.CommandTypes.DEFEND:
                Movement(targetPos);
                break;
            case GameEnums.CommandTypes.PATROL:
                float timeDelta = Time.fixedTime / 1000;
                float angle = 90 * timeDelta;

                Vector2 target = new Vector2(
                    Mathf.Sin(angle * 10) * patrolRadius,
                    Mathf.Cos(angle * 10) * patrolRadius
                ) + targetPos;

                Movement(target);
                break;

            default:
                break;
        }
    }

    public void SetCommand(GameEnums.CommandTypes newCommand, Vector2 newTargetPos) {
        currentCommand = newCommand;
        targetPos = newTargetPos;
        commandDirection = (targetPos - player.GetPosition()).normalized;
    }
}