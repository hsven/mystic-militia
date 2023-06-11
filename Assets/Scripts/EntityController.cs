using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityController : MonoBehaviour
{
    [SerializeField]
    private Collider2D hitbox;
    [SerializeField]
    private Collider2D hurtbox;

    [SerializeField]
    public Rigidbody2D rb;

    private float triggerDelay = .5f;
    float timer;

    protected Vector2 targetPos;

    public GameEnums.EntityType entityType = GameEnums.EntityType.DISTANCE;

    [Header("Movement related")]
    [Tooltip("Sets whether to use MovePosition (old) or AddForce (new). Check the code's comments to know the relevant variables")]
    public bool useOldMovement = false;

    [Range(1, 100)]
    public int movementSpeed = 50;
    public int maxSpeed = 10;
    
    [Header("Battle caracteristics")]
    public int maxHealth = 100;
    public int power = 10;
    protected int currentHealth;

    public Image healthBarGreen;
    public Image healthBarRed;

    protected Vector3 arrowInitialLocalPosition;
    protected Quaternion arrowInitialLocalRotation;
    private EntityController targetEntity;
    public int timerArrow = 0;
    protected int shootingRange = 3;
    protected bool isAlive = true;

    protected void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
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

    protected void launchArrow(EntityController target)
    {
        timerArrow = 100;
        BattleManager.Instance.NewArrow(this, target);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Transform entitySprite = transform.Find("Sprite");
            entitySprite.GetComponent<SpriteRenderer>().enabled = false;
            killEntity();
        }
    }

    protected void killEntity()
    {
        if (timerArrow >= 0)
        {
            BattleManager.Instance.DeleteEntity(this);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    { 
        if (entityType == GameEnums.EntityType.DISTANCE) return;

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

    public void UpdateHealthBar()
    {
        float healthRatio = (float)currentHealth / maxHealth;

        healthBarGreen.rectTransform.sizeDelta = new Vector2(healthRatio * healthBarGreen.rectTransform.sizeDelta.x, healthBarGreen.rectTransform.sizeDelta.y);

        float xOffset = healthBarGreen.rectTransform.sizeDelta.x * 0.5f - 0.2f;
        healthBarGreen.rectTransform.position = new Vector2(healthBarRed.rectTransform.position.x + xOffset, healthBarGreen.rectTransform.position.y);
    }

    public Vector2 GetPosition() {
        return rb.position;
    }
}
