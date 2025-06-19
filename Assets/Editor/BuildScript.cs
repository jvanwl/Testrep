using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class BuildScript
{
    static readonly string[] scenes = {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/MainGame.unity",
        "Assets/Scenes/LoadingScreen.unity"
    };

    static readonly string keyStorePath = "release.keystore";
    static readonly string keyStorePass = "civilization_game_pass";
    static readonly string keyStoreAlias = "civilization_game";
    static readonly string keyStoreAliasPass = "civilization_game_pass";

    [MenuItem("Build/Android/Development")]
    public static void BuildAndroidDevelopment()
    {
        BuildAndroid(false);
    }

    [MenuItem("Build/Android/Release")]
    public static void BuildAndroidRelease()
    {
        BuildAndroid(true);
    }

    static void BuildAndroid(bool isRelease)
    {
        // Set the version info
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 1;

        // Configure Android settings
        PlayerSettings.Android.useCustomKeystore = isRelease;
        if (isRelease)
        {
            PlayerSettings.Android.keystoreName = keyStorePath;
            PlayerSettings.Android.keystorePass = keyStorePass;
            PlayerSettings.Android.keyaliasName = keyStoreAlias;
            PlayerSettings.Android.keyaliasPass = keyStoreAliasPass;
        }

        // Set build options
        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = $"Builds/Android/CivilizationGame_{DateTime.Now:yyyyMMdd}_{(isRelease ? "Release" : "Debug")}.aab",
            target = BuildTarget.Android,
            options = isRelease ? 
                BuildOptions.None : 
                BuildOptions.Development | BuildOptions.AllowDebugging
        };

        // Configure Android build settings
        EditorUserBuildSettings.buildAppBundle = true;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        
        // Set optimization settings for release
        if (isRelease)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.High);
        }

        // Build the project
        BuildPipeline.BuildPlayer(options);
    }

    [MenuItem("Build/Android/Clean")]
    public static void CleanBuild()
    {
        if (Directory.Exists("Builds/Android"))
        {
            Directory.Delete("Builds/Android", true);
            Debug.Log("Cleaned Android build directory");
        }
    }
}
