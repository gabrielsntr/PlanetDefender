using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{

    private Tower parent;

    private Animator animator;

    private Monster target;
    public Monster Target { get => target; }

    [SerializeField]
    private string projectileType;

    [SerializeField]
    private float projectileSpeed;

    public float ProjectileSpeed { get => projectileSpeed; }

    private Queue<Monster> monsters = new Queue<Monster>();

    private bool canAttack = true;

    private float attackTimer;

    [SerializeField]
    private float attackCooldown;

    // Start is called before the first frame update
    void Start()
    {
        parent = this.GetComponentInParent<Tower>();
        animator = parent.GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }


    private void Attack()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }
        if (target == null && monsters.Count > 0)
        {
            target = monsters.Dequeue();
        }
        if (target != null && target.IsActive)
        {
            if (canAttack)
            {
                TurretTurn();
                animator.SetTrigger("Attack");
                Shoot();
                canAttack = false;
            }
            
        }
    }

    private void Shoot()
    {
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.Initialize(this);
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Monster")
        {
            monsters.Enqueue(collider.GetComponent<Monster>());
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Monster")
        {
            target = null;
        }
    }

    public void TurretTurn()
    {
        GameObject turret = parent.transform.GetChild(1).gameObject;

        Vector2 direction = new Vector2(
        target.transform.position.x - turret.transform.position.x,
        target.transform.position.y - turret.transform.position.y
        );

        turret.transform.up = direction;

    }
}
