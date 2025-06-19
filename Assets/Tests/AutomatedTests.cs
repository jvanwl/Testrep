using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class AutomatedTests
{
    [UnityTest]
    public IEnumerator TestGameInitialization()
    {
        var gameObject = new GameObject();
        var gameManager = gameObject.AddComponent<GameManager>();
        
        yield return new WaitForSeconds(1f);
        
        Assert.IsNotNull(GameManager.Instance);
        Assert.AreEqual(gameManager, GameManager.Instance);
    }

    [Test]
    public void TestEconomySystem()
    {
        var gameObject = new GameObject();
        var economySystem = gameObject.AddComponent<DynamicEconomySystem>();
        
        economySystem.AddResource("gold", 100);
        Assert.AreEqual(100, economySystem.GetResource("gold"));
        
        economySystem.SpendResource("gold", 50);
        Assert.AreEqual(50, economySystem.GetResource("gold"));
    }

    [UnityTest]
    public IEnumerator TestCivilizationCreation()
    {
        var gameObject = new GameObject();
        var civilizationManager = gameObject.AddComponent<CivilizationManager>();
        
        yield return new WaitForSeconds(1f);
        
        var civ = civilizationManager.CreateCivilization("ancient_egypt");
        Assert.IsNotNull(civ);
        Assert.AreEqual("Ancient Egypt", civ.Name);
    }

    [Test]
    public void TestCultureSystem()
    {
        var gameObject = new GameObject();
        var cultureSystem = gameObject.AddComponent<AdvancedCultureSystem>();
        
        var traits = cultureSystem.GetAvailableTraits();
        Assert.IsNotNull(traits);
        Assert.IsTrue(traits.Count > 0);
    }

    [UnityTest]
    public IEnumerator TestBuildingConstruction()
    {
        var gameObject = new GameObject();
        var buildingSystem = gameObject.AddComponent<Building>();
        
        buildingSystem.StartConstruction("barracks");
        
        yield return new WaitForSeconds(buildingSystem.constructionTime);
        
        Assert.IsTrue(buildingSystem.IsConstructed);
    }

    [Test]
    public void TestSaveSystem()
    {
        var gameObject = new GameObject();
        var saveSystem = gameObject.AddComponent<SaveSystem>();
        
        var testData = new Dictionary<string, object>
        {
            { "gold", 100 },
            { "population", 50 }
        };
        
        Assert.IsTrue(saveSystem.SaveGame("test_save", testData));
        var loadedData = saveSystem.LoadGame("test_save");
        Assert.IsNotNull(loadedData);
        Assert.AreEqual(100, loadedData["gold"]);
    }

    [UnityTest]
    public IEnumerator TestPerformance()
    {
        var gameObject = new GameObject();
        var perfMonitor = gameObject.AddComponent<PerformanceMonitor>();
        
        yield return new WaitForSeconds(5f);
        
        var stats = perfMonitor.GetStats();
        Assert.IsTrue(stats.averageFPS > 0);
        Assert.IsTrue(stats.memoryUsage > 0);
    }
}
