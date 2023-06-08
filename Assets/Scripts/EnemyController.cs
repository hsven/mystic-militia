using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    private List<Vector2> unitsPositions = new List<Vector2>();
    private Vector3 arrowInitialLocalPosition;
    private Quaternion arrowInitialLocalRotation;

    private PlayerController player;
    
    private int timerArrow = 1;
    private UnitController targetUnit;

    public GameEnums.EnemyTarget enemyTarget = GameEnums.EnemyTarget.UNIT;
    public GameEnums.EnemyType enemyType = GameEnums.EnemyType.CONTACT;
    public int shootingRange = 0;

    void Start()
    {
        if (!player) player = BattleManager.Instance.player;
        Transform arrowTransform = transform.Find("Arrow");
        arrowInitialLocalPosition = arrowTransform.localPosition;
        arrowInitialLocalRotation = arrowTransform.localRotation;
    }

    private void Update()
    {
        unitsPositions = BattleManager.Instance.GetPlayerUnitPositions();
    }

    private void FixedUpdate()
    {
        Vector2 ownPosition = new Vector2(transform.position.x, transform.position.y);
        int targetIndex = 0;

        if (enemyTarget == GameEnums.EnemyTarget.PLAYER)
        {
            targetPos = player.GetPosition();
        }
        else if (enemyTarget == GameEnums.EnemyTarget.UNIT)
        {
            if (unitsPositions.Count == 0) return;

            float minDistance = Mathf.Infinity;

            for (int i = 0; i < unitsPositions.Count; i++)
            {
                float newDistance = Vector2.Distance(unitsPositions[i], ownPosition);
                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    targetIndex = i;
                    targetPos = unitsPositions[i];
                }
            }
        }

        if (enemyType == GameEnums.EnemyType.DISTANCE && (Vector2.Distance(targetPos, ownPosition) < 2.5 || timerArrow <= 0))
        {
            if (timerArrow == 0)
            {
                timerArrow = -1;
                
                transform.Find("Arrow").GetComponent<SpriteRenderer>().enabled = true;
                targetUnit = BattleManager.Instance.totalPlayerUnits[targetIndex];

            }
            else if (timerArrow < 0)
            {
                Vector2 targetArrow = targetUnit.GetPosition();
                Transform arrowTransform = transform.Find("Arrow");
                Vector3 arrowPosition = arrowTransform.position;
                arrowPosition += ((Vector3)targetArrow - arrowPosition).normalized * movementSpeed * Time.deltaTime;
                arrowTransform.position = arrowPosition;

                Vector3 direction = targetArrow - ownPosition;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                arrowTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                if (Vector3.Distance(targetArrow, arrowPosition) < 0.2)
                {
                    targetUnit.TakeDamage(25);
                    arrowTransform.GetComponent<SpriteRenderer>().enabled = false;

                    arrowTransform.localPosition = arrowInitialLocalPosition;
                    arrowTransform.localRotation = arrowInitialLocalRotation;
                    timerArrow = 100;
                }

            }
            else
            {
                timerArrow--;
            }
        }
        else
        {
            Movement(targetPos, Vector2.zero);
        }
    }
}
