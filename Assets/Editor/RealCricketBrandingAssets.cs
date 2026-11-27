#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>Generates the branded Android launcher icon from the in-game shield logo.</summary>
public static class RealCricketBrandingAssets
{
    private const string BrandingFolder = "Assets/Branding";
    private const string IconPath = BrandingFolder + "/real-cricket-3d-app-icon.png";

    public static void ConfigureAndroidIcon()
    {
        Directory.CreateDirectory(BrandingFolder);
        Sprite logo = RealCricketLogo.CreateSprite();
        File.WriteAllBytes(IconPath, logo.texture.EncodeToPNG());
        AssetDatabase.ImportAsset(IconPath, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = AssetImporter.GetAtPath(IconPath) as TextureImporter;
        if (importer == null)
            throw new System.InvalidOperationException("Could not import REAL CRICKET 3D app icon.");

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.SaveAndReimport();

        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
        if (icon == null)
            throw new System.InvalidOperationException("REAL CRICKET 3D app icon did not import.");

        PlayerSettings.SetIcons(NamedBuildTarget.Android, new[] { icon }, IconKind.Application);
        PlayerSettings.productName = "REAL CRICKET 3D";
        PlayerSettings.companyName = "REAL CRICKET STUDIOS";
    }
}
#endif