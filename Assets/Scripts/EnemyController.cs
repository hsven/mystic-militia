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

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerController>(); 
    }

    private void FixedUpdate()
    {
        targetPos = player.GetPosition();
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
