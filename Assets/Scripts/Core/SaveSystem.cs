using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [Serializable]
    public class SaveConfig
    {
        public bool useEncryption = true;
        public bool useCompression = true;
        public bool autoSave = true;
        public float autoSaveInterval = 300f; // 5 minutes
        public int maxAutoSaves = 5;
        public int maxManualSaves = 10;
        public string saveFileExtension = ".sav";
    }

    [SerializeField] private SaveConfig config = new SaveConfig();
    
    private string saveDirectory;
    private string backupDirectory;
    private float lastAutoSaveTime;
    private Dictionary<string, object> cachedData = new Dictionary<string, object>();
    private byte[] encryptionKey;
    private bool isSaving;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSaveSystem()
    {
        saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
        backupDirectory = Path.Combine(Application.persistentDataPath, "Backups");
        
        Directory.CreateDirectory(saveDirectory);
        Directory.CreateDirectory(backupDirectory);

        GenerateEncryptionKey();
        LoadCachedData();

        GameLogger.Instance.Log(LogSeverity.Info, "Save System Initialized");
    }

    private void Update()
    {
        if (config.autoSave && Time.time - lastAutoSaveTime >= config.autoSaveInterval)
        {
            AutoSave();
        }
    }

    private void GenerateEncryptionKey()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        string salt = "CivilizationGame_" + Application.version;
        
        using (var deriveBytes = new Rfc2898DeriveBytes(deviceId, Encoding.UTF8.GetBytes(salt), 1000))
        {
            encryptionKey = deriveBytes.GetBytes(32); // 256 bits
        }
    }

    public async Task<bool> SaveGame(string saveName, bool isAutoSave = false)
    {
        if (isSaving)
        {
            GameLogger.Instance.Log(LogSeverity.Warning, "Save operation already in progress");
            return false;
        }

        isSaving = true;
        string fileName = saveName + config.saveFileExtension;
        string filePath = Path.Combine(saveDirectory, fileName);
        string backupPath = Path.Combine(backupDirectory, fileName + ".backup");

        try
        {
            // Collect save data
            var saveData = new Dictionary<string, object>
            {
                ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["version"] = Application.version,
                ["gameData"] = await CollectGameData()
            };

            // Serialize
            string jsonData = JsonConvert.SerializeObject(saveData, Formatting.None,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            // Compress if enabled
            byte[] saveBytes = Encoding.UTF8.GetBytes(jsonData);
            if (config.useCompression)
            {
                saveBytes = await CompressData(saveBytes);
            }

            // Encrypt if enabled
            if (config.useEncryption)
            {
                saveBytes = EncryptData(saveBytes);
            }

            // Backup existing save if it exists
            if (File.Exists(filePath))
            {
                File.Copy(filePath, backupPath, true);
            }

            // Write new save
            await File.WriteAllBytesAsync(filePath, saveBytes);

            // Manage save files
            if (isAutoSave)
            {
                CleanupAutoSaves();
            }
            else
            {
                CleanupManualSaves();
            }

            GameLogger.Instance.Log(LogSeverity.Info, $"Game saved successfully: {saveName}");
            return true;
        }
        catch (Exception ex)
        {
            GameLogger.Instance.Log(LogSeverity.Error, $"Failed to save game: {saveName}", ex);
            
            // Restore backup if available
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, filePath, true);
                GameLogger.Instance.Log(LogSeverity.Info, "Restored save file from backup");
            }
            return false;
        }
        finally
        {
            isSaving = false;
        }
    }

    public async Task<bool> LoadGame(string saveName)
    {
        string filePath = Path.Combine(saveDirectory, saveName + config.saveFileExtension);

        if (!File.Exists(filePath))
        {
            GameLogger.Instance.Log(LogSeverity.Error, $"Save file not found: {saveName}");
            return false;
        }

        try
        {
            byte[] saveBytes = await File.ReadAllBytesAsync(filePath);

            // Decrypt if enabled
            if (config.useEncryption)
            {
                saveBytes = DecryptData(saveBytes);
            }

            // Decompress if enabled
            if (config.useCompression)
            {
                saveBytes = await DecompressData(saveBytes);
            }

            string jsonData = Encoding.UTF8.GetString(saveBytes);
            var saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            // Verify version compatibility
            string savedVersion = saveData["version"].ToString();
            if (!IsVersionCompatible(savedVersion))
            {
                GameLogger.Instance.Log(LogSeverity.Warning, $"Save file version mismatch. Current: {Application.version}, Save: {savedVersion}");
                return false;
            }

            // Apply save data
            await ApplyGameData(saveData["gameData"]);

            GameLogger.Instance.Log(LogSeverity.Info, $"Game loaded successfully: {saveName}");
            return true;
        }
        catch (Exception ex)
        {
            GameLogger.Instance.Log(LogSeverity.Error, $"Failed to load game: {saveName}", ex);
            return false;
        }
    }

    private async Task<Dictionary<string, object>> CollectGameData()
    {
        var gameData = new Dictionary<string, object>();

        // Collect data from various game systems
        gameData["player"] = await CollectPlayerData();
        gameData["world"] = await CollectWorldData();
        gameData["economy"] = await CollectEconomyData();
        gameData["diplomacy"] = await CollectDiplomacyData();
        gameData["technology"] = await CollectTechnologyData();
        gameData["culture"] = await CollectCultureData();

        return gameData;
    }

    private async Task ApplyGameData(object gameData)
    {
        var data = gameData as Dictionary<string, object>;
        if (data == null) return;

        // Apply data to various game systems
        await Task.WhenAll(
            ApplyPlayerData(data["player"]),
            ApplyWorldData(data["world"]),
            ApplyEconomyData(data["economy"]),
            ApplyDiplomacyData(data["diplomacy"]),
            ApplyTechnologyData(data["technology"]),
            ApplyCultureData(data["culture"])
        );
    }

    private void AutoSave()
    {
        string autoSaveName = $"AutoSave_{DateTime.Now:yyyyMMdd_HHmmss}";
        _ = SaveGame(autoSaveName, true);
        lastAutoSaveTime = Time.time;
    }

    private void CleanupAutoSaves()
    {
        CleanupSaveFiles("AutoSave_", config.maxAutoSaves);
    }

    private void CleanupManualSaves()
    {
        CleanupSaveFiles("", config.maxManualSaves, true);
    }

    private void CleanupSaveFiles(string prefix, int maxFiles, bool excludeAutoSaves = false)
    {
        var files = Directory.GetFiles(saveDirectory, $"{prefix}*{config.saveFileExtension}")
            .OrderByDescending(f => File.GetLastWriteTime(f));

        if (excludeAutoSaves)
        {
            files = files.Where(f => !Path.GetFileName(f).StartsWith("AutoSave_"));
        }

        foreach (var file in files.Skip(maxFiles))
        {
            try
            {
                File.Delete(file);
                string backupFile = Path.Combine(backupDirectory, Path.GetFileName(file) + ".backup");
                if (File.Exists(backupFile))
                {
                    File.Delete(backupFile);
                }
            }
            catch (Exception ex)
            {
                GameLogger.Instance.Log(LogSeverity.Warning, $"Failed to delete old save file: {file}", ex);
            }
        }
    }

    private bool IsVersionCompatible(string savedVersion)
    {
        // Implement version compatibility check logic
        return true; // Placeholder
    }

    private byte[] EncryptData(byte[] data)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = encryptionKey;
            aes.GenerateIV();

            using (MemoryStream ms = new MemoryStream())
            {
                // Write IV to the beginning of the stream
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }
    }

    private byte[] DecryptData(byte[] data)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = encryptionKey;

            // Read IV from the beginning of the data
            byte[] iv = new byte[aes.IV.Length];
            Array.Copy(data, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, iv.Length, data.Length - iv.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }
    }

    private async Task<byte[]> CompressData(byte[] data)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var gzipStream = new System.IO.Compression.GZipStream(memoryStream, 
                System.IO.Compression.CompressionLevel.Optimal))
            {
                await gzipStream.WriteAsync(data, 0, data.Length);
            }
            return memoryStream.ToArray();
        }
    }

    private async Task<byte[]> DecompressData(byte[] data)
    {
        using (var memoryStream = new MemoryStream(data))
        using (var resultStream = new MemoryStream())
        using (var gzipStream = new System.IO.Compression.GZipStream(memoryStream, 
            System.IO.Compression.CompressionMode.Decompress))
        {
            await gzipStream.CopyToAsync(resultStream);
            return resultStream.ToArray();
        }
    }

    // Data collection methods
    private async Task<Dictionary<string, object>> CollectPlayerData()
    {
        // Implementation to collect player data
        return new Dictionary<string, object>();
    }

    private async Task<Dictionary<string, object>> CollectWorldData()
    {
        // Implementation to collect world data
        return new Dictionary<string, object>();
    }

    private async Task<Dictionary<string, object>> CollectEconomyData()
    {
        // Implementation to collect economy data
        return new Dictionary<string, object>();
    }

    private async Task<Dictionary<string, object>> CollectDiplomacyData()
    {
        // Implementation to collect diplomacy data
        return new Dictionary<string, object>();
    }

    private async Task<Dictionary<string, object>> CollectTechnologyData()
    {
        // Implementation to collect technology data
        return new Dictionary<string, object>();
    }

    private async Task<Dictionary<string, object>> CollectCultureData()
    {
        // Implementation to collect culture data
        return new Dictionary<string, object>();
    }

    // Data application methods
    private async Task ApplyPlayerData(object data)
    {
        // Implementation to apply player data
        await Task.Yield(); // Ensures the method is truly async
        return;
    }

    private async Task ApplyWorldData(object data)
    {
        // Implementation to apply world data
    }

    private async Task ApplyEconomyData(object data)
    {
        // Implementation to apply economy data
    }

    private async Task ApplyDiplomacyData(object data)
    {
        // Implementation to apply diplomacy data
    }

    private async Task ApplyTechnologyData(object data)
    {
        // Implementation to apply technology data
    }

    private async Task ApplyCultureData(object data)
    {
        // Implementation to apply culture data
    }
}
