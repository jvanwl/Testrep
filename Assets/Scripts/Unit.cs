using UnityEngine;
using System;

public abstract class Unit : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackSpeed = 1f;
    [SerializeField] protected int goldCost = 100;
    [SerializeField] protected int experienceValue = 20;

    [Header("Current State")]
    protected int currentHealth;
    protected float lastAttackTime;
    protected bool isMovingRight;
    protected Unit currentTarget;
    
    public UnitType Type { get; protected set; }
    public bool IsDead => currentHealth <= 0;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsEnemy { get; protected set; }

    public event Action<Unit> OnUnitDeath;
    public event Action<Unit, int> OnUnitDamaged;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        isMovingRight = !IsEnemy; // Enemies move left, allies move right
        GameManager.Instance.RegisterUnit(this);
    }

    public virtual void UpdateUnit()
    {
        if (IsDead) return;

        FindTarget();
        
        if (currentTarget != null && IsInRange(currentTarget))
        {
            Attack();
        }
        else
        {
            Move();
        }
    }

    protected virtual void FindTarget()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        float closestDistance = float.MaxValue;
        Unit closestEnemy = null;

        foreach (Unit unit in allUnits)
        {
            if (unit.IsEnemy == IsEnemy || unit.IsDead) continue;

            float distance = Vector3.Distance(transform.position, unit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = unit;
            }
        }

        currentTarget = closestEnemy;
    }

    protected virtual void Move()
    {
        float direction = isMovingRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
    }

    protected virtual void Attack()
    {
        if (Time.time - lastAttackTime >= attackSpeed)
        {
            currentTarget.TakeDamage(damage);
            lastAttackTime = Time.time;
            // Trigger attack animation here
        }
    }

    protected virtual bool IsInRange(Unit target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= attackRange;
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (IsDead) return;

        currentHealth -= damageAmount;
        OnUnitDamaged?.Invoke(this, damageAmount);

        if (IsDead)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        OnUnitDeath?.Invoke(this);
        GameManager.Instance.UnregisterUnit(this);
        
        if (IsEnemy)
        {
            GameManager.Instance.AddGold(goldCost / 2); // Reward for killing enemy
            GameManager.Instance.AddExperience(experienceValue);
        }

        // Trigger death animation here
        Destroy(gameObject, 1f); // Destroy after animation
    }

    protected virtual void OnDestroy()
    {
        GameManager.Instance?.UnregisterUnit(this);
    }
}

public enum UnitType
{
    Melee,
    Ranged,
    Flying,
    Siege
}
