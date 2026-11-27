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

        GameObject scoreObject = new GameObject("Score", typeof(RectTransform), typeof(Text));
        scoreObject.transform.SetParent(canvas.transform, false);
        RectTransform scoreRect = scoreObject.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.02f, 0.88f);
        scoreRect.anchorMax = new Vector2(0.35f, 0.99f);
        scoreRect.offsetMin = Vector2.zero;
        scoreRect.offsetMax = Vector2.zero;
        Text scoreText = scoreObject.GetComponent<Text>();
        scoreText.fontSize = 34;
        scoreText.color = Color.white;
        scoreText.text = "0/0  0.0";
        SetObjectReference(match, "scoreText", scoreText);

        CreateSwipeArea(canvas.transform, shots);

        CreateButton(canvas.transform, "FAST", new Vector2(0.92f, 0.08f), variations.BowlFast);
        CreateButton(canvas.transform, "SPIN", new Vector2(0.80f, 0.08f), variations.BowlOffSpin);
        CreateButton(canvas.transform, "SHOT", new Vector2(0.68f, 0.08f), shots.PlaySelectedShot);
        CreateButton(canvas.transform, "RUN", new Vector2(0.56f, 0.08f), running.RequestRun);
        CreateButton(canvas.transform, "2 RUN", new Vector2(0.44f, 0.08f), running.RequestTwoRuns);
        CreateButton(canvas.transform, "DRIVE", new Vector2(0.32f, 0.08f), shots.SelectDrive);
        CreateButton(canvas.transform, "PULL", new Vector2(0.20f, 0.08f), shots.SelectPull);
        CreateButton(canvas.transform, "LOFT", new Vector2(0.18f, 0.18f), shots.SelectLofted);
        CreateButton(canvas.transform, "STROKE", new Vector2(0.40f, 0.18f), shots.SelectStroke);
        CreateButton(canvas.transform, "PUSH", new Vector2(0.62f, 0.18f), shots.SelectPush);
        CreateButton(canvas.transform, "LEAVE", new Vector2(0.84f, 0.18f), shots.SelectLeave);
        CreateButton(canvas.transform, "PAUSE / RESUME",
            new Vector2(0.94f, 0.93f), pauseController.TogglePause);
        CreateMatchSetupMenu(canvas.transform, matchSetup);
        CreatePostMatchMenu(canvas.transform, postMatch);
    }


    private static void CreateMatchSetupMenu(Transform canvas, CricketMatchSetupController setup)
    {
        GameObject panel = new GameObject("MatchSetupMenu",
            typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas, false);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.16f, 0.12f);
        panelRect.anchorMax = new Vector2(0.84f, 0.88f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.025f, 0.055f, 0.10f, 0.97f);

        CreateMenuText(panel.transform, "REALCRICKET3D  |  MATCH SETUP",
            new Vector2(0.5f, 0.88f), new Vector2(780f, 70f), 38);

        Text status = CreateMenuText(panel.transform, "INDIA vs AUSTRALIA",
            new Vector2(0.5f, 0.70f), new Vector2(780f, 60f), 30);
        SetObjectReference(setup, "setupStatus", status);

        CreateButton(panel.transform, "CHANGE TEAM A",
            new Vector2(0.27f, 0.58f), setup.SelectNextFirstTeam);
        CreateButton(panel.transform, "CHANGE TEAM B",
            new Vector2(0.73f, 0.58f), setup.SelectNextSecondTeam);

        CreateButton(panel.transform, "T20",
            new Vector2(0.25f, 0.40f), setup.SelectT20);
        CreateButton(panel.transform, "ODI",
            new Vector2(0.50f, 0.40f), setup.SelectODI);
        CreateButton(panel.transform, "TEST",
            new Vector2(0.75f, 0.40f), setup.SelectTest);

        CreateButton(panel.transform, "TOSS",
            new Vector2(0.50f, 0.23f), setup.PerformToss);
        CreateButton(panel.transform, "START 4-TEAM CUP",
            new Vector2(0.50f, 0.31f), () =>
            {
                setup.CreateFourTeamTournamentAndStart();
                panel.SetActive(false);
            });

        CreateButton(panel.transform, "WINNER BAT",
            new Vector2(0.30f, 0.08f), () =>
            {
                if (!setup.IsTossReady) return;
                setup.TossWinnerChooseBat();
                panel.SetActive(false);
            });
        CreateButton(panel.transform, "WINNER BOWL",
            new Vector2(0.70f, 0.08f), () =>
            {
                if (!setup.IsTossReady) return;
                setup.TossWinnerChooseBowl();
                panel.SetActive(false);
            });
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
        return text;
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

    private static void CreateButton(Transform parent, string label, Vector2 anchor, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(180f, 76f);
        rect.anchoredPosition = Vector2.zero;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.08f, 0.16f, 0.28f, 0.92f);
        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(action);

        GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        Text text = textObject.GetComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 28;
        text.color = Color.white;
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
