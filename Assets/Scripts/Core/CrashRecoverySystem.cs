using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class CrashRecoverySystem : MonoBehaviour
{
    public static CrashRecoverySystem Instance { get; private set; }

    [Serializable]
    public class RecoveryConfig
    {
        public bool enableRecovery = true;
        public float recoveryCheckInterval = 60f;
        public int maxRecoveryAttempts = 3;
        public bool createBackups = true;
        public int maxBackups = 5;
        public string recoveryFilePath = "crash_recovery.json";
    }

    [SerializeField] private RecoveryConfig config = new RecoveryConfig();

    private float lastRecoveryCheck;
    private int recoveryAttempts;
    private bool isRecovering;
    private Dictionary<string, object> lastKnownState;
    private string backupDirectory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeRecoverySystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeRecoverySystem()
    {
        if (!config.enableRecovery) return;

        backupDirectory = Path.Combine(Application.persistentDataPath, "CrashRecovery");
        Directory.CreateDirectory(backupDirectory);

        Application.logMessageReceived += HandleLogMessage;
        AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

        CheckForCrashRecovery();

        GameLogger.Instance.Log(LogSeverity.Info, "Crash Recovery System Initialized");
    }

    private void Update()
    {
        if (!config.enableRecovery || isRecovering) return;

        if (Time.unscaledTime - lastRecoveryCheck >= config.recoveryCheckInterval)
        {
            SaveRecoveryPoint();
            lastRecoveryCheck = Time.unscaledTime;
        }
    }

    private async void SaveRecoveryPoint()
    {
        try
        {
            var gameState = await CollectGameState();
            string jsonState = JsonUtility.ToJson(gameState);
            
            if (config.createBackups)
            {
                CreateBackup(jsonState);
            }

            File.WriteAllText(GetRecoveryFilePath(), jsonState);
            lastKnownState = gameState;
        }
        catch (Exception ex)
        {
            GameLogger.Instance.Log(LogSeverity.Error, "Failed to save recovery point", ex);
        }
    }

    private void CreateBackup(string jsonState)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string backupPath = Path.Combine(backupDirectory, $"recovery_backup_{timestamp}.json");
        
        File.WriteAllText(backupPath, jsonState);
        CleanupOldBackups();
    }

    private void CleanupOldBackups()
    {
        var backupFiles = Directory.GetFiles(backupDirectory, "recovery_backup_*.json")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .Skip(config.maxBackups);

        foreach (var file in backupFiles)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                GameLogger.Instance.Log(LogSeverity.Warning, $"Failed to delete old backup: {file}", ex);
            }
        }
    }

    private async void CheckForCrashRecovery()
    {
        string recoveryPath = GetRecoveryFilePath();
        if (!File.Exists(recoveryPath)) return;

        try
        {
            string jsonState = File.ReadAllText(recoveryPath);
            var recoveryState = JsonUtility.FromJson<Dictionary<string, object>>(jsonState);

            if (await ValidateRecoveryState(recoveryState))
            {
                await AttemptRecovery(recoveryState);
            }
        }
        catch (Exception ex)
        {
            GameLogger.Instance.Log(LogSeverity.Error, "Failed to check for crash recovery", ex);
        }
    }

    private async Task<bool> ValidateRecoveryState(Dictionary<string, object> state)
    {
        if (state == null) return false;

        try
        {
            // Validate timestamp
            if (state.TryGetValue("timestamp", out object timestampObj))
            {
                DateTime timestamp = DateTime.Parse(timestampObj.ToString());
                TimeSpan age = DateTime.Now - timestamp;
                
                if (age.TotalHours > 24)
                {
                    GameLogger.Instance.Log(LogSeverity.Warning, "Recovery state is too old");
                    return false;
                }
            }

            // Validate version
            if (state.TryGetValue("version", out object versionObj))
            {
                string version = versionObj.ToString();
                if (version != Application.version)
                {
                    GameLogger.Instance.Log(LogSeverity.Warning, "Recovery state version mismatch");
                    return false;
                }
            }

            // Validate data integrity
            if (!state.ContainsKey("gameData"))
            {
                GameLogger.Instance.Log(LogSeverity.Warning, "Recovery state missing game data");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            GameLogger.Instance.Log(LogSeverity.Error, "Failed to validate recovery state", ex);
            return false;
        }
    }

    private async Task AttemptRecovery(Dictionary<string, object> state)
    {
        if (recoveryAttempts >= config.maxRecoveryAttempts)
        {
            GameLogger.Instance.Log(LogSeverity.Error, "Max recovery attempts reached");
            HandleRecoveryFailure();
            return;
        }

        isRecovering = true;
        recoveryAttempts++;

        try
        {
            GameLogger.Instance.Log(LogSeverity.Info, $"Attempting recovery (Attempt {recoveryAttempts}/{config.maxRecoveryAttempts})");

            // Apply recovery state
            await ApplyRecoveryState(state);

            // Verify recovery
            if (await VerifyRecovery())
            {
                HandleRecoverySuccess();
            }
            else
            {
                await AttemptRecovery(state);
            }
        }
        catch (Exception ex)
        {
            GameLogger.Instance.Log(LogSeverity.Error, "Recovery attempt failed", ex);
            await AttemptRecovery(state);
        }
        finally
        {
            isRecovering = false;
        }
    }

    private async Task ApplyRecoveryState(Dictionary<string, object> state)
    {
        // Apply game state components in the correct order
        await Task.WhenAll(
            ApplyPlayerState(state),
            ApplyWorldState(state),
            ApplyResourceState(state),
            ApplyUnitState(state)
        );
    }

    private async Task<bool> VerifyRecovery()
    {
        try
        {
            var currentState = await CollectGameState();
            return ValidateStateEquality(currentState, lastKnownState);
        }
        catch (Exception ex)
        {
            GameLogger.Instance.Log(LogSeverity.Error, "Recovery verification failed", ex);
            return false;
        }
    }

    private bool ValidateStateEquality(Dictionary<string, object> state1, Dictionary<string, object> state2)
    {
        if (state1 == null || state2 == null) return false;
        if (state1.Count != state2.Count) return false;

        foreach (var kvp in state1)
        {
            if (!state2.ContainsKey(kvp.Key)) return false;
            if (!AreValuesEqual(kvp.Value, state2[kvp.Key])) return false;
        }

        return true;
    }

    private bool AreValuesEqual(object val1, object val2)
    {
        if (val1 == null && val2 == null) return true;
        if (val1 == null || val2 == null) return false;

        if (val1 is Dictionary<string, object> dict1 && val2 is Dictionary<string, object> dict2)
        {
            return ValidateStateEquality(dict1, dict2);
        }

        return val1.Equals(val2);
    }

    private void HandleRecoverySuccess()
    {
        GameLogger.Instance.Log(LogSeverity.Info, "Recovery completed successfully");
        recoveryAttempts = 0;
        File.Delete(GetRecoveryFilePath());
    }

    private void HandleRecoveryFailure()
    {
        GameLogger.Instance.Log(LogSeverity.Error, "Recovery failed after maximum attempts");
        
        // Create error report
        CreateCrashReport();

        // Reset to default state
        ResetToDefaultState();
    }

    private void HandleLogMessage(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            SaveRecoveryPoint();
        }
    }

    private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        var exception = args.ExceptionObject as Exception;
        GameLogger.Instance.Log(LogSeverity.Critical, "Unhandled exception occurred", exception);
        CreateCrashReport(exception);
    }

    private void CreateCrashReport(Exception exception = null)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string crashReportPath = Path.Combine(backupDirectory, $"crash_report_{timestamp}.txt");

            using (var writer = new StreamWriter(crashReportPath))
            {
                writer.WriteLine("=== CRASH REPORT ===");
                writer.WriteLine($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Version: {Application.version}");
                writer.WriteLine($"Platform: {Application.platform}");
                writer.WriteLine($"Unity Version: {Application.unityVersion}");
                writer.WriteLine($"Recovery Attempts: {recoveryAttempts}");

                if (exception != null)
                {
                    writer.WriteLine("\n=== EXCEPTION DETAILS ===");
                    writer.WriteLine($"Type: {exception.GetType().FullName}");
                    writer.WriteLine($"Message: {exception.Message}");
                    writer.WriteLine($"Stack Trace: {exception.StackTrace}");
                }

                writer.WriteLine("\n=== SYSTEM INFO ===");
                writer.WriteLine($"OS: {SystemInfo.operatingSystem}");
                writer.WriteLine($"Device: {SystemInfo.deviceModel}");
                writer.WriteLine($"Memory: {SystemInfo.systemMemorySize}MB");
                writer.WriteLine($"Processors: {SystemInfo.processorCount}");
                writer.WriteLine($"Graphics: {SystemInfo.graphicsDeviceName}");
            }
        }
        catch (Exception ex)
        {
            GameLogger.Instance.Log(LogSeverity.Error, "Failed to create crash report", ex);
        }
    }

    private string GetRecoveryFilePath()
    {
        return Path.Combine(Application.persistentDataPath, config.recoveryFilePath);
    }

    private async Task<Dictionary<string, object>> CollectGameState()
    {
        return new Dictionary<string, object>
        {
            ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            ["version"] = Application.version,
            ["gameData"] = await CollectGameData()
        };
    }

    private async Task<Dictionary<string, object>> CollectGameData()
    {
        // Implementation to collect game data
        return new Dictionary<string, object>();
    }

    private async Task ApplyPlayerState(Dictionary<string, object> state)
    {
        // Implementation to apply player state
    }

    private async Task ApplyWorldState(Dictionary<string, object> state)
    {
        // Implementation to apply world state
    }

    private async Task ApplyResourceState(Dictionary<string, object> state)
    {
        // Implementation to apply resource state
    }

    private async Task ApplyUnitState(Dictionary<string, object> state)
    {
        // Implementation to apply unit state
    }

    private void ResetToDefaultState()
    {
        // Implementation to reset game to default state
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLogMessage;
        AppDomain.CurrentDomain.UnhandledException -= HandleUnhandledException;
    }
}
