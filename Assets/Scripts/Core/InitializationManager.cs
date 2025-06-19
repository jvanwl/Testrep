using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class InitializationManager : MonoBehaviour
{
    public static InitializationManager Instance { get; private set; }

    [System.Serializable]
    public class InitializationConfig
    {
        public bool showDebugLogs = true;
        public float timeoutSeconds = 30f;
        public bool failOnTimeout = true;
    }

    [SerializeField] private InitializationConfig config = new InitializationConfig();

    private Dictionary<string, bool> systemStatus = new Dictionary<string, bool>();
    private List<string> initializationOrder = new List<string>
    {
        "GameLogger",
        "PerformanceMonitor",
        "CrashRecoverySystem",
        "SaveSystem",
        "GameManager",
        "EconomySystem",
        "DiplomacySystem",
        "CultureSystem",
        "TechnologySystem"
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void InitializeSystems()
    {
        foreach (var system in initializationOrder)
        {
            systemStatus[system] = false;
        }

        var initTask = InitializeAllSystems();
        var timeoutTask = Task.Delay((int)(config.timeoutSeconds * 1000));

        var completedTask = await Task.WhenAny(initTask, timeoutTask);
        
        if (completedTask == timeoutTask)
        {
            HandleInitializationTimeout();
        }
        else if (completedTask == initTask)
        {
            HandleInitializationComplete();
        }
    }

    private async Task InitializeAllSystems()
    {
        foreach (var system in initializationOrder)
        {
            await InitializeSystem(system);
        }
    }

    private async Task InitializeSystem(string systemName)
    {
        if (config.showDebugLogs)
        {
            Debug.Log($"Initializing {systemName}...");
        }

        bool success = false;

        try
        {
            switch (systemName)
            {
                case "GameLogger":
                    success = await InitializeGameLogger();
                    break;
                case "PerformanceMonitor":
                    success = await InitializePerformanceMonitor();
                    break;
                case "CrashRecoverySystem":
                    success = await InitializeCrashRecovery();
                    break;
                case "SaveSystem":
                    success = await InitializeSaveSystem();
                    break;
                // Add other systems here
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize {systemName}: {e.Message}");
            success = false;
        }

        systemStatus[systemName] = success;

        if (config.showDebugLogs)
        {
            Debug.Log($"{systemName} initialization {(success ? "succeeded" : "failed")}");
        }
    }

    private void HandleInitializationTimeout()
    {
        string failedSystems = GetFailedSystemsString();
        string errorMessage = $"System initialization timed out after {config.timeoutSeconds} seconds. Failed systems: {failedSystems}";
        
        Debug.LogError(errorMessage);

        if (config.failOnTimeout)
        {
            // Handle critical failure
            Application.Quit();
        }
    }

    private void HandleInitializationComplete()
    {
        if (AreAllSystemsInitialized())
        {
            Debug.Log("All systems initialized successfully");
        }
        else
        {
            string failedSystems = GetFailedSystemsString();
            Debug.LogError($"Initialization completed with errors. Failed systems: {failedSystems}");
        }
    }

    private string GetFailedSystemsString()
    {
        List<string> failedSystems = new List<string>();
        foreach (var kvp in systemStatus)
        {
            if (!kvp.Value)
            {
                failedSystems.Add(kvp.Key);
            }
        }
        return string.Join(", ", failedSystems);
    }

    private bool AreAllSystemsInitialized()
    {
        foreach (var status in systemStatus.Values)
        {
            if (!status) return false;
        }
        return true;
    }

    // System-specific initialization methods
    private async Task<bool> InitializeGameLogger()
    {
        if (GameLogger.Instance != null)
        {
            return true;
        }
        return false;
    }

    private async Task<bool> InitializePerformanceMonitor()
    {
        if (PerformanceMonitor.Instance != null)
        {
            return true;
        }
        return false;
    }

    private async Task<bool> InitializeCrashRecovery()
    {
        if (CrashRecoverySystem.Instance != null)
        {
            return true;
        }
        return false;
    }

    private async Task<bool> InitializeSaveSystem()
    {
        if (SaveSystem.Instance != null)
        {
            return true;
        }
        return false;
    }
}
