using UnityEngine;

/// <summary>
/// Applies team colours to the generated prototype player's kit without
/// instantiating one material per player. Helmet and trim use the accent colour.
/// </summary>
public class CricketPlayerKit : MonoBehaviour
{
    [SerializeField] private Color primaryColor = new Color(0.08f, 0.22f, 0.55f);
    [SerializeField] private Color accentColor = new Color(0.95f, 0.68f, 0.12f);

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        ApplyTeamColors(primaryColor, accentColor);
    }

    public void ApplyTeamColors(Color primary, Color accent)
    {
        primaryColor = primary;
        accentColor = accent;
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer item in renderers)
        {
            if (item == null) continue;
            string partName = item.gameObject.name.ToLowerInvariant();
            bool useAccent = partName.Contains("helmet") || partName.Contains("kittrim");
            Color selected = useAccent ? accentColor : primaryColor;

            item.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorId, selected);
            propertyBlock.SetColor(ColorId, selected);
            item.SetPropertyBlock(propertyBlock);
        }
    }
}
