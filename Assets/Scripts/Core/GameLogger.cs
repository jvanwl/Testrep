using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GameLogger : MonoBehaviour
{
    public static GameLogger Instance { get; private set; }

    [Serializable]
    public class LogConfig
    {
        public bool logToFile = true;
        public bool logToConsole = true;
        public bool includeStackTrace = true;
        public bool includeTimestamp = true;
        public string logFilePath = "game_log.txt";
        public LogSeverity minimumSeverity = LogSeverity.Info;
    }

    public enum LogSeverity
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    [SerializeField] private LogConfig config = new LogConfig();
    
    private Dictionary<LogSeverity, string> severityColors = new Dictionary<LogSeverity, string>
    {
        { LogSeverity.Debug, "gray" },
        { LogSeverity.Info, "white" },
        { LogSeverity.Warning, "yellow" },
        { LogSeverity.Error, "red" },
        { LogSeverity.Critical, "magenta" }
    };

    private Queue<string> logBuffer = new Queue<string>();
    private const int MAX_BUFFER_SIZE = 1000;
    private StringBuilder messageBuilder = new StringBuilder();
    private System.Timers.Timer flushTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLogger();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeLogger()
    {
        Application.logMessageReceived += HandleUnityLog;
        
        flushTimer = new System.Timers.Timer(5000); // Flush every 5 seconds
        flushTimer.Elapsed += (s, e) => FlushLogBuffer();
        flushTimer.Start();

        Log(LogSeverity.Info, "Game Logger Initialized");
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleUnityLog;
        FlushLogBuffer();
        flushTimer?.Dispose();
    }

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        LogSeverity severity = ConvertUnityLogTypeToSeverity(type);
        if (severity >= config.minimumSeverity)
        {
            LogInternal(severity, logString, stackTrace);
        }
    }

    private LogSeverity ConvertUnityLogTypeToSeverity(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                return LogSeverity.Error;
            case LogType.Assert:
                return LogSeverity.Critical;
            case LogType.Warning:
                return LogSeverity.Warning;
            case LogType.Log:
                return LogSeverity.Info;
            default:
                return LogSeverity.Debug;
        }
    }

    public void Log(LogSeverity severity, string message, Exception exception = null)
    {
        if (severity < config.minimumSeverity) return;

        messageBuilder.Clear();

        if (config.includeTimestamp)
        {
            messageBuilder.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ");
        }

        messageBuilder.Append($"[{severity}] {message}");

        if (exception != null)
        {
            messageBuilder.Append($"\nException: {exception.Message}");
            if (config.includeStackTrace)
            {
                messageBuilder.Append($"\nStack Trace: {exception.StackTrace}");
            }
        }

        string finalMessage = messageBuilder.ToString();

        if (config.logToConsole)
        {
            string coloredMessage = $"<color={severityColors[severity]}>{finalMessage}</color>";
            Debug.Log(coloredMessage);
        }

        if (config.logToFile)
        {
            logBuffer.Enqueue(finalMessage);
            if (logBuffer.Count >= MAX_BUFFER_SIZE)
            {
                FlushLogBuffer();
            }
        }

        // Handle critical errors immediately
        if (severity == LogSeverity.Critical)
        {
            FlushLogBuffer();
            HandleCriticalError(message, exception);
        }
    }

    public void LogGameplayEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        messageBuilder.Clear();
        messageBuilder.Append($"[GameplayEvent] {eventName}");

        if (parameters != null && parameters.Count > 0)
        {
            messageBuilder.Append(" - Parameters: {");
            foreach (var param in parameters)
            {
                messageBuilder.Append($"{param.Key}:{param.Value}, ");
            }
            messageBuilder.Length -= 2; // Remove last comma and space
            messageBuilder.Append("}");
        }

        Log(LogSeverity.Info, messageBuilder.ToString());
    }

    public void LogPerformanceMetric(string metricName, float value, string unit = "ms")
    {
        Log(LogSeverity.Debug, $"[Performance] {metricName}: {value}{unit}");
    }

    private void FlushLogBuffer()
    {
        if (!config.logToFile || logBuffer.Count == 0) return;

        try
        {
            string[] lines = logBuffer.ToArray();
            System.IO.File.AppendAllLines(config.logFilePath, lines);
            logBuffer.Clear();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to flush log buffer: {ex.Message}");
        }
    }

    private void HandleCriticalError(string message, Exception exception)
    {
        // Create error report
        StringBuilder errorReport = new StringBuilder();
        errorReport.AppendLine("=== CRITICAL ERROR REPORT ===");
        errorReport.AppendLine($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        errorReport.AppendLine($"Message: {message}");
        
        if (exception != null)
        {
            errorReport.AppendLine($"Exception: {exception.GetType().Name}");
            errorReport.AppendLine($"Message: {exception.Message}");
            errorReport.AppendLine($"Stack Trace: {exception.StackTrace}");
        }

        // Add system info
        errorReport.AppendLine("\n=== SYSTEM INFORMATION ===");
        errorReport.AppendLine($"Operating System: {SystemInfo.operatingSystem}");
        errorReport.AppendLine($"Device Model: {SystemInfo.deviceModel}");
        errorReport.AppendLine($"GPU: {SystemInfo.graphicsDeviceName}");
        errorReport.AppendLine($"Memory: {SystemInfo.systemMemorySize}MB");

        // Add game state
        errorReport.AppendLine("\n=== GAME STATE ===");
        if (GameManager.Instance != null)
        {
            // Add relevant game state information
            errorReport.AppendLine($"Active Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            // Add more game state info as needed
        }

        // Save error report
        string errorFilePath = $"error_report_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
        try
        {
            System.IO.File.WriteAllText(errorFilePath, errorReport.ToString());
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save error report: {ex.Message}");
        }
    }

    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public void LogDebug(string message)
    {
        Log(LogSeverity.Debug, message);
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        // Ensure critical errors are always logged
        if (config.minimumSeverity > LogSeverity.Critical)
        {
            config.minimumSeverity = LogSeverity.Critical;
        }
    }
    #endif
}
