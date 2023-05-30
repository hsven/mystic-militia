using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{

    [SerializeField]
    protected Rigidbody2D rb;

    protected Vector2 targetPos;

    [Header("Movement related")]
    [Tooltip("Sets whether to use MovePosition (old) or AddForce (new). Check the code's comments to know the relevant variables")]
    public bool useOldMovement = false;
    
    [Range(1, 1000)]
    public int lifePoint = 100;

    [Range(1, 100)]
    public int power = 10;

    [Range(1, 100)]
    public int movementSpeed = 50;
    public int maxSpeed = 10;

    protected void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }


    protected void Movement(Vector2 target, Vector2 offset)
    {
        //Relevant variables: movementSpeed
        Vector2 currentPos = rb.position;
        if (useOldMovement) {
            float realSpeed = 100 - movementSpeed;
            Vector2 newPos = currentPos + (target + offset - currentPos) / realSpeed;
            rb.MovePosition(newPos);
            return;
        }

        //Relevant variables: rb.linearDrag, rb.mass, movementSpeed, maxSpeed
        var dir = (target + offset - currentPos);
        rb.AddForce((dir.normalized * movementSpeed) / (1 / dir.magnitude));

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    public void TakeDamage(int damage)
    {
        lifePoint -= damage;

        if (lifePoint <= 0)
        {
            BattleManager.Instance.DeleteEntity(this);
            Destroy(gameObject);
        }
    }
}
