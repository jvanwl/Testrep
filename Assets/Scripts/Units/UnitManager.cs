using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private GameObject soldierPrefab;
    
    private Dictionary<string, GameObject> unitPrefabs;
    private List<Unit> selectedUnits = new List<Unit>();

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

        InitializePrefabs();
    }

    private void InitializePrefabs()
    {
        unitPrefabs = new Dictionary<string, GameObject>
        {
            { "worker", workerPrefab },
            { "soldier", soldierPrefab }
        };
    }

    public void CreateUnit(string unitType)
    {
        if (!unitPrefabs.ContainsKey(unitType))
        {
            Debug.LogError($"Unit type {unitType} not found!");
            return;
        }

        // Check resources
        if (!ResourceManager.Instance.CanAffordUnit(unitType))
        {
            Debug.Log("Not enough resources to create unit!");
            return;
        }

        // Deduct resources
        ResourceManager.Instance.SpendUnitCost(unitType);

        // Find spawn point (nearest rally point)
        Vector3 spawnPoint = FindSpawnPoint();

        // Create the unit
        GameObject unitObj = Instantiate(unitPrefabs[unitType], spawnPoint, Quaternion.identity);
        Unit unit = unitObj.GetComponent<Unit>();
        unit.Initialize(unitType);
    }

    public void SelectUnit(Unit unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
            unit.Select();
        }
    }

    public void DeselectUnit(Unit unit)
    {
        if (selectedUnits.Contains(unit))
        {
            selectedUnits.Remove(unit);
            unit.Deselect();
        }
    }

    public void DeselectAll()
    {
        foreach (var unit in selectedUnits)
        {
            unit.Deselect();
        }
        selectedUnits.Clear();
    }

    public void MoveSelectedUnits(Vector3 target)
    {
        if (selectedUnits.Count == 0) return;

        // Calculate formation positions
        Vector3[] formationPositions = CalculateFormationPositions(target, selectedUnits.Count);

        // Assign positions to units
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            selectedUnits[i].MoveTo(formationPositions[i]);
        }
    }

    private Vector3[] CalculateFormationPositions(Vector3 center, int unitCount)
    {
        Vector3[] positions = new Vector3[unitCount];
        float spacing = 2f; // Space between units
        int unitsPerRow = Mathf.CeilToInt(Mathf.Sqrt(unitCount));

        for (int i = 0; i < unitCount; i++)
        {
            int row = i / unitsPerRow;
            int col = i % unitsPerRow;
            float xOffset = (col - unitsPerRow/2f) * spacing;
            float zOffset = row * spacing;

            positions[i] = center + new Vector3(xOffset, 0, zOffset);
        }

        return positions;
    }

    private Vector3 FindSpawnPoint()
    {
        // Find nearest rally point or default to a safe position
        return new Vector3(0, 0, 0); // Replace with actual logic
    }
}
