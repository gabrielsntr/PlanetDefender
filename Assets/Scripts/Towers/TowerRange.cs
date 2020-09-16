using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerRange : MonoBehaviour
{

    private Tower parent;

    private Animator animator;

    [SerializeField]
    private int damage;

    private Monster target;

    public Monster Target { get => target; }

    [SerializeField]
    private string projectileType;

    [SerializeField]
    private float projectileSpeed;

    public float ProjectileSpeed { get => projectileSpeed; }
    
    
    public int Damage { get => damage; }
    public float AttackCooldown { get => attackCooldown; }
    public string ProjectileType { get => projectileType; }

    //private Queue<Monster> monsters = new Queue<Monster>();

    private List<Monster> monsters = new List<Monster>();

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
        target = FindClosestMonsterToEnd();
        TurretTurn();
        if (target != null)
        {
            if (canAttack)
            {

                animator.SetTrigger("Attack");
                Shoot();
                canAttack = false;
            }
        }
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= AttackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }
    }

    private void Shoot()
    {
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.Initialize(this);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            //monsters.Enqueue(other.GetComponent<Monster>());
            monsters.Add(other.GetComponent<Monster>());
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            monsters.Remove(other.GetComponent<Monster>());
            GameObject go = other.gameObject;
            if (go.activeInHierarchy)
            {
                //target = null;
                monsters.Remove(other.GetComponent<Monster>());
            }
        }
    }

    public void TurretTurn()
    {
        if (target != null)
        {
            GameObject turret = parent.transform.GetChild(1).gameObject;
            /*Vector2 direction = new Vector2(
            target.transform.position.x - turret.transform.position.x,
            target.transform.position.y - turret.transform.position.y
            );
            turret.transform.up = direction;*/
            float angle = 
                Mathf.Atan2(target.transform.position.y - turret.transform.position.y, 
                target.transform.position.x - turret.transform.position.x) * Mathf.Rad2Deg;
            
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle-90));

            turret.transform.rotation = Quaternion.Lerp(turret.transform.rotation, targetRotation, 50*Time.deltaTime);


        }
    }

    private Monster FindClosestMonsterToEnd()
    {
        float distanceToClosest = Mathf.Infinity;
        Monster target = null;
        foreach (Monster monster in monsters)
        {
            if (monster.IsActive && monster.Alive)
            {
                float distanceToMonster = Vector2.Distance(
                    new Vector2(monster.transform.position.x, monster.transform.position.y),
                    new Vector2(LevelManager.Instance.EndPath.X, LevelManager.Instance.EndPath.Y));
                if (distanceToMonster < distanceToClosest)
                {
                    distanceToClosest = distanceToMonster;
                    target = monster;
                }
            }
        }
        return target;
    }

}
