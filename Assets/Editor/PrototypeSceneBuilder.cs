#if UNITY_EDITOR
using System.IO;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PrototypeSceneBuilder
{
    private const string SceneFolder = "Assets/Scenes";
    private const string PrefabFolder = "Assets/Prefabs";
    private const string BallPrefabPath = PrefabFolder + "/CricketBall.prefab";
    private const string ScenePath = SceneFolder + "/CricketPrototype.unity";

    [MenuItem("RealCricket3D/Build Prototype Scene")]
    public static void BuildPrototypeScene()
    {
        BuildPrototypeSceneInternal(true);
    }

    public static void BuildPrototypeSceneForBuild()
    {
        BuildPrototypeSceneInternal(false);
    }

    private static void BuildPrototypeSceneInternal(bool showDialog)
    {
        EnsureFolder(SceneFolder);
        EnsureFolder(PrefabFolder);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateGround();
        CreatePitch();
        CreateStadium();
        CreateStumps(new Vector3(0f, 0f, -5.2f), "StrikerStumps");
        CreateStumps(new Vector3(0f, 0f, 6.5f), "BowlerStumps");

        GameObject batter = CreatePlayer("Batter", new Vector3(0f, 0f, -3.8f), Color.blue);
        GameObject bowler = CreatePlayer("Bowler", new Vector3(0f, 0f, 7.8f), Color.red);
        bowler.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        GameObject nonStriker = CreatePlayer("NonStriker", new Vector3(0f, 0f, 4.8f), new Color(0.15f, 0.35f, 0.85f));

        Transform bat = CreateBat(batter.transform);
        Transform shotDirection = new GameObject("ShotDirection").transform;
        shotDirection.SetParent(batter.transform);
        shotDirection.localPosition = Vector3.zero;
        shotDirection.localRotation = Quaternion.identity;

        GameObject release = new GameObject("BallReleasePoint");
        release.transform.position = new Vector3(0f, 1.2f, 6.2f);
        release.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        CricketBall ballPrefab = CreateBallPrefab();
        GameObject matchObject = new GameObject("MatchManager");
        MatchManager match = matchObject.AddComponent<MatchManager>();
        SetBoolValue(match, "startMatchOnPlay", false);

        // Keep the offline match setup, career profile and tournament save systems
        // in the generated scene so they can be wired to menus without manual creation.
        CricketCareerSaveSystem careerSave = matchObject.AddComponent<CricketCareerSaveSystem>();
        CricketTournamentManager tournament = matchObject.AddComponent<CricketTournamentManager>();
        CricketMatchSetupController matchSetup = matchObject.AddComponent<CricketMatchSetupController>();
        SetObjectReference(matchSetup, "matchManager", match);
        SetObjectReference(matchSetup, "tournamentManager", tournament);
        CricketMatchProgressionBridge progression =
            matchObject.AddComponent<CricketMatchProgressionBridge>();
        SetObjectReference(progression, "matchManager", match);
        SetObjectReference(progression, "careerSave", careerSave);
        SetObjectReference(progression, "tournament", tournament);
        CricketPostMatchMenuController postMatch =
            matchObject.AddComponent<CricketPostMatchMenuController>();
        SetObjectReference(postMatch, "matchManager", match);
        SetObjectReference(postMatch, "careerSave", careerSave);
        SetObjectReference(postMatch, "tournament", tournament);

        GameObject bowlingObject = new GameObject("BowlingController");
        BowlingController bowling = bowlingObject.AddComponent<BowlingController>();
        SetObjectReference(bowling, "ballPrefab", ballPrefab);
        SetObjectReference(bowling, "releasePoint", release.transform);

        BowlingVariationController variations = bowlingObject.AddComponent<BowlingVariationController>();
        SetObjectReference(variations, "ballPrefab", ballPrefab);
        SetObjectReference(variations, "releasePoint", release.transform);
        ConfigureBowlingProfiles(variations);

        BattingController batting = batter.AddComponent<BattingController>();
        SetObjectReference(batting, "bat", bat);
        SetObjectReference(batting, "shotDirection", shotDirection);

        ShotSelectionController shots = batter.AddComponent<ShotSelectionController>();
        SetObjectReference(shots, "battingController", batting);
        SetObjectReference(shots, "shotDirection", shotDirection);

        MatchScoringRules scoring = matchObject.AddComponent<MatchScoringRules>();
        SetObjectReference(scoring, "matchManager", match);
        CricketBallDeliveryResolver deliveryResolver =
            matchObject.AddComponent<CricketBallDeliveryResolver>();
        SetObjectReference(deliveryResolver, "matchManager", match);
        matchObject.AddComponent<CricketGraphicsSettings>();

        CreateBoundary(match);
        CreateWicketTrigger(match);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        cameraObject.transform.position = new Vector3(0f, 5f, -11f);
        cameraObject.transform.rotation = Quaternion.Euler(18f, 0f, 0f);
        CricketCameraFollow follow = cameraObject.AddComponent<CricketCameraFollow>();
        SetObjectReference(follow, "target", batter.transform);

        CreateLighting();
        RunningBetweenWicketsController running = matchObject.AddComponent<RunningBetweenWicketsController>();
        SetObjectReference(running, "strikerBatter", batter.transform);
        SetObjectReference(running, "nonStrikerBatter", nonStriker.transform);
        SetObjectReference(running, "strikerCrease", CreateCreaseMarker("StrikerCrease", new Vector3(0f, 1f, -4.5f)));
        SetObjectReference(running, "nonStrikerCrease", CreateCreaseMarker("NonStrikerCrease", new Vector3(0f, 1f, 5.5f)));
        SetObjectReference(running, "matchManager", match);
        CreateRunOutZone(new Vector3(0f, 0.8f, 6.5f), match, running);
        CreateStumpingZone(new Vector3(0f, 0.8f, -6.3f), match, running);
        CreateFielderThrowSetup(match, running);
        CreateCatchFielder("LeftSlipFielder", new Vector3(-4f, 0f, -1f), match);
        CreateCatchFielder("RightSlipFielder", new Vector3(4f, 0f, -1f), match);

        GameObject fieldingManager = new GameObject("FieldingTeamManager");
        fieldingManager.AddComponent<FielderTeamController>();

        CricketMatchPauseController pauseController =
            matchObject.AddComponent<CricketMatchPauseController>();

        CreateControls(bowling, variations, batting, shots, running, match,
            pauseController, matchSetup, postMatch);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        if (showDialog)
        {
            EditorUtility.DisplayDialog(
                "RealCricket3D",
                "Prototype scene created at " + ScenePath +
                "\nPress Play to test the greybox scene. Replace primitive models and tune physics before treating it as a finished game.",
                "OK");
        }
    }

    private static void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Outfield";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(3f, 1f, 5f);
        ground.GetComponent<Renderer>().sharedMaterial = MakeMaterial("OutfieldMaterial", new Color(0.12f, 0.36f, 0.16f));
    }

    private static void CreatePitch()
    {
        GameObject pitch = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pitch.name = "Pitch";
        pitch.transform.position = new Vector3(0f, 0.025f, 0.5f);
        pitch.transform.localScale = new Vector3(2.8f, 0.05f, 12f);
        pitch.GetComponent<Renderer>().sharedMaterial = MakeMaterial("PitchMaterial", new Color(0.62f, 0.48f, 0.31f));
    }

    private static void CreateStadium()
    {
        Material standMaterial = MakeMaterial("StadiumStandMaterial", new Color(0.025f, 0.055f, 0.12f));
        Material seatMaterial = MakeMaterial("StadiumSeatMaterial", new Color(0.08f, 0.22f, 0.38f));
        Material crowdA = MakeMaterial("CrowdBlueMaterial", new Color(0.12f, 0.32f, 0.72f));
        Material crowdB = MakeMaterial("CrowdWhiteMaterial", new Color(0.82f, 0.86f, 0.92f));
        Material crowdC = MakeMaterial("CrowdGoldMaterial", new Color(0.95f, 0.62f, 0.12f));

        // Four compact grandstand banks frame the playable oval without blocking the camera.
        CreateStandBank("NorthStand", new Vector3(0f, 3.2f, 25f),
            new Vector3(46f, 6.4f, 5f), standMaterial, seatMaterial);
        CreateStandBank("SouthStand", new Vector3(0f, 3.2f, -25f),
            new Vector3(46f, 6.4f, 5f), standMaterial, seatMaterial);
        CreateStandBank("EastStand", new Vector3(25f, 3.2f, 0f),
            new Vector3(5f, 6.4f, 42f), standMaterial, seatMaterial);
        CreateStandBank("WestStand", new Vector3(-25f, 3.2f, 0f),
            new Vector3(5f, 6.4f, 42f), standMaterial, seatMaterial);

        // Lightweight crowd blocks add stadium scale while keeping the prototype mobile-friendly.
        for (int row = 0; row < 3; row++)
        {
            for (int i = 0; i < 14; i++)
            {
                float x = -20f + i * 3f;
                Material crowdMaterial = (i % 3 == 0) ? crowdA :
                    (i % 3 == 1 ? crowdB : crowdC);
                CreateCrowdCluster("NorthCrowd_" + row + "_" + i,
                    new Vector3(x, 6.2f + row * 0.45f, 22.2f + row * 0.55f), crowdMaterial);
                CreateCrowdCluster("SouthCrowd_" + row + "_" + i,
                    new Vector3(x, 6.2f + row * 0.45f, -22.2f - row * 0.55f), crowdMaterial);
            }
        }

        CreateFloodlight("Floodlight_NW", new Vector3(-23f, 0f, 20f));
        CreateFloodlight("Floodlight_NE", new Vector3(23f, 0f, 20f));
        CreateFloodlight("Floodlight_SW", new Vector3(-23f, 0f, -20f));
        CreateFloodlight("Floodlight_SE", new Vector3(23f, 0f, -20f));

        // Boundary markers make the playable oval readable from the broadcast camera.
        for (int i = 0; i < 32; i++)
        {
            float angle = i * Mathf.PI * 2f / 32f;
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "BoundaryMarker_" + i;
            marker.transform.position = new Vector3(Mathf.Cos(angle) * 19f, 0.12f,
                Mathf.Sin(angle) * 19f);
            marker.transform.localScale = Vector3.one * 0.22f;
            marker.GetComponent<Renderer>().sharedMaterial =
                (i % 2 == 0) ? crowdB : crowdC;
        }
    }

    private static void CreateStandBank(string name, Vector3 position, Vector3 scale,
        Material bodyMaterial, Material seatMaterial)
    {
        GameObject stand = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stand.name = name;
        stand.transform.position = position;
        stand.transform.localScale = scale;
        stand.GetComponent<Renderer>().sharedMaterial = bodyMaterial;

        for (int tier = 0; tier < 3; tier++)
        {
            GameObject fascia = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fascia.name = name + "_Tier_" + tier;
            fascia.transform.position = position + Vector3.up * (-1.7f + tier * 1.7f);
            fascia.transform.localScale = new Vector3(scale.x * 1.015f, 0.16f, scale.z * 1.04f);
            fascia.GetComponent<Renderer>().sharedMaterial = seatMaterial;
        }
    }

    private static void CreateCrowdCluster(string name, Vector3 position, Material material)
    {
        GameObject crowd = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crowd.name = name;
        crowd.transform.position = position;
        crowd.transform.localScale = new Vector3(0.42f, 0.32f, 0.24f);
        crowd.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void CreateFloodlight(string name, Vector3 position)
    {
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = name + "_Pole";
        pole.transform.position = position + Vector3.up * 8f;
        pole.transform.localScale = new Vector3(0.18f, 8f, 0.18f);
        pole.GetComponent<Renderer>().sharedMaterial =
            MakeMaterial(name + "_PoleMaterial", new Color(0.18f, 0.2f, 0.24f));

        GameObject lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lamp.name = name + "_LampPanel";
        lamp.transform.position = position + new Vector3(0f, 16f, 0f);
        lamp.transform.localScale = new Vector3(2.4f, 0.9f, 0.35f);
        lamp.GetComponent<Renderer>().sharedMaterial =
            MakeMaterial(name + "_LampMaterial", new Color(0.88f, 0.94f, 1f));

        Light floodlight = lamp.AddComponent<Light>();
        floodlight.type = LightType.Point;
        floodlight.range = 32f;
        floodlight.intensity = 3f;
        floodlight.color = new Color(0.78f, 0.86f, 1f);
    }

    private static GameObject CreatePlayer(string name, Vector3 position, Color color)
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = name;
        player.transform.position = position + Vector3.up;
        player.transform.localScale = new Vector3(0.65f, 1f, 0.65f);
        player.GetComponent<Renderer>().sharedMaterial = MakeMaterial(name + "Material", color);
        return player;
    }

    private static Transform CreateBat(Transform parent)
    {
        GameObject bat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bat.name = "Bat";
        bat.transform.SetParent(parent);
        bat.transform.localPosition = new Vector3(0.42f, 0.8f, 0.45f);
        bat.transform.localRotation = Quaternion.Euler(0f, 0f, -12f);
        bat.transform.localScale = new Vector3(0.14f, 0.95f, 0.32f);
        bat.GetComponent<Renderer>().sharedMaterial = MakeMaterial("BatMaterial", new Color(0.55f, 0.28f, 0.1f));
        return bat.transform;
    }

    private static void CreateStumps(Vector3 position, string name)
    {
        GameObject root = new GameObject(name);
        root.transform.position = position;
        for (int i = -1; i <= 1; i++)
        {
            GameObject stump = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stump.name = "Stump";
            stump.transform.SetParent(root.transform);
            stump.transform.localPosition = new Vector3(i * 0.18f, 0.55f, 0f);
            stump.transform.localScale = new Vector3(0.035f, 0.55f, 0.035f);
            stump.GetComponent<Renderer>().sharedMaterial = MakeMaterial("StumpMaterial", Color.white);
        }
    }

    private static CricketBall CreateBallPrefab()
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "CricketBall";
        ball.transform.localScale = Vector3.one * 0.14f;
        ball.GetComponent<Renderer>().sharedMaterial = MakeMaterial("BallMaterial", new Color(0.55f, 0.04f, 0.04f));

        Rigidbody body = ball.AddComponent<Rigidbody>();
        body.mass = 0.16f;
        body.drag = 0.02f;
        body.angularDrag = 0.05f;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.interpolation = RigidbodyInterpolation.Interpolate;

        CricketBall cricketBall = ball.AddComponent<CricketBall>();
        SetObjectReference(cricketBall, "body", body);
        PrefabUtility.SaveAsPrefabAsset(ball, BallPrefabPath);
        Object.DestroyImmediate(ball);
        return AssetDatabase.LoadAssetAtPath<CricketBall>(BallPrefabPath);
    }

    private static void CreateBoundary(MatchManager match)
    {
        GameObject boundary = new GameObject("BoundaryScoringTrigger");
        boundary.transform.position = new Vector3(0f, 2f, 23f);
        BoxCollider trigger = boundary.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(60f, 8f, 1f);
        BoundaryScoringZone zone = boundary.AddComponent<BoundaryScoringZone>();
        SetObjectReference(zone, "matchManager", match);
    }

    private static void CreateWicketTrigger(MatchManager match)
    {
        GameObject wicket = new GameObject("StrikerWicketTrigger");
        wicket.transform.position = new Vector3(0f, 0.8f, -5.2f);
        BoxCollider trigger = wicket.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(1.1f, 1.6f, 0.35f);
        WicketTrigger wicketTrigger = wicket.AddComponent<WicketTrigger>();
        SetObjectReference(wicketTrigger, "matchManager", match);
    }

    private static void CreateControls(BowlingController bowling, BowlingVariationController variations,
        BattingController batting, ShotSelectionController shots,
        RunningBetweenWicketsController running, MatchManager match,
        CricketMatchPauseController pauseController, CricketMatchSetupController matchSetup,
        CricketPostMatchMenuController postMatch)
    {
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        GameObject canvasObject = new GameObject("GameHUD", typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CricketSafeArea));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GameObject controlsRoot = new GameObject("GameplayControls", typeof(RectTransform));
        controlsRoot.transform.SetParent(canvas.transform, false);
        RectTransform controlsRect = controlsRoot.GetComponent<RectTransform>();
        controlsRect.anchorMin = Vector2.zero;
        controlsRect.anchorMax = Vector2.one;
        controlsRect.offsetMin = Vector2.zero;
        controlsRect.offsetMax = Vector2.zero;
        controlsRoot.SetActive(false);

        GameObject scoreObject = new GameObject("Score", typeof(RectTransform), typeof(Text));
        scoreObject.transform.SetParent(controlsRoot.transform, false);
        RectTransform scoreRect = scoreObject.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.02f, 0.86f);
        scoreRect.anchorMax = new Vector2(0.42f, 0.99f);
        scoreRect.offsetMin = new Vector2(8f, 0f);
        scoreRect.offsetMax = Vector2.zero;
        Text scoreText = scoreObject.GetComponent<Text>();
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 30;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.alignment = TextAnchor.UpperLeft;
        scoreText.color = Color.white;
        scoreText.text = "TEAM A  0/0";
        SetObjectReference(match, "scoreText", scoreText);

        CreateSwipeArea(controlsRoot.transform, shots);

        CreateButton(controlsRoot.transform, "FAST", new Vector2(0.92f, 0.08f), variations.BowlFast);
        CreateButton(controlsRoot.transform, "SPIN", new Vector2(0.80f, 0.08f), variations.BowlOffSpin);
        CreateButton(controlsRoot.transform, "SHOT", new Vector2(0.68f, 0.08f), shots.PlaySelectedShot);
        CreateButton(controlsRoot.transform, "RUN", new Vector2(0.56f, 0.08f), running.RequestRun);
        CreateButton(controlsRoot.transform, "2 RUN", new Vector2(0.44f, 0.08f), running.RequestTwoRuns);
        CreateButton(controlsRoot.transform, "DRIVE", new Vector2(0.32f, 0.08f), shots.SelectDrive);
        CreateButton(controlsRoot.transform, "PULL", new Vector2(0.20f, 0.08f), shots.SelectPull);
        CreateButton(controlsRoot.transform, "LOFT", new Vector2(0.18f, 0.18f), shots.SelectLofted);
        CreateButton(controlsRoot.transform, "STROKE", new Vector2(0.40f, 0.18f), shots.SelectStroke);
        CreateButton(controlsRoot.transform, "PUSH", new Vector2(0.62f, 0.18f), shots.SelectPush);
        CreateButton(controlsRoot.transform, "LEAVE", new Vector2(0.84f, 0.18f), shots.SelectLeave);
        CreateButton(controlsRoot.transform, "PAUSE / RESUME",
            new Vector2(0.94f, 0.93f), pauseController.TogglePause);

        CreateMatchSetupMenu(canvas.transform, matchSetup, controlsRoot);
        CreatePauseMenu(canvas.transform, pauseController);
        CreatePostMatchMenu(canvas.transform, postMatch);
    }

    private static void CreateMatchSetupMenu(Transform canvas, CricketMatchSetupController setup,
        GameObject controlsRoot)
    {
        // A full-screen modal prevents the gameplay HUD from showing or receiving
        // touches behind the setup menu while the player chooses a match.
        GameObject overlay = new GameObject("MatchSetupMenu",
            typeof(RectTransform), typeof(Image));
        overlay.transform.SetParent(canvas, false);
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        Image overlayImage = overlay.GetComponent<Image>();
        overlayImage.color = new Color(0.008f, 0.018f, 0.035f, 0.94f);
        overlayImage.raycastTarget = true;

        GameObject card = new GameObject("SetupCard", typeof(RectTransform), typeof(Image));
        card.transform.SetParent(overlay.transform, false);
        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.07f, 0.07f);
        cardRect.anchorMax = new Vector2(0.93f, 0.93f);
        cardRect.offsetMin = Vector2.zero;
        cardRect.offsetMax = Vector2.zero;
        card.GetComponent<Image>().color = new Color(0.018f, 0.045f, 0.078f, 1f);

        GameObject goldRule = new GameObject("GoldTopRule", typeof(RectTransform), typeof(Image));
        goldRule.transform.SetParent(card.transform, false);
        RectTransform ruleRect = goldRule.GetComponent<RectTransform>();
        ruleRect.anchorMin = new Vector2(0f, 0.992f);
        ruleRect.anchorMax = new Vector2(1f, 1f);
        ruleRect.offsetMin = Vector2.zero;
        ruleRect.offsetMax = Vector2.zero;
        goldRule.GetComponent<Image>().color = new Color(0.86f, 0.62f, 0.18f, 1f);

        GameObject logoObject = new GameObject("REAL_CRICKET_3D_Golden_Shield", typeof(RectTransform), typeof(Image));
        logoObject.transform.SetParent(card.transform, false);
        RectTransform logoRect = logoObject.GetComponent<RectTransform>();
        logoRect.anchorMin = new Vector2(0.5f, 0.965f);
        logoRect.anchorMax = new Vector2(0.5f, 0.965f);
        logoRect.sizeDelta = new Vector2(62f, 62f);
        Image logoImage = logoObject.GetComponent<Image>();
        logoImage.sprite = RealCricketLogo.CreateSprite();
        logoImage.preserveAspect = true;
        logoImage.raycastTarget = false;

        CreateMenuText(card.transform, "REAL CRICKET 3D  |  MATCH SETUP",
            new Vector2(0.5f, 0.88f), new Vector2(1100f, 74f), 38);

        Text status = CreateMenuText(card.transform, "INDIA vs AUSTRALIA  •  T20",
            new Vector2(0.5f, 0.75f), new Vector2(1100f, 62f), 28);
        status.color = new Color(1f, 0.78f, 0.28f, 1f);
        SetObjectReference(setup, "setupStatus", status);

        CreateMenuText(card.transform, "SELECT TEAMS",
            new Vector2(0.5f, 0.65f), new Vector2(500f, 34f), 19);
        Button batButton = null;
        Button bowlButton = null;
        CreateButton(card.transform, "CHANGE TEAM A",
            new Vector2(0.25f, 0.56f), () => { setup.SelectNextFirstTeam(); if (batButton != null) batButton.interactable = false; if (bowlButton != null) bowlButton.interactable = false; },
            new Vector2(330f, 76f), new Color(0.045f, 0.13f, 0.22f, 1f), 22);
        CreateButton(card.transform, "CHANGE TEAM B",
            new Vector2(0.75f, 0.56f), () => { setup.SelectNextSecondTeam(); if (batButton != null) batButton.interactable = false; if (bowlButton != null) bowlButton.interactable = false; },
            new Vector2(330f, 76f), new Color(0.045f, 0.13f, 0.22f, 1f), 22);

        CreateMenuText(card.transform, "MATCH FORMAT",
            new Vector2(0.5f, 0.43f), new Vector2(500f, 34f), 19);
        CreateButton(card.transform, "T20",
            new Vector2(0.25f, 0.35f), () => { setup.SelectT20(); if (batButton != null) batButton.interactable = false; if (bowlButton != null) bowlButton.interactable = false; },
            new Vector2(230f, 68f), new Color(0.12f, 0.16f, 0.23f, 1f), 23);
        CreateButton(card.transform, "ODI",
            new Vector2(0.50f, 0.35f), () => { setup.SelectODI(); if (batButton != null) batButton.interactable = false; if (bowlButton != null) bowlButton.interactable = false; },
            new Vector2(230f, 68f), new Color(0.12f, 0.16f, 0.23f, 1f), 23);
        CreateButton(card.transform, "TEST",
            new Vector2(0.75f, 0.35f), () => { setup.SelectTest(); if (batButton != null) batButton.interactable = false; if (bowlButton != null) bowlButton.interactable = false; },
            new Vector2(230f, 68f), new Color(0.12f, 0.16f, 0.23f, 1f), 23);

        CreateButton(card.transform, "TOSS",
            new Vector2(0.50f, 0.22f), () =>
            {
                setup.PerformToss();
                bool tossReady = setup.IsTossReady;
                if (batButton != null) batButton.interactable = tossReady;
                if (bowlButton != null) bowlButton.interactable = tossReady;
            },
            new Vector2(280f, 70f), new Color(0.72f, 0.48f, 0.10f, 1f), 24);
        CreateButton(card.transform, "START 4-TEAM CUP",
            new Vector2(0.50f, 0.115f), () =>
            {
                if (!setup.CanStartTournament) return;
                setup.CreateFourTeamTournamentAndStart();
                overlay.SetActive(false);
                controlsRoot.SetActive(true);
            },
            new Vector2(390f, 66f), new Color(0.08f, 0.28f, 0.25f, 1f), 21);

        batButton = CreateButton(card.transform, "WINNER BAT",
            new Vector2(0.28f, 0.045f), () =>
            {
                if (!setup.IsTossReady || !setup.CanStartMatch) return;
                setup.TossWinnerChooseBat();
                overlay.SetActive(false);
                controlsRoot.SetActive(true);
            },
            new Vector2(300f, 62f), new Color(0.08f, 0.28f, 0.20f, 1f), 21);
        bowlButton = CreateButton(card.transform, "WINNER BOWL",
            new Vector2(0.72f, 0.045f), () =>
            {
                if (!setup.IsTossReady || !setup.CanStartMatch) return;
                setup.TossWinnerChooseBowl();
                overlay.SetActive(false);
                controlsRoot.SetActive(true);
            },
            new Vector2(300f, 62f), new Color(0.12f, 0.20f, 0.34f, 1f), 21);
        batButton.interactable = false;
        bowlButton.interactable = false;
    }

    private static Text CreateMenuText(Transform parent, string value, Vector2 anchor,
        Vector2 size, int fontSize)
    {
        GameObject textObject = new GameObject("MenuText", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        Text text = textObject.GetComponent<Text>();
        text.text = value;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.raycastTarget = false;
        return text;
    }

    private static void CreatePauseMenu(Transform canvas, CricketMatchPauseController controller)
    {
        GameObject panel = new GameObject("PauseMenu",
            typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.005f, 0.008f, 0.014f, 0.88f);

        GameObject card = new GameObject("PauseCard", typeof(RectTransform), typeof(Image));
        card.transform.SetParent(panel.transform, false);
        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.30f, 0.24f);
        cardRect.anchorMax = new Vector2(0.70f, 0.76f);
        cardRect.offsetMin = Vector2.zero;
        cardRect.offsetMax = Vector2.zero;
        card.GetComponent<Image>().color = new Color(0.025f, 0.045f, 0.075f, 1f);

        CreateMenuText(card.transform, "MATCH PAUSED",
            new Vector2(0.5f, 0.70f), new Vector2(700f, 90f), 40);
        CreateButton(card.transform, "RESUME MATCH",
            new Vector2(0.5f, 0.38f), controller.Resume,
            new Vector2(360f, 76f), new Color(0.72f, 0.52f, 0.16f, 1f), 25);

        SetObjectReference(controller, "pausePanel", panel);
        panel.SetActive(false);
    }

    private static void CreatePostMatchMenu(Transform canvas, CricketPostMatchMenuController controller)
    {
        GameObject panel = new GameObject("PostMatchResultPanel",
            typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.12f, 0.20f);
        rect.anchorMax = new Vector2(0.88f, 0.80f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.015f, 0.035f, 0.07f, 0.98f);

        Text result = CreateMenuText(panel.transform, "MATCH RESULT",
            new Vector2(0.5f, 0.78f), new Vector2(1000f, 100f), 38);
        Text career = CreateMenuText(panel.transform, "CAREER SUMMARY",
            new Vector2(0.5f, 0.48f), new Vector2(1100f, 130f), 26);
        SetObjectReference(controller, "resultPanel", panel);
        SetObjectReference(controller, "resultText", result);
        SetObjectReference(controller, "careerText", career);

        CreateButton(panel.transform, "NEXT TOURNAMENT MATCH",
            new Vector2(0.35f, 0.16f), controller.StartNextTournamentFixture);
        CreateButton(panel.transform, "CLOSE",
            new Vector2(0.70f, 0.16f), controller.CloseResultPanel);
        panel.SetActive(false);
    }

    private static void CreateSwipeArea(Transform parent, ShotSelectionController shots)
    {
        GameObject swipeObject = new GameObject("SwipeBattingArea",
            typeof(RectTransform), typeof(Image), typeof(SwipeBattingInput));
        swipeObject.transform.SetParent(parent, false);

        RectTransform rect = swipeObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.04f, 0.20f);
        rect.anchorMax = new Vector2(0.96f, 0.82f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = swipeObject.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.01f);
        image.raycastTarget = true;

        SwipeBattingInput swipeInput = swipeObject.GetComponent<SwipeBattingInput>();
        SetObjectReference(swipeInput, "shotSelection", shots);
    }

    private static Button CreateButton(Transform parent, string label, Vector2 anchor, UnityEngine.Events.UnityAction action)
    {
        return CreateButton(parent, label, anchor, action, new Vector2(180f, 76f),
            new Color(0.045f, 0.13f, 0.22f, 0.96f), 24);
    }

    private static Button CreateButton(Transform parent, string label, Vector2 anchor,
        UnityEngine.Events.UnityAction action, Vector2 size, Color buttonColor, int fontSize)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        Image image = buttonObject.GetComponent<Image>();
        image.color = buttonColor;
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8f, 4f);
        textRect.offsetMax = new Vector2(-8f, -4f);
        Text text = textObject.GetComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 14;
        text.resizeTextMaxSize = fontSize;
        text.raycastTarget = false;
        return button;
    }

    private static void CreateRunOutZone(Vector3 position, MatchManager match,
        RunningBetweenWicketsController running)
    {
        GameObject zone = new GameObject("NonStrikerRunOutZone");
        zone.transform.position = position;
        BoxCollider collider = zone.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(1.3f, 1.8f, 0.6f);
        RunOutWicketZone runOut = zone.AddComponent<RunOutWicketZone>();
        SetObjectReference(runOut, "matchManager", match);
        SetObjectReference(runOut, "runningController", running);
    }

    private static void CreateStumpingZone(Vector3 position, MatchManager match,
        RunningBetweenWicketsController running)
    {
        GameObject zone = new GameObject("WicketkeeperStumpingZone");
        zone.transform.position = position;
        BoxCollider collider = zone.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(1.5f, 1.8f, 0.8f);
        StumpingWicketZone stumping = zone.AddComponent<StumpingWicketZone>();
        SetObjectReference(stumping, "matchManager", match);
        SetObjectReference(stumping, "runningController", running);
    }

    private static void CreateCatchFielder(string name, Vector3 position, MatchManager match)
    {
        GameObject fielder = CreatePlayer(name, position, new Color(0.9f, 0.72f, 0.12f));
        FielderAI fielderAI = fielder.AddComponent<FielderAI>();
        SetFloatValue(fielderAI, "groundHeight", 1f);
        GameObject catchArea = new GameObject("CatchTrigger");
        catchArea.transform.SetParent(fielder.transform);
        catchArea.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        SphereCollider trigger = catchArea.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.25f;
        FielderCatchZone catchZone = catchArea.AddComponent<FielderCatchZone>();
        SetObjectReference(catchZone, "matchManager", match);
    }

    private static void CreateFielderThrowSetup(MatchManager match,
        RunningBetweenWicketsController running)
    {
        GameObject fielder = CreatePlayer("PrototypeFielder", new Vector3(4f, 0f, 0f), Color.yellow);
        FielderAI fielderAI = fielder.AddComponent<FielderAI>();
        SetFloatValue(fielderAI, "groundHeight", 1f);
        GameObject pickup = new GameObject("FielderPickupTrigger");
        pickup.transform.SetParent(fielder.transform);
        pickup.transform.localPosition = Vector3.zero;
        SphereCollider trigger = pickup.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.4f;
        FielderThrowController thrower = pickup.AddComponent<FielderThrowController>();
        SetObjectReference(thrower, "throwTarget", GameObject.Find("BowlerStumps").transform);
        SetObjectReference(thrower, "runningController", running);
    }

    private static Transform CreateCreaseMarker(string name, Vector3 position)
    {
        GameObject marker = new GameObject(name);
        marker.transform.position = position;
        return marker.transform;
    }

    private static void ConfigureBowlingProfiles(BowlingVariationController controller)
    {
        SerializedObject serialized = new SerializedObject(controller);
        SerializedProperty profiles = serialized.FindProperty("profiles");
        profiles.arraySize = 5;
        float[] speeds = { 26f, 24f, 17f, 18f, 20f };
        Vector3[] spins =
        {
            Vector3.zero,
            new Vector3(0f, 0f, 0.8f),
            new Vector3(0f, 8f, 0f),
            new Vector3(0f, -8f, 0f),
            Vector3.zero
        };
        float[] variation = { 0.01f, 0.08f, 0.04f, 0.04f, 0.02f };
        for (int i = 0; i < profiles.arraySize; i++)
        {
            SerializedProperty item = profiles.GetArrayElementAtIndex(i);
            item.FindPropertyRelative("type").enumValueIndex = i;
            item.FindPropertyRelative("speed").floatValue = speeds[i];
            item.FindPropertyRelative("spin").vector3Value = spins[i];
            item.FindPropertyRelative("variationAmount").floatValue = variation[i];
        }
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateLighting()
    {
        GameObject lightObject = new GameObject("Directional Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.15f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static Material MakeMaterial(string name, Color color)
    {
        string path = "Assets/Materials";
        EnsureFolder(path);
        string materialPath = path + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, materialPath);
        }
        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }


    private static void SetBoolValue(Object target, string propertyName, bool value)
    {
        SerializedObject serialized = new SerializedObject(target);
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property == null)
        {
            Debug.LogWarning("Serialized bool not found: " + propertyName + " on " + target.name);
            return;
        }
        property.boolValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloatValue(Object target, string propertyName, float value)
    {
        SerializedObject serialized = new SerializedObject(target);
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property == null)
        {
            Debug.LogWarning("Serialized float not found: " + propertyName + " on " + target.name);
            return;
        }
        property.floatValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetObjectReference(Object target, string propertyName, Object value)
    {
        SerializedObject serialized = new SerializedObject(target);
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property == null)
        {
            Debug.LogWarning("Serialized field not found: " + propertyName + " on " + target.name);
            return;
        }
        property.objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        string folder = Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, folder);
    }
}
#endif
