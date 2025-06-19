using UnityEngine;
using System.Collections;

public class Barracks : Building
{
    [SerializeField] private GameObject[] unitPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float trainingSpeedMultiplier = 1f;
    
    private Queue trainingQueue = new Queue();
    private bool isTraining;

    protected override void Start()
    {
        base.Start();
        Type = BuildingType.Barracks;
    }

    protected override void Update()
    {
        base.Update();
        
        if (!isConstructing && !isTraining && trainingQueue.Count > 0)
        {
            StartTraining();
        }
    }

    public void QueueUnit(int unitIndex)
    {
        if (unitIndex >= 0 && unitIndex < unitPrefabs.Length)
        {
            GameObject unitPrefab = unitPrefabs[unitIndex];
            Unit unit = unitPrefab.GetComponent<Unit>();
            
            if (unit != null && GameManager.Instance.SpendGold(unit.GoldCost))
            {
                trainingQueue.Enqueue(unitPrefab);
            }
        }
    }

    private void StartTraining()
    {
        if (trainingQueue.Count > 0)
        {
            isTraining = true;
            GameObject unitToTrain = (GameObject)trainingQueue.Peek();
            StartCoroutine(TrainUnit(unitToTrain));
        }
    }

    private IEnumerator TrainUnit(GameObject unitPrefab)
    {
        Unit unit = unitPrefab.GetComponent<Unit>();
        float trainingTime = unit.TrainingTime / (trainingSpeedMultiplier * level);
        
        // Show training progress
        float progress = 0;
        while (progress < trainingTime)
        {
            progress += Time.deltaTime;
            // Update UI progress bar if needed
            yield return null;
        }

        // Spawn the unit
        if (spawnPoint != null)
        {
            Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
        }
        
        trainingQueue.Dequeue();
        isTraining = false;
    }

    protected override int GetMaxLevel()
    {
        return 5; // Barracks can be upgraded more times
    }

    protected override void CompleteConstruction()
    {
        base.CompleteConstruction();
        trainingSpeedMultiplier = 1f + (level - 1) * 0.2f; // 20% faster per level
    }
}
