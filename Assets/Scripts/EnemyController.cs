using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    private List<Vector2> unitsPositions = new List<Vector2>();

    [SerializeField]
    private PlayerController player;

    public GameEnums.EnemyTypes enemyType = GameEnums.EnemyTypes.UNIT;
    // Start is called before the first frame update
    void Start()
    {
        if (!player) player = BattleManager.Instance.player;
    }

    private void Update() {
        unitsPositions = BattleManager.Instance.GetPlayerUnitPositions();
    }

    private void FixedUpdate()
    {
        if (enemyType == GameEnums.EnemyTypes.PLAYER){
            targetPos = player.GetPosition();
        } 
        else if (enemyType == GameEnums.EnemyTypes.UNIT)
        {
            if (unitsPositions.Count == 0) return;
            
            Vector2 ownPosition = new Vector2(transform.position.x, transform.position.y);
            float minDistance = Mathf.Infinity;
            
            for (int i = 0; i < unitsPositions.Count; i++)
            {
                float newDistance = Vector2.Distance(unitsPositions[i], ownPosition);
                if(newDistance < minDistance){
                    minDistance = newDistance;
                    targetPos = unitsPositions[i];
                }
            }
        }
        Movement(targetPos, Vector2.zero);
    }
}
