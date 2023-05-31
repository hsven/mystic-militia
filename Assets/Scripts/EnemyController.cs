using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    private List<Vector2> unitsPositions = new List<Vector2>();

    private bool isTriggerEnabled = true;
    private float triggerDelay = 1f;
    private float lastTriggerTime;

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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isTriggerEnabled)
        {
            UnitController unit = other.gameObject.GetComponent<UnitController>();
            if (unit != null)
            {
                unit.TakeDamage(power);
                TakeDamage(unit.power);
            }

            isTriggerEnabled = false;
            lastTriggerTime = Time.time;
            StartCoroutine(EnableTriggerAfterDelay());
        }
    }

    private IEnumerator EnableTriggerAfterDelay()
    {
        while (Time.time - lastTriggerTime < triggerDelay)
        {
            yield return null;
        }

        isTriggerEnabled = true;
    }

}
