using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityController : MonoBehaviour
{

    [SerializeField]
    protected Rigidbody2D rb;

    protected Vector2 targetPos;

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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            BattleManager.Instance.DeleteEntity(this);
            Destroy(gameObject);
        }
    }

    public void UpdateHealthBar()
    {
        float healthRatio = (float)currentHealth / maxHealth;

        healthBarRed.rectTransform.sizeDelta = new Vector2((1 - healthRatio) * healthBarRed.rectTransform.sizeDelta.x, healthBarRed.rectTransform.sizeDelta.y);

        float xOffset = (0.2f - healthBarRed.rectTransform.sizeDelta.x) * 0.5f;
        healthBarRed.rectTransform.position = new Vector2(healthBarRed.rectTransform.position.x + xOffset, healthBarRed.rectTransform.position.y);
    }

}
