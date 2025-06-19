using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

public class PerformanceMonitor : MonoBehaviour
{
    public static PerformanceMonitor Instance { get; private set; }

    [Serializable]
    public class PerformanceConfig
    {
        public bool enableMonitoring = true;
        public float updateInterval = 1f;
        public int frameHistorySize = 300;
        public float warningThresholdMs = 16.67f; // 60 FPS
        public float criticalThresholdMs = 33.33f; // 30 FPS
        public bool logToFile = true;
        public string logFilePath = "performance_log.csv";
    }

    [SerializeField] private PerformanceConfig config = new PerformanceConfig();

    private class MetricData
    {
        public string name;
        public Queue<float> history = new Queue<float>();
        public float currentValue;
        public float minValue = float.MaxValue;
        public float maxValue = float.MinValue;
        public float averageValue;
        public float warningThreshold;
        public float criticalThreshold;
        public int warningCount;
        public int criticalCount;
    }

    private Dictionary<string, MetricData> metrics = new Dictionary<string, MetricData>();
    private Dictionary<string, Stopwatch> activeStopwatches = new Dictionary<string, Stopwatch>();
    private float lastUpdateTime;
    private int frameCount;
    private float accumulatedTime;
    private System.IO.StreamWriter logWriter;

    private readonly string[] defaultMetrics = new[]
    {
        "FrameTime",
        "UpdateTime",
        "RenderTime",
        "PhysicsTime",
        "AITime",
        "PathfindingTime",
        "UITime",
        "GCMemory"
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMonitor();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMonitor()
    {
        if (!config.enableMonitoring) return;

        // Initialize default metrics
        foreach (var metricName in defaultMetrics)
        {
            AddMetric(metricName);
        }

        // Initialize logging
        if (config.logToFile)
        {
            InitializeLogging();
        }

        // Register to Unity's profiler if available
        #if UNITY_EDITOR
        UnityEditor.Profiling.ProfilerDriver.profileEditor = true;
        #endif

        GameLogger.Instance.Log(LogSeverity.Info, "Performance Monitor Initialized");
    }

    private void InitializeLogging()
    {
        try
        {
            logWriter = new System.IO.StreamWriter(config.logFilePath, true);
            string header = $"Timestamp,{string.Join(",", metrics.Keys)}\n";
            logWriter.Write(header);
        }
        catch (Exception ex)
        {
            GameLogger.Instance.Log(LogSeverity.Error, "Failed to initialize performance logging", ex);
            config.logToFile = false;
        }
    }

    private void Update()
    {
        if (!config.enableMonitoring) return;

        frameCount++;
        accumulatedTime += Time.unscaledDeltaTime;

        // Update basic metrics
        UpdateMetric("FrameTime", Time.unscaledDeltaTime * 1000f);
        UpdateMetric("GCMemory", (float)GC.GetTotalMemory(false) / (1024 * 1024));

        // Check for interval update
        if (Time.unscaledTime - lastUpdateTime >= config.updateInterval)
        {
            UpdatePerformanceData();
            lastUpdateTime = Time.unscaledTime;
        }
    }

    private void UpdatePerformanceData()
    {
        float fps = frameCount / accumulatedTime;
        frameCount = 0;
        accumulatedTime = 0f;

        foreach (var metric in metrics.Values)
        {
            UpdateMetricStats(metric);
        }

        if (config.logToFile)
        {
            LogPerformanceData();
        }

        CheckPerformanceWarnings();
    }

    private void UpdateMetricStats(MetricData metric)
    {
        if (metric.history.Count == 0) return;

        metric.averageValue = metric.history.Average();
        metric.minValue = Mathf.Min(metric.minValue, metric.history.Min());
        metric.maxValue = Mathf.Max(metric.maxValue, metric.history.Max());

        // Check thresholds
        if (metric.currentValue >= metric.criticalThreshold)
        {
            metric.criticalCount++;
        }
        else if (metric.currentValue >= metric.warningThreshold)
        {
            metric.warningCount++;
        }
    }

    private void LogPerformanceData()
    {
        if (logWriter == null) return;

        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string values = string.Join(",", metrics.Values.Select(m => m.currentValue.ToString("F2")));
            logWriter.WriteLine($"{timestamp},{values}");
            logWriter.Flush();
        }
        catch (Exception ex)
        {
            GameLogger.Instance.Log(LogSeverity.Error, "Failed to log performance data", ex);
        }
    }

    private void CheckPerformanceWarnings()
    {
        foreach (var metric in metrics.Values)
        {
            if (metric.currentValue >= metric.criticalThreshold)
            {
                GameLogger.Instance.Log(LogSeverity.Warning, 
                    $"Critical performance in {metric.name}: {metric.currentValue:F2} (Threshold: {metric.criticalThreshold:F2})");
            }
            else if (metric.currentValue >= metric.warningThreshold)
            {
                GameLogger.Instance.Log(LogSeverity.Info,
                    $"Performance warning in {metric.name}: {metric.currentValue:F2} (Threshold: {metric.warningThreshold:F2})");
            }
        }
    }

    public void BeginSample(string name)
    {
        if (!config.enableMonitoring) return;

        if (!metrics.ContainsKey(name))
        {
            AddMetric(name);
        }

        if (!activeStopwatches.ContainsKey(name))
        {
            activeStopwatches[name] = Stopwatch.StartNew();
        }
        else
        {
            activeStopwatches[name].Restart();
        }
    }

    public void EndSample(string name)
    {
        if (!config.enableMonitoring) return;

        if (activeStopwatches.TryGetValue(name, out Stopwatch sw))
        {
            sw.Stop();
            float milliseconds = sw.ElapsedTicks / (float)Stopwatch.Frequency * 1000f;
            UpdateMetric(name, milliseconds);
        }
    }

    private void AddMetric(string name)
    {
        metrics[name] = new MetricData
        {
            name = name,
            warningThreshold = config.warningThresholdMs,
            criticalThreshold = config.criticalThresholdMs
        };
    }

    private void UpdateMetric(string name, float value)
    {
        if (!metrics.TryGetValue(name, out MetricData metric)) return;

        metric.currentValue = value;
        metric.history.Enqueue(value);

        while (metric.history.Count > config.frameHistorySize)
        {
            metric.history.Dequeue();
        }
    }

    public PerformanceStats GetStats(string metricName)
    {
        if (!metrics.TryGetValue(metricName, out MetricData metric))
        {
            return null;
        }

        return new PerformanceStats
        {
            currentValue = metric.currentValue,
            averageValue = metric.averageValue,
            minValue = metric.minValue,
            maxValue = metric.maxValue,
            warningCount = metric.warningCount,
            criticalCount = metric.criticalCount
        };
    }

    public struct PerformanceStats
    {
        public float currentValue;
        public float averageValue;
        public float minValue;
        public float maxValue;
        public int warningCount;
        public int criticalCount;
    }

    private void OnDestroy()
    {
        logWriter?.Dispose();
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (config.frameHistorySize < 10)
        {
            config.frameHistorySize = 10;
        }
        if (config.updateInterval < 0.1f)
        {
            config.updateInterval = 0.1f;
        }
    }
    #endif
}
