using UnityEngine;
using System;

public abstract class Building : MonoBehaviour
{
    [Header("Building Stats")]
    [SerializeField] protected int maxHealth = 500;
    [SerializeField] protected int buildCost = 200;
    [SerializeField] protected float buildTime = 5f;
    
    [Header("Current State")]
    protected int currentHealth;
    protected int level = 1;
    protected bool isConstructing;
    protected float constructionProgress;

    public BuildingType Type { get; protected set; }
    public bool IsDestroyed => currentHealth <= 0;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public int Level => level;
    public bool IsEnemy { get; protected set; }

    public event Action<Building> OnBuildingDestroyed;
    public event Action<Building, int> OnBuildingDamaged;
    public event Action<Building> OnConstructionComplete;
    public event Action<Building> OnUpgradeComplete;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        GameManager.Instance.RegisterBuilding(this);
        
        if (!IsEnemy)
        {
            StartConstruction();
        }
    }

    protected virtual void Update()
    {
        if (isConstructing)
        {
            UpdateConstruction();
        }
    }

    protected virtual void StartConstruction()
    {
        isConstructing = true;
        constructionProgress = 0f;
        // Trigger construction animation/effects
    }

    protected virtual void UpdateConstruction()
    {
        constructionProgress += Time.deltaTime;
        
        if (constructionProgress >= buildTime)
        {
            CompleteConstruction();
        }
    }

    protected virtual void CompleteConstruction()
    {
        isConstructing = false;
        OnConstructionComplete?.Invoke(this);
        // Enable building functionality
    }

    public virtual bool CanUpgrade()
    {
        int upgradeCost = GetUpgradeCost();
        return GameManager.Instance.Gold >= upgradeCost && level < GetMaxLevel();
    }

    public virtual void Upgrade()
    {
        if (!CanUpgrade()) return;

        int upgradeCost = GetUpgradeCost();
        if (GameManager.Instance.SpendGold(upgradeCost))
        {
            level++;
            maxHealth += maxHealth / 2;
            currentHealth = maxHealth;
            OnUpgradeComplete?.Invoke(this);
            // Trigger upgrade effects/animation
        }
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (IsDestroyed) return;

        currentHealth -= damageAmount;
        OnBuildingDamaged?.Invoke(this, damageAmount);

        if (IsDestroyed)
        {
            Destroy();
        }
    }

    protected virtual void Destroy()
    {
        OnBuildingDestroyed?.Invoke(this);
        GameManager.Instance.UnregisterBuilding(this);
        
        // Trigger destruction animation/effects
        Destroy(gameObject, 1f);
    }

    protected virtual int GetUpgradeCost()
    {
        return buildCost * level;
    }

    protected virtual int GetMaxLevel()
    {
        return 3; // Default max level, override in specific buildings
    }

    protected virtual void OnDestroy()
    {
        GameManager.Instance?.UnregisterBuilding(this);
    }
}

public enum BuildingType
{
    Barracks,
    Turret,
    ResourceGenerator,
    ResearchCenter,
    Wall
}
