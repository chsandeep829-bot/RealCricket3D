# Gameplay scripts

This folder contains the Unity gameplay foundation for RealCricket3D.

## Systems added across stages 1–5

- **Batting timing:** `Batting/BattingController.cs` scores contact timing against a configurable sweet-spot point in the swing. It applies a quality-based hit-power multiplier and can display optional PERFECT / GOOD / MISTIMED feedback. This is a first-pass swing-time model; ball-flight prediction and per-delivery timing calibration still need playtesting.
- **Bowling control:** `Bowling/BowlingVariationController.cs` supports delivery profiles plus adjustable line and length. Connect a UI Slider to `MobileCricketControls.OnBowlingLineChanged` and another to `OnBowlingLengthChanged`.
- **Broadcast cameras:** `Gameplay/CricketBroadcastDirector.cs` switches between assigned camera rigs and can enable a ball-follow camera. Assign camera references in the Inspector; the class does not create camera rigs or cinematic replay footage.
- **Team selection and toss:** `Match/CricketMatchSetupController.cs` exposes team and format selection, a random toss, bat/bowl decisions, and match start. Connect menu controls to its public methods and assign a MatchManager.
- **Career saves:** `Match/CricketCareerSaveSystem.cs` stores a local player profile and basic match statistics in PlayerPrefs. This is device-local persistence, not cloud sync.
- **Tournament bracket:** `Match/CricketTournamentManager.cs` manages a four-team knockout bracket and stores progress locally. Call `CreateTournament`, then `RecordCurrentMatchWinner` after each completed match.
- **Match progression bridge:** `Match/CricketMatchProgressionBridge.cs` is now added automatically by the prototype scene builder. It records completed match statistics into the local career profile and passes match winners to an active tournament. The prototype treats Team A as the player's team; a future menu can call `SetPlayerTeamName` to choose another side.
- `Gameplay/CricketBall.cs`: Rigidbody-based ball state and velocity.
- `Gameplay/CricketCameraFollow.cs`: smooth camera follow for a selected player.
- `Gameplay/RunningBetweenWicketsController.cs`: queues one/two-run requests and animates batters between crease markers.
- `Gameplay/CricketPlayerAnimator.cs`: sends movement and action parameters/triggers to an assigned Animator Controller; actual animation clips and controller must be supplied separately.
- `Gameplay/CricketMatchPauseController.cs`: pauses and resumes match time.
- Fielding scripts provide basic fielder chase, catch, throw, run-out and stumping prototypes.
- `Bowling/BowlingController.cs`: creates and releases a basic delivery.
- `Batting/ShotSelectionController.cs`: separate shot selection and swipe aiming.
- `Match/MatchManager.cs`: T20, ODI and simplified four-innings Test match flow, score tracking, innings transitions, chase targets and results.
- `Match/MatchScoringRules.cs`: legal deliveries, wides, no-balls, byes, leg-byes and legal-ball wickets.
- `UI/MobileCricketControls.cs`: exposes UI actions for shot selection, bowling variations, bowling line/length and running.
- `UI/SwipeBattingInput.cs`: reads drag gestures and passes swipe direction to the shot selector.

## Existing match format defaults

- T20: 20 overs per innings.
- ODI: 50 overs per innings.
- Test prototype: four innings, with a configurable 90-over cap per innings. Declarations, follow-ons, sessions and full Test rules are not implemented.

## Unity setup

1. Open the repository as a Unity project and let Unity finish importing and compiling.
2. Use **RealCricket3D → Build Prototype Scene** from the Unity Editor menu.
3. Open `Assets/Scenes/CricketPrototype.unity` and press Play. The match setup menu appears first: change both teams if desired, choose T20/ODI/Test, press TOSS, then choose WINNER BAT or WINNER BOWL to enter the match.
4. Assign missing references in the Inspector. For timing feedback, assign a Unity UI Text to BattingController's optional Timing Feedback field.
5. Add camera rigs and assign them to CricketBroadcastDirector. Use at least three camera entries for broadcast, bowler and batting views.
6. Add CricketMatchSetupController to a menu object, assign MatchManager, and wire team selection, format, toss and start buttons.
7. Add CricketCareerSaveSystem and CricketTournamentManager to a persistent scene object. Call `RecordCompletedMatch(matchManager, playerTeamName)` from the career result screen. Call `RecordWinnerFromMatch(matchManager)` once from the tournament result screen; start each bracket with `CreateTournament(team1, team2, team3, team4)`. Assign the match setup controller and wire its public methods to actual UI controls.

**Important:** the scene generator replaces the generated prototype scene. Save manual scene changes before running it again.

## Limitations and verification

The repository is still an early prototype foundation, not a finished commercial cricket game. Player models remain placeholders unless you add original/licensed rigs and animations. The timing model, bowling line/length tuning, camera cuts, menu wiring, tournament progression and career-stat callbacks need Unity Editor playtesting. No Unity compile, Android build or device test is claimed by these source changes. Stage 5 here means core code foundations for team/toss/career/tournament, not a finished menu or full career mode.
