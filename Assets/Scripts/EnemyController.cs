using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    private Vector2 targetPos;

    [Range(1, 100)]
    public int movementSpeed = 50;

    public PlayerController player;

    [SerializeField]
    public GameEnums.EnemyTypes enemyType = GameEnums.EnemyTypes.UNIT;

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
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
            List<Vector2> unitsPositions = player.GetUnitsPositions();
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
        EnemyMovement(targetPos);
    }

    private void EnemyMovement(Vector2 target)
    {
        Vector2 currentPos = rb.position;
        
        float realSpeed = 100 - movementSpeed;
        Vector2 newPos = currentPos + (target - currentPos) / realSpeed;
        rb.MovePosition(newPos);
    }
}
