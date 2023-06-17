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

    public UnitData unitData;

    public Image healthBarGreen;
    public Image healthBarRed;

    protected Vector3 arrowInitialLocalPosition;
    protected Quaternion arrowInitialLocalRotation;

    private EntityController targetEntity;
    public int timerRangedWeapon = 0;
    protected bool isAlive = true;

    public int currentHealth;
    
    [Header("Movement related")]
    [Tooltip("Sets whether to use MovePosition (old) or AddForce (new). Check the code's comments to know the relevant variables")]
    public bool useOldMovement = false;



    protected void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    protected void Movement(Vector2 target, Vector2 offset)
    {
        //Relevant variables: movementSpeed
        Vector2 currentPos = rb.position;
        if (useOldMovement)
        {
            float realSpeed = 100 - unitData.movementSpeed;
            Vector2 newPos = currentPos + (target + offset - currentPos) / realSpeed;
            rb.MovePosition(newPos);
            return;
        }

        //Relevant variables: rb.linearDrag, rb.mass, movementSpeed, maxSpeed
        var dir = (target + offset - currentPos);
        rb.AddForce((dir.normalized * unitData.movementSpeed) / (1 / dir.magnitude));

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, unitData.movementSpeed);
    }

    protected void LaunchRangedWeapon(EntityController target)
    {
        timerRangedWeapon = 100;
        NewRangedWeapon(target);
    }

    public void SetupUnitData(UnitData data)
    {
        unitData = data;
        currentHealth = unitData.healthPoints;
    }

    public void SetupEnemyUnitData()
    {
        currentHealth = unitData.healthPoints;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Transform entitySprite = transform.Find("Sprite");
            entitySprite.GetComponent<SpriteRenderer>().enabled = false;
            KillEntity();
        }
    }

    protected void KillEntity()
    {
        if (timerRangedWeapon >= 0)
        {
            BattleManager.Instance.DeleteEntity(this);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (unitData.shootingRange > 0) return;

        timer -= Time.deltaTime;
        if (timer > 0) return;
        else timer = triggerDelay;

        if (other.transform.root.CompareTag("Enemy") && this.transform.root.CompareTag("Ally"))
        {
            var controller = other.transform.root.GetComponent<EnemyController>();
            if (!hitbox.IsTouching(controller.hurtbox)) return;

            Debug.Log("Enemy takes damage");
            controller.TakeDamage(unitData.damage);
        }
        else if (other.transform.root.CompareTag("Ally") && this.transform.root.CompareTag("Enemy"))
        {
            
            var controller = other.transform.root.GetComponent<UnitController>();
            if (!hitbox.IsTouching(controller.hurtbox)) return;

            Debug.Log("Ally takes damage");
            controller.TakeDamage(unitData.damage);
        }
    }

    public void UpdateHealthBar()
    {
        float healthRatio = (float)currentHealth / unitData.healthPoints;

        healthBarGreen.rectTransform.sizeDelta = new Vector2(healthRatio * healthBarRed.rectTransform.sizeDelta.x, healthBarRed.rectTransform.sizeDelta.y);

        float xOffset = healthBarGreen.rectTransform.sizeDelta.x * 0.5f - 0.2f;
        healthBarGreen.rectTransform.position = new Vector2(healthBarRed.rectTransform.position.x + xOffset, healthBarRed.rectTransform.position.y);
    }

    public Vector2 GetPosition()
    {
        return rb.position;
    }

    

    public void NewRangedWeapon(EntityController target)
    {
        if (this.unitData.name == "Archery" || this.unitData.name == "Crossbow")
        {
            Arrow arrow = CreateProjectile<Arrow>(target);
        }
        else if (this.unitData.name == "Pyroman")
        {
            FireBall fireBall = CreateProjectile<FireBall>(target);
        }
        else if (this.unitData.name == "Finisher" || this.unitData.name == "Mage")
        {
            MagicBall magicBall = CreateProjectile<MagicBall>(target);
        }
    }

    public T CreateProjectile<T>(EntityController target) where T : RangedWeapon
    {
        GameObject projectileObject = Instantiate(this.unitData.rangedWeaponPrefab);
        T projectile = projectileObject.GetComponent<T>();

        if (projectile == null)
        {
            projectile = projectileObject.AddComponent<T>();
        }

        projectile.Initialize(this, target);

        return projectile;
    }
}
