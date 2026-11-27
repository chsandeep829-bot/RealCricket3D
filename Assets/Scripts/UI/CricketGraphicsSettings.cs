using UnityEngine;

/// <summary>
/// Runtime Android graphics presets. Attach once to a persistent settings object
/// and connect the public methods to Settings UI buttons.
/// </summary>
public class CricketGraphicsSettings : MonoBehaviour
{
    public enum GraphicsPreset { Low, Medium, High }

    private const string PresetKey = "RealCricket3D.GraphicsPreset";
    private const string FrameRateKey = "RealCricket3D.FrameRate";

    public GraphicsPreset CurrentPreset { get; private set; } = GraphicsPreset.Medium;
    public int TargetFrameRate { get; private set; } = 60;

    private void Awake()
    {
        CurrentPreset = (GraphicsPreset)Mathf.Clamp(
            PlayerPrefs.GetInt(PresetKey, (int)GraphicsPreset.Medium), 0, 2);
        TargetFrameRate = PlayerPrefs.GetInt(FrameRateKey, 60);
        ApplyPreset(CurrentPreset, false);
        SetTargetFrameRate(TargetFrameRate, false);
    }

    public void SetLow() => ApplyPreset(GraphicsPreset.Low);
    public void SetMedium() => ApplyPreset(GraphicsPreset.Medium);
    public void SetHigh() => ApplyPreset(GraphicsPreset.High);

    public void ApplyPreset(GraphicsPreset preset) => ApplyPreset(preset, true);

    private void ApplyPreset(GraphicsPreset preset, bool save)
    {
        CurrentPreset = preset;
        switch (preset)
        {
            case GraphicsPreset.Low:
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.shadowDistance = 15f;
                QualitySettings.pixelLightCount = 1;
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                break;
            case GraphicsPreset.High:
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.shadowDistance = 60f;
                QualitySettings.pixelLightCount = 4;
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                break;
            default:
                QualitySettings.shadows = ShadowQuality.HardOnly;
                QualitySettings.shadowDistance = 30f;
                QualitySettings.pixelLightCount = 2;
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                break;
        }

        if (save)
        {
            PlayerPrefs.SetInt(PresetKey, (int)preset);
            PlayerPrefs.Save();
        }
    }

    public void Set30Fps() => SetTargetFrameRate(30);
    public void Set60Fps() => SetTargetFrameRate(60);
    public void Set90Fps() => SetTargetFrameRate(90);
    public void Set120Fps() => SetTargetFrameRate(120);

    public void SetTargetFrameRate(int frameRate) => SetTargetFrameRate(frameRate, true);

    private void SetTargetFrameRate(int frameRate, bool save)
    {
        TargetFrameRate = Mathf.Clamp(frameRate, 30, 120);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFrameRate;
        if (save)
        {
            PlayerPrefs.SetInt(FrameRateKey, TargetFrameRate);
            PlayerPrefs.Save();
        }
    }
}