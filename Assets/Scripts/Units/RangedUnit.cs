using UnityEngine;

public class RangedUnit : Unit
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float minAttackRange = 3f;

    protected override void Start()
    {
        base.Start();
        Type = UnitType.Ranged;
        attackRange = 8f; // Longer range than melee units
    }

    public override void UpdateUnit()
    {
        if (IsDead) return;

        FindTarget();
        
        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            
            if (distance <= attackRange && distance >= minAttackRange)
            {
                Attack();
            }
            else if (distance < minAttackRange)
            {
                // Move away from target
                Retreat();
            }
            else
            {
                Move();
            }
        }
        else
        {
            Move();
        }
    }

    protected override void Attack()
    {
        if (Time.time - lastAttackTime >= attackSpeed)
        {
            FireProjectile();
            lastAttackTime = Time.time;
            // Trigger attack animation
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab != null && currentTarget != null)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            
            if (projectile != null)
            {
                projectile.Initialize(damage, projectileSpeed, currentTarget, IsEnemy);
            }
        }
    }

    private void Retreat()
    {
        float direction = isMovingRight ? -1f : 1f; // Move in opposite direction
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
    }
}
