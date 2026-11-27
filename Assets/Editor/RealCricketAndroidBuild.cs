#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class RealCricketAndroidBuild
{
    public static void BuildAndroid()
    {
        Debug.Log("Preparing REAL CRICKET 3D Android build...");
        RealCricketBrandingAssets.ConfigureAndroidIcon();

        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Android,
            BuildTarget.Android);

        PrototypeSceneBuilder.BuildPrototypeSceneForBuild();

        const string outputDirectory = "Builds/Android";
        const string outputPath = outputDirectory + "/RealCricket3D.apk";
        Directory.CreateDirectory(outputDirectory);

        PlayerSettings.productName = "REAL CRICKET 3D";
        PlayerSettings.companyName = "REAL CRICKET STUDIOS";
        PlayerSettings.SetApplicationIdentifier(
            UnityEditor.Build.NamedBuildTarget.Android,
            "com.sandeep.realcricket3d");
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/CricketPrototype.unity" },
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new System.Exception(
                "Android build failed: " + report.summary.result +
                " (" + report.summary.totalErrors + " errors).");
        }

        Debug.Log("APK created: " + Path.GetFullPath(outputPath));
    }
}
#endif
