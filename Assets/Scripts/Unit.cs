using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{

    public enum Task
    {
        idle, move, follow, chase, attack
    }

    const string ANIMATOR_SPEED = "Speed";
    const string ANIMATOR_ALIVE = "Alive";
    const string ANIMATOR_SHOOTING = "Shooting";

    public static List<ISelectable> SelectableUnits { get { return selectableUnits; } }
    static List<ISelectable> selectableUnits = new List<ISelectable>();

    public float HealthPercent { get { return hp / hpMax; } }
    public bool isAlive { get { return hp > 0; } }

    [Header("Unit")]
    [SerializeField]
    GameObject hpBarPrefab;
    [SerializeField]
    float hp, hpMax = 100;
    [SerializeField]
    protected float shootingDistance = 1;
    [SerializeField]
    protected float stoppingDistance = 1;
    [SerializeField]
    protected float shootingCooldown = 1;
    [SerializeField]
    protected float shootingDamage = 0;

    protected  Transform target;
    protected HPBar healthBar;
    protected Task task = Task.idle;
    protected NavMeshAgent nav;

    float attackTimer;

    Animator animator;
    

    protected virtual void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        hp = hpMax;
        healthBar =  Instantiate(hpBarPrefab, transform).GetComponent<HPBar>();
    }

    private void Start()
    {
        if (this is ISelectable)
        {
            selectableUnits.Add(this as ISelectable);
            (this as ISelectable).SetSelected(false);
        }
            
    }

    private void OnDestroy()
    {
        if (this is ISelectable)
            selectableUnits.Remove(this as ISelectable);
    }

    void Update()
    {
       
        if(isAlive)
            switch (task)
            {
            case Task.idle: Idling(); break;
            case Task.move: Moving(); break;
            case Task.attack: Attacking(); break;
            case Task.follow: Following(); break;
            case Task.chase: Chasing(); break;
            }
        Animate();
    }

    protected virtual void Idling()
    {
        nav.velocity = Vector3.zero;
    }
    protected virtual void Attacking()
    {
        if(target)
        {
            nav.velocity = Vector3.zero;
            transform.LookAt(target);
            float distance = Vector3.Magnitude(nav.destination - transform.position);
            if (distance <= shootingDistance)
            {             
                if((attackTimer -= Time.deltaTime) <= 0)
                {
                    Shoot();
                }
            }
            else
            {
                task = Task.chase;
            }
        }
        else
        {
            task = Task.idle;
        }
         

    }
    protected virtual void Moving()
    {
        float distance = Vector3.Magnitude(nav.destination - transform.position);
         if(distance <= stoppingDistance)
         {
            task = Task.idle;
         }
    }
    protected virtual void Following()
    {
        if (target)
        {
            nav.SetDestination(target.position);
        }
        else
        {
            task = Task.idle;
        }
    }
    protected virtual void Chasing()
    {
        if (target)
        {
            nav.SetDestination(target.position);
            float distance = Vector3.Magnitude(nav.destination - transform.position);
            if (distance <= shootingDistance)
            {
                task = Task.attack;
            }
        }
        else
        {
            task = Task.idle;
        }
    }

    protected virtual void Animate()
    {
        var speedVector = nav.velocity;
        speedVector.y = 0;
        float speed = speedVector.magnitude;
        animator.SetFloat(ANIMATOR_SPEED, speed);
        animator.SetBool(ANIMATOR_ALIVE, isAlive);
    }

    public virtual void Shoot()
    {
        animator.SetTrigger(ANIMATOR_SHOOTING);
        attackTimer = shootingCooldown;
    }

    public virtual void DealDamage()
    {
        if(target)
        {
            Unit unit = target.GetComponent<Unit>();
            if (unit && unit.isAlive)
            {
                unit.hp -= shootingDamage;
            }
            else
                target = null;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingDistance);
    }

}
