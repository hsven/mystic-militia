using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [SerializeField]
    private Collider2D hitbox;
    [SerializeField]
    private Collider2D hurtbox;

    [SerializeField]
    protected Rigidbody2D rb;

    private float triggerDelay = .1f;
    float timer;

    protected Vector2 targetPos;

    [Header("Movement related")]
    [Tooltip("Sets whether to use MovePosition (old) or AddForce (new). Check the code's comments to know the relevant variables")]
    public bool useOldMovement = false;

    [Range(1, 100)]
    public int movementSpeed = 50;
    public int maxSpeed = 10;

    [Header("Battle characteristics")]
    public int lifePoint = 100;
    public int power = 10;

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

    private void OnTriggerStay2D(Collider2D other)
    { 
        timer -= Time.deltaTime;
        if (timer > 0) return;
        else timer = triggerDelay;

        if (other.transform.root.CompareTag("Enemy") && this.transform.root.CompareTag("Ally"))
        {
            var controller = other.transform.root.GetComponent<EnemyController>();
            if (!hitbox.IsTouching(controller.hurtbox)) return;

            Debug.Log("Enemy take damages");
            controller.TakeDamage(power);
        }
        else if (other.transform.root.CompareTag("Ally") && this.transform.root.CompareTag("Enemy"))
        {

            var controller = other.transform.root.GetComponent<UnitController>();
            if (!hitbox.IsTouching(controller.hurtbox)) return;
            
            Debug.Log("Ally take damages");
            controller.TakeDamage(power);
        }
    }
}
