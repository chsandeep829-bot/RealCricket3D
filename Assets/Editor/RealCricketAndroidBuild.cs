#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class RealCricketAndroidBuild
{
    private const string LogoPath = "Assets/Resources/Branding/CricnovaLogo.jpg";
    private const string IconBackgroundPath = "Assets/Resources/Branding/CricnovaIconBackground.png";
    private const string SplashLogoPath = "Assets/Resources/Branding/CricnovaSplashLogo.jpg";
    private const string OutputDirectory = "Builds/Android";
    private const string OutputPath = OutputDirectory + "/CRICNOVA3D.apk";

    public static void BuildAndroid()
    {
        Debug.Log("Preparing CRICNOVA 3D Android build...");

        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Android,
            BuildTarget.Android);

        ConfigureBrandIdentity();

        PrototypeSceneBuilder.BuildPrototypeSceneForBuild();
        Directory.CreateDirectory(OutputDirectory);

        PlayerSettings.productName = "CRICNOVA 3D";
        PlayerSettings.companyName = "CRICNOVA STUDIOS";
        PlayerSettings.bundleVersion = "0.2.0";
        PlayerSettings.SetApplicationIdentifier(
            NamedBuildTarget.Android,
            "com.sandeep.realcricket3d");
        PlayerSettings.Android.bundleVersionCode = 2;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/CricketPrototype.unity" },
            locationPathName = OutputPath,
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

        if (!File.Exists(OutputPath) || new FileInfo(OutputPath).Length < 1024 * 1024)
            throw new System.Exception("CRICNOVA APK was not generated or is unexpectedly small.");

        Debug.Log("CRICNOVA 3D signed APK created: " + Path.GetFullPath(OutputPath));
    }

    private static void ConfigureBrandIdentity()
    {
        if (!File.Exists(LogoPath))
            throw new System.Exception("Missing CRICNOVA logo asset: " + LogoPath);

        // Keep the source logo as a Texture2D so Resources.Load<Texture2D> in
        // the in-game UI continues to work. Create a separate Sprite asset for
        // Unity's native splash-screen logo slot.
        TextureImporter logoImporter = AssetImporter.GetAtPath(LogoPath) as TextureImporter;
        if (logoImporter == null)
            throw new System.Exception("Unity could not import the CRICNOVA logo texture.");

        if (logoImporter.textureType != TextureImporterType.Default || logoImporter.mipmapEnabled)
        {
            logoImporter.textureType = TextureImporterType.Default;
            logoImporter.mipmapEnabled = false;
            logoImporter.alphaIsTransparency = false;
            logoImporter.maxTextureSize = 2048;
            logoImporter.SaveAndReimport();
        }

        AssetDatabase.ImportAsset(LogoPath, ImportAssetOptions.ForceUpdate);
        Texture2D logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(LogoPath);
        if (logoTexture == null)
            throw new System.Exception("CRICNOVA logo did not import as a Texture2D.");

        Directory.CreateDirectory(Path.GetDirectoryName(SplashLogoPath));
        File.Copy(LogoPath, SplashLogoPath, true);
        AssetDatabase.ImportAsset(SplashLogoPath, ImportAssetOptions.ForceUpdate);
        TextureImporter splashImporter = AssetImporter.GetAtPath(SplashLogoPath) as TextureImporter;
        if (splashImporter == null)
            throw new System.Exception("Unity could not import the CRICNOVA splash logo.");
        splashImporter.textureType = TextureImporterType.Sprite;
        splashImporter.spriteImportMode = SpriteImportMode.Single;
        splashImporter.mipmapEnabled = false;
        splashImporter.alphaIsTransparency = false;
        splashImporter.maxTextureSize = 2048;
        splashImporter.SaveAndReimport();

        Sprite logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SplashLogoPath);
        if (logoSprite == null)
            throw new System.Exception("CRICNOVA splash logo did not import as a Sprite.");

        ConfigureAdaptiveLauncherIcon(logoTexture);

        // Replace the Unity splash mark with the CRICNOVA shield.
        PlayerSettings.SplashScreen.show = true;
        PlayerSettings.SplashScreen.showUnityLogo = false;
        PlayerSettings.SplashScreen.backgroundColor = new Color(0.008f, 0.012f, 0.02f, 1f);
        PlayerSettings.SplashScreen.overlayOpacity = 0f;
        PlayerSettings.SplashScreen.logos = new[]
        {
            PlayerSettings.SplashScreenLogo.Create(2.5f, logoSprite)
        };

        Debug.Log("CRICNOVA branding configured: adaptive Android icon and native splash logo.");
    }

    private static void ConfigureAdaptiveLauncherIcon(Texture2D logo)
    {
        Texture2D background = CreateOrLoadIconBackground();
        PlatformIcon[] slots = PlayerSettings.GetPlatformIcons(
            NamedBuildTarget.Android, AndroidPlatformIconKind.Adaptive);

        if (slots == null || slots.Length == 0)
            throw new System.Exception("Unity Android Adaptive icon slots are unavailable.");

        // Android adaptive icon layers are background first, foreground second.
        // The supplied shield image remains the foreground artwork.
        foreach (PlatformIcon slot in slots)
            slot.SetTextures(new[] { background, logo });

        PlayerSettings.SetPlatformIcons(
            NamedBuildTarget.Android, AndroidPlatformIconKind.Adaptive, slots);
        Debug.Log("Configured " + slots.Length + " Android adaptive launcher icon slot(s).");
    }

    private static Texture2D CreateOrLoadIconBackground()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(IconBackgroundPath));

        const int size = 432;
        Texture2D source = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[size * size];
        Color32 black = new Color32(5, 8, 15, 255);
        Color32 gold = new Color32(105, 70, 18, 255);

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x - size * 0.5f;
            float dy = y - size * 0.5f;
            float radius = Mathf.Sqrt(dx * dx + dy * dy);
            pixels[y * size + x] = Mathf.Abs(radius - 205f) < 5f ? gold : black;
        }

        source.SetPixels32(pixels);
        source.Apply(false, false);
        File.WriteAllBytes(IconBackgroundPath, source.EncodeToPNG());
        Object.DestroyImmediate(source);
        AssetDatabase.ImportAsset(IconBackgroundPath, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = AssetImporter.GetAtPath(IconBackgroundPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = false;
            importer.maxTextureSize = 512;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        Texture2D background = AssetDatabase.LoadAssetAtPath<Texture2D>(IconBackgroundPath);
        if (background == null)
            throw new System.Exception("Could not create the CRICNOVA adaptive icon background.");
        return background;
    }
}
#endif
