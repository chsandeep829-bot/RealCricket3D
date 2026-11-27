using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// CRICNOVA 3D branded entrance and home menu. The existing match HUD, toss,
/// scoring, batting, bowling and tournament controllers remain the gameplay source of truth.
/// Add artwork at Resources/Branding/CricnovaSplash and CricnovaLogo (Texture2D).
/// </summary>
public sealed class CricketHomeScreen : MonoBehaviour
{
    private const string HomeName = "CricnovaHomeScreen";
    private const string SplashName = "CricnovaStadiumEntrance";
    private const string SetupName = "MatchSetupMenu";
    private const string ResultName = "PostMatchResultPanel";

    private static readonly Color Black = new Color(0.008f, 0.012f, 0.02f, 1f);
    private static readonly Color Panel = new Color(0.025f, 0.035f, 0.052f, 0.98f);
    private static readonly Color Gold = new Color(0.91f, 0.67f, 0.22f, 1f);
    private static readonly Color BrightGold = new Color(1f, 0.82f, 0.39f, 1f);
    private static Font UiFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

    private GameObject[] gameplayHud;
    private GameObject setupPanel;
    private GameObject resultPanel;
    private GameObject splashPanel;
    private GameObject homePanel;
    private CricketMatchSetupController matchSetup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null || canvas.name != "GameHUD") return;
        if (canvas.transform.Find(HomeName) != null) return;
        canvas.gameObject.AddComponent<CricketHomeScreen>().Build(canvas);
    }

    private void Build(Canvas canvas)
    {
        gameplayHud = new GameObject[canvas.transform.childCount];
        for (int i = 0; i < gameplayHud.Length; i++)
        {
            gameplayHud[i] = canvas.transform.GetChild(i).gameObject;
            gameplayHud[i].SetActive(false);
        }

        setupPanel = canvas.transform.Find(SetupName)?.gameObject;
        resultPanel = canvas.transform.Find(ResultName)?.gameObject;
        matchSetup = Object.FindAnyObjectByType<CricketMatchSetupController>();
        if (setupPanel != null) setupPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);

        BuildDashboard(canvas.transform);
        BuildStadiumEntrance(canvas.transform);
    }

    private void BuildStadiumEntrance(Transform parent)
    {
        splashPanel = MakePanel(parent, SplashName, Black, new Vector2(.5f, .5f),
            Vector2.one, Vector2.zero);
        splashPanel.transform.SetAsLastSibling();

        Texture2D splashTexture = Resources.Load<Texture2D>("Branding/CricnovaSplash");
        Sprite splashSprite = splashTexture != null ? Sprite.Create(
            splashTexture, new Rect(0, 0, splashTexture.width, splashTexture.height),
            new Vector2(.5f, .5f), 100f) : CreateFallbackStadiumArt();

        Image backdrop = MakeImage(splashPanel.transform, "GoldenStadiumArtwork",
            new Vector2(.5f, .5f), Vector2.zero, splashSprite);
        RectTransform backdropRect = backdrop.rectTransform;
        backdropRect.anchorMin = Vector2.zero;
        backdropRect.anchorMax = Vector2.one;
        backdropRect.offsetMin = Vector2.zero;
        backdropRect.offsetMax = Vector2.zero;
        backdrop.preserveAspect = true;

        MakePanel(splashPanel.transform, "EntranceTint",
            new Color(0.005f, 0.008f, 0.014f, .24f),
            new Vector2(.5f, .5f), Vector2.one, Vector2.zero);

        Texture2D logoTexture = Resources.Load<Texture2D>("Branding/CricnovaLogo");
        Sprite logoSprite = logoTexture != null ? Sprite.Create(
            logoTexture, new Rect(0, 0, logoTexture.width, logoTexture.height),
            new Vector2(.5f, .5f), 100f) : CreateFallbackLogo();

        Image logo = MakeImage(splashPanel.transform, "CRICNOVA_Golden_Shield_Logo",
            new Vector2(.5f, .50f), new Vector2(760f, 500f), logoSprite);
        logo.preserveAspect = true;

        MakePanel(splashPanel.transform, "TopGoldLine", Gold,
            new Vector2(.5f, .992f), new Vector2(1f, .008f), Vector2.zero);
        MakePanel(splashPanel.transform, "BottomGoldLine", Gold,
            new Vector2(.5f, .008f), new Vector2(1f, .008f), Vector2.zero);

        MakeLabel(splashPanel.transform, "CRICNOVA STUDIOS",
            new Vector2(.5f, .13f), new Vector2(650f, 52f), 27,
            BrightGold, TextAnchor.MiddleCenter);
        MakeLabel(splashPanel.transform, "THE ULTIMATE CRICKET EXPERIENCE",
            new Vector2(.5f, .085f), new Vector2(900f, 42f), 19,
            Color.white, TextAnchor.MiddleCenter);
        MakeButton(splashPanel.transform, "ENTER CRICNOVA 3D",
            new Vector2(.5f, .035f), new Vector2(360f, 64f),
            Gold, EnterDashboard, 24);
    }

    private void BuildDashboard(Transform parent)
    {
        homePanel = MakePanel(parent, HomeName, Black,
            new Vector2(.5f, .5f), Vector2.one, Vector2.zero);
        homePanel.SetActive(false);

        MakePanel(homePanel.transform, "GoldHeaderRule", Gold,
            new Vector2(.5f, .985f), new Vector2(1f, .012f), Vector2.zero);
        MakePanel(homePanel.transform, "GoldFooterRule", Gold,
            new Vector2(.5f, .12f), new Vector2(1f, .004f), Vector2.zero);

        Texture2D logoTexture = Resources.Load<Texture2D>("Branding/CricnovaLogo");
        Sprite logoSprite = logoTexture != null ? Sprite.Create(
            logoTexture, new Rect(0, 0, logoTexture.width, logoTexture.height),
            new Vector2(.5f, .5f), 100f) : CreateFallbackLogo();

        Image logo = MakeImage(homePanel.transform, "CricnovaHeaderLogo",
            new Vector2(.12f, .86f), new Vector2(260f, 150f), logoSprite);
        logo.preserveAspect = true;

        MakeLabel(homePanel.transform, "CRICNOVA 3D",
            new Vector2(.50f, .90f), new Vector2(700f, 70f), 48,
            Color.white, TextAnchor.MiddleCenter);
        MakeLabel(homePanel.transform, "CRICNOVA STUDIOS",
            new Vector2(.50f, .845f), new Vector2(600f, 42f), 22,
            BrightGold, TextAnchor.MiddleCenter);
        MakeLabel(homePanel.transform, "SELECT YOUR MATCH",
            new Vector2(.20f, .735f), new Vector2(520f, 48f), 26,
            BrightGold, TextAnchor.MiddleLeft);

        MakeGameModeCard(homePanel.transform, "QUICK MATCH",
            "Choose teams • Toss • Play", new Vector2(.22f, .54f),
            new Color(.16f, .105f, .035f, 1f), Play);
        MakeGameModeCard(homePanel.transform, "TOURNAMENT",
            "Four-team knockout cup", new Vector2(.22f, .32f),
            new Color(.12f, .09f, .035f, 1f), StartTournament);
        MakeGameModeCard(homePanel.transform, "PRACTICE",
            "Enter match setup to train", new Vector2(.56f, .54f),
            new Color(.055f, .075f, .085f, 1f), Play);
        MakeGameModeCard(homePanel.transform, "CAREER",
            "Local profile and match records", new Vector2(.56f, .32f),
            new Color(.055f, .075f, .085f, 1f), Play);

        MakeLabel(homePanel.transform, "BLACK • GOLD • CRICKET",
            new Vector2(.80f, .76f), new Vector2(400f, 42f), 20,
            BrightGold, TextAnchor.MiddleCenter);
        MakeLabel(homePanel.transform, "PLAY  •  COMPETE  •  WIN",
            new Vector2(.80f, .30f), new Vector2(450f, 54f), 27,
            Gold, TextAnchor.MiddleCenter);
        MakeButton(homePanel.transform, "BACK TO ENTRANCE",
            new Vector2(.5f, .075f), new Vector2(300f, 56f),
            new Color(.12f, .12f, .13f, 1f), ReturnToEntrance, 19);
        MakeLabel(homePanel.transform, "© CRICNOVA STUDIOS",
            new Vector2(.5f, .025f), new Vector2(500f, 28f), 14,
            new Color(.66f, .62f, .52f, 1f), TextAnchor.MiddleCenter);
    }

    private void EnterDashboard()
    {
        if (splashPanel != null) splashPanel.SetActive(false);
        if (homePanel != null) homePanel.SetActive(true);
    }

    private void ReturnToEntrance()
    {
        if (homePanel != null) homePanel.SetActive(false);
        if (splashPanel != null) splashPanel.SetActive(true);
    }

    private void Play()
    {
        if (homePanel != null) homePanel.SetActive(false);
        RevealGameplayHud();
        if (setupPanel != null) setupPanel.SetActive(true);
        else if (matchSetup != null) matchSetup.StartConfiguredMatch();
    }

    private void StartTournament()
    {
        if (homePanel != null) homePanel.SetActive(false);
        RevealGameplayHud();
        if (matchSetup != null) matchSetup.CreateFourTeamTournamentAndStart();
        else if (setupPanel != null) setupPanel.SetActive(true);
    }

    private void RevealGameplayHud()
    {
        foreach (GameObject item in gameplayHud)
            if (item != null && item != setupPanel && item != resultPanel)
                item.SetActive(true);
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private static void MakeGameModeCard(Transform parent, string title, string subtitle,
        Vector2 anchor, Color background, UnityAction action)
    {
        GameObject card = MakePanel(parent, "Mode_" + title.Replace(" ", ""),
            background, anchor, new Vector2(330f / 1920f, 185f / 1080f),
            new Vector2(330f, 185f));
        RectTransform rect = card.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(560f, 170f);

        MakePanel(card.transform, "GoldAccent", Gold,
            new Vector2(.5f, .985f), new Vector2(1f, .025f), Vector2.zero);
        MakeLabel(card.transform, title, new Vector2(.5f, .63f),
            new Vector2(510f, 54f), 28, Color.white, TextAnchor.MiddleCenter);
        MakeLabel(card.transform, subtitle, new Vector2(.5f, .31f),
            new Vector2(510f, 42f), 18, BrightGold, TextAnchor.MiddleCenter);

        Button button = card.AddComponent<Button>();
        button.targetGraphic = card.GetComponent<Image>();
        button.onClick.AddListener(action);
    }

    private static GameObject MakePanel(Transform parent, string name, Color color,
        Vector2 anchor, Vector2 span, Vector2 size)
    {
        GameObject item = new GameObject(name, typeof(RectTransform), typeof(Image));
        item.transform.SetParent(parent, false);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchorMin = anchor - span * .5f;
        rect.anchorMax = anchor + span * .5f;
        rect.sizeDelta = size;
        item.GetComponent<Image>().color = color;
        return item;
    }

    private static Image MakeImage(Transform parent, string name, Vector2 anchor,
        Vector2 size, Sprite sprite)
    {
        GameObject item = new GameObject(name, typeof(RectTransform), typeof(Image));
        item.transform.SetParent(parent, false);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        Image image = item.GetComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;
        image.raycastTarget = false;
        return image;
    }

    private static Text MakeLabel(Transform parent, string value, Vector2 anchor,
        Vector2 size, int fontSize, Color color, TextAnchor alignment)
    {
        GameObject item = new GameObject("Text_" + value.Replace(" ", ""),
            typeof(RectTransform), typeof(Text));
        item.transform.SetParent(parent, false);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        Text text = item.GetComponent<Text>();
        text.text = value;
        text.font = UiFont;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }

    private static void MakeButton(Transform parent, string title, Vector2 anchor,
        Vector2 size, Color color, UnityAction action, int fontSize)
    {
        GameObject buttonObject = MakePanel(parent, "Button_" + title.Replace(" ", ""),
            color, anchor, new Vector2(size.x / 1920f, size.y / 1080f), size);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.onClick.AddListener(action);
        MakeLabel(buttonObject.transform, title, new Vector2(.5f, .5f),
            new Vector2(size.x - 18f, size.y - 10f), fontSize,
            Color.white, TextAnchor.MiddleCenter);
    }

    private static Sprite CreateFallbackLogo()
    {
        const int size = 256;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = new Color(0, 0, 0, 0);
        Color gold = BrightGold;
        Color black = Black;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x - 128f;
            float dy = y - 128f;
            float shield = Mathf.Abs(dx) * .65f + Mathf.Max(0f, dy) * .85f;
            Color pixel = clear;
            if (y > 24 && y < 226 && shield < 126f) pixel = gold;
            if (y > 35 && y < 213 && shield < 112f) pixel = black;
            if (y > 92 && y < 166 && x > 34 && x < 222) pixel = gold;
            if (y > 101 && y < 157 && x > 43 && x < 213) pixel = black;
            if (Mathf.Abs(x - 128) < 5 && y > 38 && y < 78) pixel = gold;
            texture.SetPixel(x, y, pixel);
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size),
            new Vector2(.5f, .5f), 100f);
    }

    private static Sprite CreateFallbackStadiumArt()
    {
        const int size = 512;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color night = new Color(.008f, .012f, .02f, 1f);
        Color gold = new Color(.91f, .62f, .16f, 1f);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = x - size * .5f;
            float dy = y - size * .52f;
            float ring = Mathf.Abs(Mathf.Sqrt(dx * dx + dy * dy) - 205f);
            Color pixel = night;
            if (ring < 3f && y < 365) pixel = gold;
            if (y < 100 && Mathf.Abs(dx) < 220f) pixel = new Color(.035f, .055f, .065f, 1f);
            if (y < 72 && x > 80 && x < 432) pixel = new Color(.10f, .075f, .035f, 1f);
            bool light = ((x % 26) < 8 && (y % 28) < 8);
            if (light && y > 145 && y < 340 && (x < 100 || x > 412)) pixel = BrightGold;
            texture.SetPixel(x, y, pixel);
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size),
            new Vector2(.5f, .5f), 100f);
    }
}
