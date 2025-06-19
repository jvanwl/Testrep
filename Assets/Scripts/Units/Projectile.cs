using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    private float speed;
    private bool isEnemyProjectile;
    private Unit target;
    private bool isInitialized;

    public void Initialize(int damage, float speed, Unit target, bool isEnemyProjectile)
    {
        this.damage = damage;
        this.speed = speed;
        this.target = target;
        this.isEnemyProjectile = isEnemyProjectile;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move towards target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotate projectile to face direction of travel
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Check if we hit the target
        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        target.TakeDamage(damage);
        // Trigger hit effect
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit != null && unit.IsEnemy != isEnemyProjectile)
        {
            HitTarget();
        }
    }
}
