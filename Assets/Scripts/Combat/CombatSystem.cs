using UnityEngine;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    [System.Serializable]
    public class CombatStats
    {
        public float baseAttack = 10f;
        public float baseDefense = 5f;
        public float attackRange = 2f;
        public float attackSpeed = 1f;
        public bool canAttack = true;
    }

    [Header("Combat Settings")]
    [SerializeField] private float combatUpdateInterval = 0.5f;
    [SerializeField] private LayerMask combatLayers;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject deathEffectPrefab;

    private Dictionary<Unit, CombatStats> unitCombatStats = new Dictionary<Unit, CombatStats>();
    private Dictionary<Building, CombatStats> buildingCombatStats = new Dictionary<Building, CombatStats>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InvokeRepeating("UpdateCombat", 0f, combatUpdateInterval);
    }

    public void RegisterUnit(Unit unit, CombatStats stats)
    {
        if (!unitCombatStats.ContainsKey(unit))
        {
            unitCombatStats.Add(unit, stats);
        }
    }

    public void RegisterBuilding(Building building, CombatStats stats)
    {
        if (!buildingCombatStats.ContainsKey(building))
        {
            buildingCombatStats.Add(building, stats);
        }
    }

    private void UpdateCombat()
    {
        foreach (var unitPair in unitCombatStats)
        {
            if (unitPair.Value.canAttack)
            {
                CheckForTargets(unitPair.Key);
            }
        }

        foreach (var buildingPair in buildingCombatStats)
        {
            if (buildingPair.Value.canAttack)
            {
                CheckForTargets(buildingPair.Key.gameObject);
            }
        }
    }

    private void CheckForTargets(GameObject attacker)
    {
        CombatStats attackerStats = GetCombatStats(attacker);
        if (attackerStats == null) return;

        Collider[] hitColliders = Physics.OverlapSphere(
            attacker.transform.position, 
            attackerStats.attackRange,
            combatLayers
        );

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == attacker) continue;

            if (IsEnemy(attacker, hitCollider.gameObject))
            {
                PerformAttack(attacker, hitCollider.gameObject, attackerStats);
                break;
            }
        }
    }

    private bool IsEnemy(GameObject attacker, GameObject target)
    {
        // Get faction/team components and compare
        var attackerFaction = attacker.GetComponent<FactionMember>();
        var targetFaction = target.GetComponent<FactionMember>();

        if (attackerFaction != null && targetFaction != null)
        {
            return attackerFaction.FactionID != targetFaction.FactionID;
        }

        return false;
    }

    private void PerformAttack(GameObject attacker, GameObject target, CombatStats attackerStats)
    {
        CombatStats defenderStats = GetCombatStats(target);
        if (defenderStats == null) return;

        // Calculate damage
        float damage = CalculateDamage(attackerStats, defenderStats);
        
        // Apply damage
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            
            // Show hit effect
            ShowHitEffect(target.transform.position);
            
            // Check if target is destroyed
            if (damageable.IsDead())
            {
                HandleUnitDeath(target);
            }
        }
    }

    private float CalculateDamage(CombatStats attacker, CombatStats defender)
    {
        float baseDamage = attacker.baseAttack;
        float defense = defender.baseDefense;
        
        // Apply random variation (±10%)
        float variation = Random.Range(0.9f, 1.1f);
        
        // Calculate final damage
        float damage = (baseDamage - defense * 0.5f) * variation;
        return Mathf.Max(1f, damage); // Minimum 1 damage
    }

    private void ShowHitEffect(Vector3 position)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    private void HandleUnitDeath(GameObject unit)
    {
        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, unit.transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Remove from combat system
        if (unit.GetComponent<Unit>() != null)
        {
            unitCombatStats.Remove(unit.GetComponent<Unit>());
        }
        else if (unit.GetComponent<Building>() != null)
        {
            buildingCombatStats.Remove(unit.GetComponent<Building>());
        }

        // Notify game manager
        GameManager.Instance.OnUnitDestroyed(unit);

        // Destroy the unit
        Destroy(unit);
    }

    private CombatStats GetCombatStats(GameObject obj)
    {
        if (obj.TryGetComponent<Unit>(out Unit unit))
        {
            return unitCombatStats.GetValueOrDefault(unit);
        }
        else if (obj.TryGetComponent<Building>(out Building building))
        {
            return buildingCombatStats.GetValueOrDefault(building);
        }
        return null;
    }
}
