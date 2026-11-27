using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Original REAL CRICKET 3D title screen for the generated playable cricket scene.
/// The existing 3D stadium remains visible behind this menu; starting a mode reveals
/// the real gameplay HUD and the existing match setup/toss systems.
/// </summary>
public sealed class CricketHomeScreen : MonoBehaviour
{
    private const string HomeObjectName = "RealCricketHomeScreen";
    private const string SetupObjectName = "MatchSetupMenu";
    private const string ResultObjectName = "PostMatchResultPanel";

    private GameObject[] hudObjects;
    private GameObject setupPanel;
    private GameObject resultPanel;
    private GameObject homePanel;
    private CricketMatchSetupController matchSetup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null || canvas.name != "GameHUD") return;
        if (canvas.transform.Find(HomeObjectName) != null) return;

        CricketHomeScreen screen = canvas.gameObject.AddComponent<CricketHomeScreen>();
        screen.Build(canvas);
    }

    private void Build(Canvas canvas)
    {
        hudObjects = new GameObject[canvas.transform.childCount];
        for (int i = 0; i < canvas.transform.childCount; i++)
            hudObjects[i] = canvas.transform.GetChild(i).gameObject;

        setupPanel = canvas.transform.Find(SetupObjectName)?.gameObject;
        resultPanel = canvas.transform.Find(ResultObjectName)?.gameObject;
        matchSetup = Object.FindObjectOfType<CricketMatchSetupController>();

        for (int i = 0; i < hudObjects.Length; i++)
            hudObjects[i].SetActive(false);

        if (resultPanel != null) resultPanel.SetActive(false);
        if (setupPanel != null) setupPanel.SetActive(false);

        homePanel = new GameObject(HomeObjectName, typeof(RectTransform), typeof(Image));
        homePanel.transform.SetParent(canvas.transform, false);
        homePanel.transform.SetAsLastSibling();

        RectTransform panelRect = homePanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image background = homePanel.GetComponent<Image>();
        background.color = new Color(0.012f, 0.025f, 0.055f, 0.96f);

        CreateAccent(homePanel.transform, new Vector2(0.5f, 0.995f),
            new Vector2(1f, 0.01f), new Color(0.92f, 0.62f, 0.16f));

        Image logo = CreateImage(homePanel.transform, "OriginalCricketEmblem",
            new Vector2(0.5f, 0.78f), new Vector2(190f, 190f), CreateEmblem());
        logo.preserveAspect = true;

        CreateText(homePanel.transform, "REAL CRICKET 3D",
            new Vector2(0.5f, 0.605f), new Vector2(1000f, 100f), 76,
            new Color(0.98f, 0.97f, 0.91f), FontStyle.Bold);
        CreateText(homePanel.transform, "OWN THE PITCH  •  PLAY YOUR GAME",
            new Vector2(0.5f, 0.535f), new Vector2(900f, 54f), 25,
            new Color(0.91f, 0.66f, 0.25f), FontStyle.Bold);
        CreateText(homePanel.transform, "A 3D cricket experience by Sandeep Studio",
            new Vector2(0.5f, 0.475f), new Vector2(900f, 48f), 22,
            new Color(0.72f, 0.79f, 0.88f), FontStyle.Normal);

        CreateMenuButton(homePanel.transform, "PLAY MATCH",
            new Vector2(0.5f, 0.345f), new Vector2(470f, 92f),
            new Color(0.72f, 0.12f, 0.12f), OpenMatchSetup);
        CreateMenuButton(homePanel.transform, "4-TEAM CHAMPIONS CUP",
            new Vector2(0.5f, 0.225f), new Vector2(470f, 78f),
            new Color(0.10f, 0.23f, 0.38f), StartTournament);
        CreateText(homePanel.transform, "OFFLINE PLAY  •  T20  •  ODI  •  TEST",
            new Vector2(0.5f, 0.085f), new Vector2(900f, 44f), 20,
            new Color(0.68f, 0.75f, 0.83f), FontStyle.Normal);
        CreateText(homePanel.transform, "REAL CRICKET 3D",
            new Vector2(0.5f, 0.035f), new Vector2(500f, 32f), 16,
            new Color(0.48f, 0.57f, 0.68f), FontStyle.Bold);
    }

    private void OpenMatchSetup()
    {
        RevealGameplayHud();
        if (setupPanel != null) setupPanel.SetActive(true);
        else if (matchSetup != null) matchSetup.StartConfiguredMatch();
    }

    private void StartTournament()
    {
        if (matchSetup == null)
        {
            OpenMatchSetup();
            return;
        }

        RevealGameplayHud();
        if (setupPanel != null) setupPanel.SetActive(false);
        matchSetup.CreateFourTeamTournamentAndStart();
    }

    private void RevealGameplayHud()
    {
        if (homePanel != null) homePanel.SetActive(false);
        for (int i = 0; i < hudObjects.Length; i++)
        {
            GameObject item = hudObjects[i];
            if (item == null || item == setupPanel || item == resultPanel) continue;
            item.SetActive(true);
        }
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private static void CreateAccent(Transform parent, Vector2 anchor,
        Vector2 size, Color color)
    {
        GameObject stripe = new GameObject("GoldAccent", typeof(RectTransform), typeof(Image));
        stripe.transform.SetParent(parent, false);
        RectTransform rect = stripe.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(1920f * size.x, 1080f * size.y);
        rect.anchoredPosition = Vector2.zero;
        stripe.GetComponent<Image>().color = color;
    }

    private static Image CreateImage(Transform parent, string objectName,
        Vector2 anchor, Vector2 size, Sprite sprite)
    {
        GameObject item = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        item.transform.SetParent(parent, false);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        Image image = item.GetComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;
        image.raycastTarget = false;
        return image;
    }

    private static Text CreateText(Transform parent, string value, Vector2 anchor,
        Vector2 size, int fontSize, Color color, FontStyle style)
    {
        GameObject item = new GameObject("Title_" + value.Replace(" ", ""),
            typeof(RectTransform), typeof(Text));
        item.transform.SetParent(parent, false);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        Text text = item.GetComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }

    private static void CreateMenuButton(Transform parent, string label,
        Vector2 anchor, Vector2 size, Color color, UnityAction action)
    {
        GameObject item = new GameObject("MenuButton_" + label.Replace(" ", ""),
            typeof(RectTransform), typeof(Image), typeof(Button));
        item.transform.SetParent(parent, false);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        Image image = item.GetComponent<Image>();
        image.color = color;
        Button button = item.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        labelObject.transform.SetParent(item.transform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12f, 4f);
        labelRect.offsetMax = new Vector2(-12f, -4f);
        Text text = labelObject.GetComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 29;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
    }

    // Generates an original ball-and-stumps emblem at runtime so no third-party
    // game artwork or external image download is required.
    private static Sprite CreateEmblem()
    {
        const int size = 256;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.name = "REAL_CRICKET_3D_Original_Emblem";
        texture.filterMode = FilterMode.Bilinear;
        Color clear = new Color(0f, 0f, 0f, 0f);
        Color gold = new Color(0.96f, 0.66f, 0.18f, 1f);
        Color navy = new Color(0.035f, 0.09f, 0.17f, 1f);
        Color red = new Color(0.72f, 0.075f, 0.09f, 1f);
        Color white = new Color(0.96f, 0.97f, 0.94f, 1f);

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x - 128f;
            float dy = y - 128f;
            float radius = Mathf.Sqrt(dx * dx + dy * dy);
            Color pixel = clear;

            if (radius <= 119f) pixel = navy;
            if (radius >= 105f && radius <= 119f) pixel = gold;
            if (radius <= 65f) pixel = red;

            // White curved ball seam, drawn as a narrow offset ellipse.
            float seam = Mathf.Abs((dx * 0.48f + dy * 0.88f) -
                12f * Mathf.Sin(dy / 22f));
            if (radius <= 65f && seam < 3.2f) pixel = white;

            // Three gold stumps and a bails line above the ball.
            if (x >= 94 && x <= 99 && y >= 180 && y <= 226) pixel = gold;
            if (x >= 125 && x <= 131 && y >= 180 && y <= 226) pixel = gold;
            if (x >= 157 && x <= 162 && y >= 180 && y <= 226) pixel = gold;
            if (y >= 224 && y <= 230 && x >= 91 && x <= 165) pixel = white;

            texture.SetPixel(x, y, pixel);
        }

        texture.Apply(false, false);
        return Sprite.Create(texture, new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f), 100f);
    }
}
