using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private float speed;

    public float Speed { get => speed; set => speed = value; }

    private Stack<Node> path;

    [SerializeField]
    private Stat healthStat;

    [SerializeField]
    private int health;
    public int Health { get => health; set => health = value; }

    public bool Alive
    {
        get => healthStat.CurrentValue > 0;
    }

    private int distanceToEndPoint;

    public int DistanceToEndPoint { get => distanceToEndPoint; }

    public Point GridPosition{ get; set; }

    private Vector3 destination;

    public bool IsActive { get; set; }
    

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        healthStat.Initialize();
    }


    private void Update()
    {
        if (IsActive)
        {
            Move();
        }
    }

    public void Spawn(int health, float speed)
    {
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        GetComponent<Animator>().enabled = true;
        IsActive = false;
        Speed = speed;
        Health = health;
        this.healthStat.Bar.Reset();
        this.healthStat.MaxVal = Health;
        this.healthStat.CurrentValue = this.healthStat.MaxVal;
        
        //Debug.Log("Health max: " + this.healthStat.MaxVal + ", health current: " + this.healthStat.CurrentValue);
        
        Vector3 portalPosition = LevelManager.Instance.RedPortal.transform.position;
        transform.position = new Vector3(portalPosition.x + 0.55f, portalPosition.y - 1.5f, -1);
        
        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1, 1)));
        
        SetPath(LevelManager.Instance.Path);
    }

    public IEnumerator Scale(Vector3 from, Vector3 to)
    {
        float progress = 0;

        while (progress <= 1)
        {
            transform.localScale = Vector3.Lerp(from, to, progress);
            progress += Time.deltaTime*5;
            yield return null;
        }
        transform.localScale = to;
        IsActive = true;
    }

    private void Move()
    {

        Vector3 destination2 = new Vector3(destination.x + 0.65f, destination.y - 1.2f);
        transform.position = Vector2.MoveTowards(transform.position, destination2, Speed * Time.deltaTime);
        
        if (transform.position == destination2)
        {
            if (path != null && path.Count > 0)
            {
                Animate(GridPosition, path.Peek().GridPosition);
                GridPosition = path.Peek().GridPosition;
                destination = path.Pop().WorldPosition;
            }
            else if (path != null && path.Count == 0)
            {
                EndPath();
            }

        }
    }

    private void SetPath(Stack<Node> newPath)
    {
        if (newPath != null)
        {
            this.path = newPath;
            Animate(GridPosition, path.Peek().GridPosition);
            GridPosition = path.Peek().GridPosition;            
            destination = path.Pop().WorldPosition;
        }
    }

    private void Animate(Point currentPos, Point newPos)
    {
        if (currentPos. Y < newPos.Y)
        {
            //Down
            //Debug.Log("Down");
            animator.SetInteger("Horizontal", 0);
            animator.SetInteger("Vertical", -1);
        }
        else if (currentPos.Y > newPos.Y)
        {
            //Up
            //Debug.Log("Up");
            animator.SetInteger("Horizontal", 0);
            animator.SetInteger("Vertical", 1);
        }
        if (currentPos.Y == newPos.Y)
        {
            if (currentPos.X > newPos.X)
            {
                //Left
                //Debug.Log("Left");
                animator.SetInteger("Horizontal", -1);
                animator.SetInteger("Vertical", 0);
            }
            else if (currentPos.X < newPos.X)
            {
                //Right
                //Debug.Log("Right");
                animator.SetInteger("Horizontal", 1);
                animator.SetInteger("Vertical", 0);
            }
        }
    }

    private void EndPath()
    {
        Release();
        GameManager.Instance.Lives--;
    }

    public void Release()
    {
        GameManager.Instance.Pool.ReleaseObject(gameObject);
        GridPosition = LevelManager.Instance.StartPath;
        GameManager.Instance.RemoveMonster(this);
    }

    public void TakeDamage(int damage)
    {
        if (IsActive)
        {
            this.healthStat.CurrentValue -= damage;
            if (this.healthStat.CurrentValue <= 0)
            {
                KillMonster();
                animator.SetTrigger("Death");
            }
        }
    }

    public void KillMonster()
    {
        IsActive = false;
        SoundManager.Instance.PlayEffect("monster_dying");
        GameManager.Instance.MonstersKilled += 1;
        GetComponent<SpriteRenderer>().sortingOrder--;
        GameManager.Instance.Currency += 1;
    }

}
