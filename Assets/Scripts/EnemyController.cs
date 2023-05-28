using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    public PlayerController player;

    [SerializeField]
    public GameEnums.EnemyTypes enemyType = GameEnums.EnemyTypes.UNIT;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>(); 
    }

    private void FixedUpdate()
    {
        if (enemyType == GameEnums.EnemyTypes.PLAYER){
            targetPos = player.GetPosition();
        } 
        else if (enemyType == GameEnums.EnemyTypes.UNIT)
        {
            Vector2 enemyPosition = new Vector2(transform.position.x, transform.position.y);
            List<Vector2> unitsPositions = BattleManager.Instance.GetPlayerUnitPositions();
            float minDistance = Vector2.Distance(enemyPosition, unitsPositions[0]);
            
            for (int i = 1; i < unitsPositions.Count; i++)
            {
                float newDistance = Vector2.Distance(unitsPositions[i], enemyPosition);
                if(newDistance < minDistance){
                    minDistance = newDistance;
                    targetPos = unitsPositions[i];
                }
            }
        }
        Movement(targetPos, Vector2.zero);
    }
}
