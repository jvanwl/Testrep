using UnityEngine;
using System.Collections;

public class Turret : Building
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float projectileSpeed = 15f;
    
    private float lastAttackTime;
    private Unit currentTarget;

    protected override void Start()
    {
        base.Start();
        Type = BuildingType.Turret;
    }

    protected override void Update()
    {
        base.Update();
        
        if (!isConstructing)
        {
            UpdateTurret();
        }
    }

    private void UpdateTurret()
    {
        if (currentTarget == null || currentTarget.IsDead || !IsInRange(currentTarget))
        {
            FindTarget();
        }

        if (currentTarget != null && Time.time - lastAttackTime >= attackSpeed)
        {
            Attack();
        }
    }

    private void FindTarget()
    {
        Unit[] units = FindObjectsOfType<Unit>();
        float closestDistance = float.MaxValue;
        Unit closestEnemy = null;

        foreach (Unit unit in units)
        {
            if (unit.IsEnemy == IsEnemy || unit.IsDead) continue;

            float distance = Vector3.Distance(transform.position, unit.transform.position);
            if (distance <= attackRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = unit;
            }
        }

        currentTarget = closestEnemy;
    }

    private bool IsInRange(Unit target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= attackRange;
    }

    private void Attack()
    {
        if (projectilePrefab != null && currentTarget != null)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            
            if (projectile != null)
            {
                int finalDamage = damage * level; // Damage increases with level
                projectile.Initialize(finalDamage, projectileSpeed, currentTarget, IsEnemy);
                lastAttackTime = Time.time;
                // Trigger attack animation/effects
            }
        }
    }

    protected override void CompleteConstruction()
    {
        base.CompleteConstruction();
        damage = damage * level; // Update damage based on level
        attackSpeed = attackSpeed * (1f - (level - 1) * 0.1f); // 10% faster attack speed per level
    }
}
