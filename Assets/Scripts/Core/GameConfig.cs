using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject
{
    [Header("Game Settings")]
    public float baseResourceRate = 1.0f;
    public int startingGold = 100;
    public int maxPopulation = 1000;

    [Header("Civilization Settings")]
    public float culturalSpreadRate = 0.5f;
    public float techProgressRate = 1.0f;

    [Header("Economy Settings")]
    public float inflationRate = 0.01f;
    public float tradeMultiplier = 1.2f;
}