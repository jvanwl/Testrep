using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine;
using System;

public class BuildAutomation
{
    public static void BuildAndroidBundle()
    {
        try
        {
            // Set the version info
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;

            // Configure Android settings
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = "release.keystore";
            PlayerSettings.Android.keystorePass = "civilization_game_pass";
            PlayerSettings.Android.keyaliasName = "civilization_game";
            PlayerSettings.Android.keyaliasPass = "civilization_game_pass";

            // Set the build target to Android
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.buildAppBundle = true;

            // Configure Android architecture
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            // Set IL2CPP scripting backend
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.High);

            // Build the player
            BuildReport report = BuildPipeline.BuildPlayer(
                EditorBuildSettings.scenes,
                "Builds/Android/CivilizationGame.aab",
                BuildTarget.Android,
                BuildOptions.None
            );

            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }
            else
            {
                Debug.Log("Build failed");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Build failed with exception: " + e.Message);
        }
    }
}
