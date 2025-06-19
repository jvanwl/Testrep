using UnityEngine;

public class MeleeUnit : Unit
{
    [SerializeField] private float chargeSpeed = 8f;
    [SerializeField] private float chargeDamageMultiplier = 1.5f;
    private bool isCharging;

    protected override void Start()
    {
        base.Start();
        Type = UnitType.Melee;
    }

    public override void UpdateUnit()
    {
        if (IsDead) return;

        FindTarget();
        
        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            
            if (distance <= attackRange)
            {
                isCharging = false;
                Attack();
            }
            else if (distance <= attackRange * 2 && !isCharging)
            {
                StartCharge();
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

    private void StartCharge()
    {
        isCharging = true;
        // Trigger charge animation
    }

    protected override void Move()
    {
        float currentSpeed = isCharging ? chargeSpeed : speed;
        float direction = isMovingRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * currentSpeed * Time.deltaTime);
    }

    protected override void Attack()
    {
        if (Time.time - lastAttackTime >= attackSpeed)
        {
            int finalDamage = isCharging ? Mathf.RoundToInt(damage * chargeDamageMultiplier) : damage;
            currentTarget.TakeDamage(finalDamage);
            lastAttackTime = Time.time;
            isCharging = false;
            // Trigger attack animation
        }
    }
}
