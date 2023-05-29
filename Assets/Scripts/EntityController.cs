using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody2D rb;

    protected Vector2 targetPos;

    [Range(1, 100)]
    public int movementSpeed = 50;

    protected void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    protected void Movement(Vector2 target, Vector2 offset)
    {
        Vector2 currentPos = rb.position;
        
        float realSpeed = 100 - movementSpeed;
        Vector2 newPos = currentPos + (target + offset - currentPos) / realSpeed;
        rb.MovePosition(newPos);
    }
}
