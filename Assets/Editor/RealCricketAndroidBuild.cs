#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class RealCricketAndroidBuild
{
    public static void BuildAndroid()
    {
        Debug.Log("Preparing CRICNOVA 3D Android build...");

        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Android,
            BuildTarget.Android);

        PrototypeSceneBuilder.BuildPrototypeSceneForBuild();

        const string outputDirectory = "Builds/Android";
        const string outputPath = outputDirectory + "/CRICNOVA3D.apk";
        Directory.CreateDirectory(outputDirectory);

        PlayerSettings.productName = "CRICNOVA 3D";
        PlayerSettings.companyName = "CRICNOVA STUDIOS";
        PlayerSettings.SetApplicationIdentifier(
            UnityEditor.Build.NamedBuildTarget.Android,
            "com.sandeep.realcricket3d");

        Texture2D cricnovaIcon = Resources.Load<Texture2D>("Branding/CricnovaLogo");
        if (cricnovaIcon != null)
        {
            int[] iconSizes = PlayerSettings.GetIconSizes(
                UnityEditor.Build.NamedBuildTarget.Android, IconKind.Application);
            Texture2D[] appIcons = new Texture2D[iconSizes.Length];
            for (int i = 0; i < appIcons.Length; i++)
                appIcons[i] = cricnovaIcon;
            PlayerSettings.SetIcons(
                UnityEditor.Build.NamedBuildTarget.Android, appIcons, IconKind.Application);
        }
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
