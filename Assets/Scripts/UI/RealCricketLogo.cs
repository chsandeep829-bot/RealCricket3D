using UnityEngine;

/// <summary>
/// Creates the original REAL CRICKET 3D shield mark as a crisp transparent UI sprite.
/// The sprite is generated locally, so the game has no network or third-party asset dependency.
/// </summary>
public static class RealCricketLogo
{
    private static Sprite cached;

    public static Sprite CreateSprite()
    {
        if (cached != null) return cached;

        const int size = 512;
        Color32[] pixels = new Color32[size * size];
        Color32 gold = new Color32(226, 188, 91, 255);
        Color32 highlight = new Color32(255, 225, 151, 255);
        Color32 black = new Color32(10, 12, 16, 255);
        Color32 ball = new Color32(173, 42, 31, 255);

        Vector2[] outer = {
            new Vector2(256, 12), new Vector2(442, 78), new Vector2(424, 286),
            new Vector2(370, 385), new Vector2(256, 492), new Vector2(142, 385),
            new Vector2(88, 286), new Vector2(70, 78)
        };
        Vector2[] inner = {
            new Vector2(256, 42), new Vector2(411, 96), new Vector2(395, 279),
            new Vector2(345, 365), new Vector2(256, 461), new Vector2(167, 365),
            new Vector2(117, 279), new Vector2(101, 96)
        };

        FillPolygon(pixels, size, outer, gold);
        FillPolygon(pixels, size, inner, black);

        // Three gold stumps and bails.
        FillRect(pixels, size, 183, 158, 205, 345, highlight);
        FillRect(pixels, size, 244, 158, 266, 365, gold);
        FillRect(pixels, size, 305, 158, 327, 345, highlight);
        FillRect(pixels, size, 171, 350, 339, 365, gold);

        // Cricket ball and its curved seam.
        FillCircle(pixels, size, 256, 112, 48, ball);
        DrawLine(pixels, size, new Vector2(230, 78), new Vector2(282, 146), 5, highlight);
        DrawLine(pixels, size, new Vector2(240, 72), new Vector2(292, 140), 3, gold);

        // Small gold lower chevron creates a shield/crest finish.
        DrawLine(pixels, size, new Vector2(168, 52), new Vector2(256, 24), 5, highlight);
        DrawLine(pixels, size, new Vector2(256, 24), new Vector2(344, 52), 5, highlight);

        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.name = "REAL_CRICKET_3D_Golden_Shield";
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels32(pixels);
        texture.Apply(false, false);
        cached = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 256);
        cached.name = "REAL_CRICKET_3D_Golden_Shield";
        return cached;
    }

    private static void FillPolygon(Color32[] pixels, int size, Vector2[] points, Color32 color)
    {
        float minX = size, minY = size, maxX = 0, maxY = 0;
        for (int i = 0; i < points.Length; i++)
        {
            minX = Mathf.Min(minX, points[i].x);
            minY = Mathf.Min(minY, points[i].y);
            maxX = Mathf.Max(maxX, points[i].x);
            maxY = Mathf.Max(maxY, points[i].y);
        }

        for (int y = Mathf.Max(0, (int)minY); y <= Mathf.Min(size - 1, (int)maxY); y++)
        for (int x = Mathf.Max(0, (int)minX); x <= Mathf.Min(size - 1, (int)maxX); x++)
        {
            bool inside = false;
            for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                Vector2 a = points[i], b = points[j];
                if (((a.y > y) != (b.y > y)) &&
                    x < (b.x - a.x) * (y - a.y) / (b.y - a.y) + a.x)
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
        int r2 = radius * radius;
        for (int y = Mathf.Max(0, cy - radius); y <= Mathf.Min(size - 1, cy + radius); y++)
        for (int x = Mathf.Max(0, cx - radius); x <= Mathf.Min(size - 1, cx + radius); x++)
        {
            int dx = x - cx, dy = y - cy;
            if (dx * dx + dy * dy <= r2) pixels[y * size + x] = color;
        }
    }

    private static void DrawLine(Color32[] pixels, int size, Vector2 a, Vector2 b, int thickness, Color32 color)
    {
        int steps = Mathf.CeilToInt(Vector2.Distance(a, b));
        int radius = Mathf.Max(1, thickness / 2);
        for (int i = 0; i <= steps; i++)
        {
            Vector2 p = Vector2.Lerp(a, b, steps == 0 ? 0f : (float)i / steps);
            FillCircle(pixels, size, Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y), radius, color);
        }
    }
}