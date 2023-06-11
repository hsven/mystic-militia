using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed;
    private EntityController archery;
    private EntityController target;
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    private int power;

    private CircleCollider2D circleCollider;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
    }

    public void Initialize(EntityController archery, EntityController targetEntity)
    {
        target = targetEntity;
        power = archery.power;

        Vector3 arrowSize = transform.lossyScale;
        transform.position = new Vector3(   (int)archery.transform.position.x + arrowSize.x / 2, 
                                            (int)archery.transform.position.y + arrowSize.y / 2, 
                                            transform.position.z);

        Vector3 direction = target.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        float distance = Vector3.Distance(transform.position, target.transform.position);
        float timeToReachTarget = distance / 5f;
        speed = distance / timeToReachTarget;
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (target == null)
        {
            destroyArrow();
            return;
        }

        if (Vector3.Distance(target.GetPosition(), transform.position) < 0.3f)
        {
            if (target) target.TakeDamage(power);
            destroyArrow();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        Vector3 direction = target.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    private void destroyArrow()
    {
        BattleManager.Instance.arrows.Remove(this);
        Destroy(gameObject);
    }

}
