#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class RealCricketAndroidBuild
{
    private const string LogoPath = "Assets/Resources/Branding/CricnovaLogo.png";
    private const string IconBackgroundPath = "Assets/Resources/Branding/CricnovaIconBackground.png";
    private const string SplashLogoPath = "Assets/Resources/Branding/CricnovaSplashLogo.png";
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
        ValidateGeneratedGameplayScene();
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

    private static void ValidateGeneratedGameplayScene()
    {
        RequireSceneComponent<MatchManager>("match scoring manager");
        RequireSceneComponent<BattingController>("batter controller");
        RequireSceneComponent<BatCollisionRelay>("bat collision relay");
        RequireSceneComponent<BowlingVariationController>("bowling controls");
        RequireSceneComponent<CricketBallDeliveryResolver>("ball delivery resolver");
        RequireSceneComponent<CricketMatchSetupController>("match setup and toss");
        RequireSceneComponent<RunningBetweenWicketsController>("running between wickets");
        RequireSceneComponent<FielderTeamController>("fielding team");
        RequireSceneComponent<Canvas>("mobile HUD canvas");
        RequireSceneComponent<UnityEngine.EventSystems.EventSystem>("mobile UI event system");
        Debug.Log("CRICNOVA gameplay scene validation passed.");
    }

    private static void RequireSceneComponent<T>(string description) where T : Component
    {
        if (Object.FindAnyObjectByType<T>() == null)
            throw new System.Exception("Generated CRICNOVA scene is missing " + description + ".");
    }

    private static void ConfigureBrandIdentity()
    {
        // The previous repository JPEG was corrupt (Unity's importer reported
        // an invalid JPEG structure). Generate a clean, deterministic PNG
        // crest during the build so CI never depends on a broken binary asset.
        // Replace this procedural fallback with the supplied high-resolution
        // shield artwork once that source file is re-uploaded to the repository.
        CreateCricnovaLogoAsset();

        TextureImporter logoImporter = AssetImporter.GetAtPath(LogoPath) as TextureImporter;
        if (logoImporter == null)
            throw new System.Exception("Unity could not import the generated CRICNOVA logo texture.");
        logoImporter.textureType = TextureImporterType.Default;
        logoImporter.mipmapEnabled = false;
        logoImporter.alphaIsTransparency = true;
        logoImporter.maxTextureSize = 1024;
        logoImporter.textureCompression = TextureImporterCompression.Uncompressed;
        logoImporter.SaveAndReimport();

        AssetDatabase.ImportAsset(LogoPath, ImportAssetOptions.ForceUpdate);
        Texture2D logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(LogoPath);
        if (logoTexture == null || logoTexture.width != 512 || logoTexture.height != 512)
            throw new System.Exception("Generated CRICNOVA logo did not import as a valid 512x512 Texture2D.");

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

    private static void CreateCricnovaLogoAsset()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(LogoPath));
        const int size = 512;
        Color32 clear = new Color32(0, 0, 0, 0);
        Color32 gold = new Color32(255, 190, 45, 255);
        Color32 deepGold = new Color32(120, 73, 12, 255);
        Color32 black = new Color32(8, 10, 16, 255);
        Color32 white = new Color32(255, 244, 215, 255);
        Color32[] pixels = new Color32[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;

        // Shield silhouette, double gold edge and dark inset.
        Vector2Int[] shield = {
            new Vector2Int(256, 486), new Vector2Int(440, 405),
            new Vector2Int(421, 170), new Vector2Int(256, 92),
            new Vector2Int(91, 170), new Vector2Int(72, 405)
        };
        FillPolygon(pixels, size, shield, deepGold);
        Vector2Int[] inner = {
            new Vector2Int(256, 458), new Vector2Int(414, 389),
            new Vector2Int(397, 187), new Vector2Int(256, 122),
            new Vector2Int(115, 187), new Vector2Int(98, 389)
        };
        FillPolygon(pixels, size, inner, gold);
        Vector2Int[] core = {
            new Vector2Int(256, 432), new Vector2Int(388, 373),
            new Vector2Int(374, 203), new Vector2Int(256, 143),
            new Vector2Int(138, 203), new Vector2Int(124, 373)
        };
        FillPolygon(pixels, size, core, black);

        // Crown above the monogram.
        FillPolygon(pixels, size, new[] {
            new Vector2Int(177, 300), new Vector2Int(163, 235),
            new Vector2Int(211, 264), new Vector2Int(256, 207),
            new Vector2Int(301, 264), new Vector2Int(349, 235),
            new Vector2Int(335, 300)
        }, gold);
        FillRect(pixels, size, 178, 294, 334, 314, gold);

        // Original CN monogram in a clean geometric stroke style.
        DrawLine(pixels, size, 188, 356, 188, 326, white, 13);
        DrawLine(pixels, size, 188, 356, 220, 356, white, 13);
        DrawLine(pixels, size, 188, 326, 220, 326, white, 13);
        DrawLine(pixels, size, 242, 326, 242, 356, white, 13);
        DrawLine(pixels, size, 242, 326, 278, 356, white, 13);
        DrawLine(pixels, size, 278, 356, 278, 326, white, 13);

        // Cricket bat and red-gold ball mark.
        DrawLine(pixels, size, 298, 248, 346, 198, gold, 17);
        DrawLine(pixels, size, 346, 198, 362, 182, white, 10);
        FillCircle(pixels, size, 151, 250, 22, gold);
        DrawLine(pixels, size, 137, 238, 164, 262, deepGold, 4);
        DrawLine(pixels, size, 139, 258, 161, 237, deepGold, 4);

        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.SetPixels32(pixels);
        texture.Apply(false, false);
        File.WriteAllBytes(LogoPath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(LogoPath, ImportAssetOptions.ForceUpdate);
    }

    private static void FillPolygon(Color32[] pixels, int size, Vector2Int[] points, Color32 color)
    {
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            bool inside = false;
            for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                Vector2Int a = points[i], b = points[j];
                if (((a.y > y) != (b.y > y)) &&
                    x < (b.x - a.x) * (y - a.y) / (float)(b.y - a.y) + a.x)
                    inside = !inside;
            }
            if (inside) pixels[y * size + x] = color;
        }
    }

    private static void FillRect(Color32[] pixels, int size, int x0, int y0, int x1, int y1, Color32 color)
    {
        for (int y = Mathf.Max(0, y0); y < Mathf.Min(size, y1); y++)
        for (int x = Mathf.Max(0, x0); x < Mathf.Min(size, x1); x++)
            pixels[y * size + x] = color;
    }

    private static void FillCircle(Color32[] pixels, int size, int cx, int cy, int radius, Color32 color)
    {
        for (int y = -radius; y <= radius; y++)
        for (int x = -radius; x <= radius; x++)
            if (x * x + y * y <= radius * radius)
            {
                int px = cx + x, py = cy + y;
                if (px >= 0 && px < size && py >= 0 && py < size)
                    pixels[py * size + px] = color;
            }
    }

    private static void DrawLine(Color32[] pixels, int size, int x0, int y0, int x1, int y1, Color32 color, int thickness)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int error = dx + dy;
        while (true)
        {
            FillCircle(pixels, size, x0, y0, Mathf.Max(1, thickness / 2), color);
            if (x0 == x1 && y0 == y1) break;
            int twice = 2 * error;
            if (twice >= dy) { error += dy; x0 += sx; }
            if (twice <= dx) { error += dx; y0 += sy; }
        }
    }

    private static void ConfigureAdaptiveLauncherIcon(Texture2D logo)
    {
        // Unity 6.6 selects adaptive icon layers through SetPlatformIcons;
        // it does not expose a separate Android iconSourceType property.
        Texture2D background = CreateOrLoadIconBackground();
        PlatformIcon[] slots = PlayerSettings.GetPlatformIcons(
            NamedBuildTarget.Android, AndroidPlatformIconKind.Adaptive);

        if (slots == null || slots.Length == 0)
            throw new System.Exception("Unity Android Adaptive icon slots are unavailable.");

        // Android adaptive icon layers are background first, foreground second.
        // Use the exact layer count Unity reports for each slot (some versions
        // expose an optional monochrome layer as well).
        foreach (PlatformIcon slot in slots)
        {
            Texture2D[] layers = new Texture2D[slot.maxLayerCount];
            if (layers.Length < 2)
                throw new System.Exception("Android adaptive icon slot does not expose background and foreground layers.");

            layers[0] = background;
            layers[1] = logo;
            for (int layer = 2; layer < layers.Length; layer++)
                layers[layer] = logo;
            slot.SetTextures(layers);
        }

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
