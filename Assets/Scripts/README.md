# Gameplay scripts

This folder contains the Unity gameplay foundation for RealCricket3D.

## Current systems
- `Gameplay/CricketBall.cs`: Rigidbody-based ball state and velocity.
- `Gameplay/CricketCameraFollow.cs`: smooth camera follow for a selected player.
- `Gameplay/RunningBetweenWicketsController.cs`: queues one/two-run requests, animates the two batter transforms between crease markers and awards runs only after each completed crossing.
- `Gameplay/CricketPlayerAnimator.cs`: sends movement and action parameters/triggers to an assigned Animator Controller; actual animation clips and rig mappings must be supplied separately.
- `Gameplay/CricketMatchPauseController.cs`: pauses and resumes match time and toggles an optional pause panel.
- `Fielding/FielderThrowController.cs`: detects a hit ball at a fielder pickup trigger and throws it toward an assigned target; it can be restricted to active running plays.\n- `Fielding/FielderTeamController.cs`: assigns each newly hit ball to the nearest available fielder, with periodic scanning and optional automatic scene discovery.
- `Fielding/StumpingWicketZone.cs`: records a stumping when an unhit delivery reaches the keeper zone while the striker is outside the configured crease tolerance.
- `Fielding/FielderCatchZone.cs`: detects a descending airborne hit ball inside a fielder catch volume and records a caught dismissal once.
- `Fielding/RunOutWicketZone.cs`: detects a hit ball reaching a wicket zone while batters are running, cancels queued runs and records a run-out.
- `Fielding/StumpingWicketZone.cs`: records a stumping when an unhit delivery reaches the keeper zone while the striker is outside the configured crease tolerance.
- `Bowling/BowlingController.cs`: creates and releases a delivery.
- `Batting/BattingController.cs`: swing action and ball contact.
- `Match/MatchManager.cs`: T20, ODI and simplified four-innings Test match flow, score tracking, innings transitions, chase targets and results.
- `Match/MatchScoringRules.cs`: legal deliveries, wides, no-balls, byes, leg-byes and legal-ball wickets.
- `Match/MatchManager.cs`: tracks odd-run strike rotation and end changes after six legal balls.
- `Match/BoundaryScoringZone.cs`: awards configured boundary runs when a hit ball enters its trigger.
- `Match/WicketTrigger.cs`: records a wicket when an unhit delivery enters the wicket trigger.
- `UI/MobileCricketControls.cs`: exposes UI actions for shot selection, batting, pace/spin bowling variations and running.

## Match format defaults
- T20: 20 overs per innings.
- ODI: 50 overs per innings.
- Test prototype: four innings, with a configurable 90-over cap per innings so a local test match can reach a result in a prototype session.

The format and team names can be changed on the MatchManager component in the Unity Inspector. The scoreboard uses Unity's built-in UI Text component, so TextMeshPro is not required. The Test implementation is a simplified local rules model; declarations, follow-ons, session/day limits and full Test-specific rules are not implemented yet.

## Scene setup
1. Open this repository as a Unity project.
2. Let Unity finish importing and compiling scripts.
3. Use **RealCricket3D → Build Prototype Scene** from the Unity Editor menu.
4. Open `Assets/Scenes/CricketPrototype.unity` and press Play. The scene generator creates placeholder batters, a bowler, configured fast/spin deliveries, run controls, basic catch zones, a run-out zone and a keeper stumping zone. The updated builder also creates two batters, running controls, fast/spin bowling controls, basic catch volumes, a run-out zone and a keeper stumping zone.
5. Assign the generated scene's score text and gameplay references if the builder reports any missing references.
6. To enable running, add `RunningBetweenWicketsController` to a scene object; assign both batter transforms, the two crease marker transforms and `MatchManager`. Connect mobile Run buttons to `RequestRun` or `RequestTwoRuns`. Set `runSpeed` to tune movement.
7. For catches, add a trigger collider and `FielderCatchZone` to each fielder's catch area. Assign `MatchManager` and tune the minimum/maximum catch heights. The ball must be hit and descending.
8. For run-outs, add a trigger collider at the stumps and attach `RunOutWicketZone`. Assign `MatchManager` and the active `RunningBetweenWicketsController`. The ball must reach the wicket while a run is in progress.

## Important prototype limitations
This repository is still an early playable-prototype foundation, not a finished commercial cricket game. Player models are placeholders; animations, team selection, complete fielding animations, stumping, throw animations, innings presentation, audio, commentary, replay, save data, online multiplayer and Android performance testing remain to be built and tested.

A milestone is complete only after its scene and assets are integrated in Unity, builds successfully, and is tested in the Unity Editor and on target Android devices. Repository scripts alone do not constitute a finished game.

## Automatic fielder chase setup\n\n1. Add an empty GameObject named `FieldingTeamManager` to the scene.\n2. Add the `FielderTeamController` component.\n3. Add `FielderAI` to each fielding player and tune movement speed and pickup distance.\n4. Leave `Discover Fielders On Start` enabled to find scene fielders automatically, or disable it and assign the team array manually.\n5. During play, the nearest available fielder will chase each newly hit ball. This prototype does not yet animate a pickup, return throw or relay automatically; those remain separate systems.\n\n## Additional scene wiring

- Add `FielderThrowController` to a trigger collider around a fielder's pickup area. Assign the wicket target and, when required, the running controller. The wicket's `RunOutWicketZone` must be configured separately.
- Add `CricketPlayerAnimator` to a rig with an Animator Controller. The controller must define the configured float and trigger parameters. This script does not create animation clips or a rig.
- Add `CricketMatchPauseController` to a scene object, assign an optional pause panel, and connect Pause/Resume buttons to its public methods.
- `MobileCricketControls` exposes methods for each shot, bowling variation, and running action. Assign the relevant scene components in the Inspector before wiring UI buttons.

## Scoreboard improvements
- Displays the current run rate (CRR) using completed legal balls.
- During a chase, displays the target, runs still required, and required run rate (RRR).
- Uses actual newline characters so each scoreboard item appears on its own line.
