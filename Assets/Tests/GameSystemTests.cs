using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace Tests
{
    public class GameSystemTests
    {
        private GameManager gameManager;
        private DynamicEconomySystem economySystem;
        private CivilizationManager civilizationManager;
        private AIConsciousnessSystem aiSystem;
        private CulturalEvolutionSystem cultureSystem;

        [SetUp]
        public void Setup()
        {
            GameObject go = new GameObject();
            gameManager = go.AddComponent<GameManager>();
            economySystem = go.AddComponent<DynamicEconomySystem>();
            civilizationManager = go.AddComponent<CivilizationManager>();
            aiSystem = go.AddComponent<AIConsciousnessSystem>();
            cultureSystem = go.AddComponent<CulturalEvolutionSystem>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(gameManager.gameObject);
        }

        [Test]
        public void TestResourceManagement()
        {
            // Test initial resources
            var startingResources = economySystem.GetAllResources();
            Assert.IsNotNull(startingResources);
            Assert.IsTrue(startingResources.Count > 0);
        }

        [Test]
        public void TestCivilizationCreation()
        {
            var civ = civilizationManager.CreateNewCivilization("Test Civilization");
            Assert.IsNotNull(civ);
            Assert.AreEqual("Test Civilization", civ.Name);
        }

        [Test]
        public void TestAIBehavior()
        {
            var civ = civilizationManager.CreateNewCivilization("AI Test");
            var aiDecision = aiSystem.GetNextDecision(civ);
            Assert.IsNotNull(aiDecision);
        }

        [Test]
        public void TestCulturalEvolution()
        {
            var civ = civilizationManager.CreateNewCivilization("Culture Test");
            var culturalProgress = cultureSystem.CalculateCulturalProgress(civ);
            Assert.IsTrue(culturalProgress >= 0);
        }

        [UnityTest]
        public IEnumerator TestGameStatePersistence()
        {
            // Set some game state
            gameManager.SetGameState(GameManager.GameState.Playing);
            
            // Save game state
            yield return new WaitForSeconds(0.1f);
            gameManager.SaveGame();
            
            // Load game state
            yield return new WaitForSeconds(0.1f);
            gameManager.LoadGame();
            
            Assert.AreEqual(GameManager.GameState.Playing, gameManager.CurrentGameState);
        }
    }
}
