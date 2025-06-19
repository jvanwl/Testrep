using UnityEngine;
using System.Collections.Generic;

public class CivilizationSystem : MonoBehaviour
{
    private Dictionary<string, CivilizationData> activeCivilizations;
    private AIDirector aiDirector;

    void Start()
    {
        activeCivilizations = new Dictionary<string, CivilizationData>();
        aiDirector = gameObject.AddComponent<AIDirector>();
        InitializeCivilizations();
    }

    private void InitializeCivilizations()
    {
        // Initialize starting civilizations
        CreateCivilization("Rome", Vector2.zero);
        CreateCivilization("Greece", new Vector2(100, 0));
    }

    public void CreateCivilization(string name, Vector2 position)
    {
        var newCiv = new CivilizationData(name, position);
        activeCivilizations.Add(name, newCiv);
        aiDirector.AssignAI(newCiv);
    }
}