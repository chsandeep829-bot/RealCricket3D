# Gameplay scripts

This folder contains the Unity gameplay foundation for RealCricket3D.

## Current systems
- `Gameplay/CricketBall.cs`: Rigidbody-based ball state and velocity.
- `Gameplay/CricketCameraFollow.cs`: smooth camera follow for a selected player.
- `Bowling/BowlingController.cs`: creates and releases a delivery.
- `Batting/BattingController.cs`: swing action and ball contact.
- `Match/MatchManager.cs`: T20, ODI and simplified four-innings Test match flow, score tracking, innings transitions, chase targets and results.
- `Match/MatchScoringRules.cs`: legal deliveries, wides, no-balls, byes, leg-byes and legal-ball wickets.
- `Match/BoundaryScoringZone.cs`: awards configured boundary runs when a hit ball enters its trigger.
- `Match/WicketTrigger.cs`: records a wicket when an unhit delivery enters the wicket trigger.
- `UI/MobileCricketControls.cs`: connects Unity UI buttons to batting and bowling actions.

## Match format defaults
- T20: 20 overs per innings.
- ODI: 50 overs per innings.
- Test prototype: four innings, with a configurable 90-over cap per innings so a local test match can reach a result in a prototype session.

The format and team names can be changed on the MatchManager component in the Unity Inspector. The Test implementation is a simplified local rules model; declarations, follow-ons, session/day limits and full Test-specific rules are not implemented yet.

## Scene setup
1. Open this repository as a Unity project.
2. Let Unity finish importing and compiling scripts.
3. Use **RealCricket3D → Build Prototype Scene** from the Unity Editor menu.
4. Open `Assets/Scenes/CricketPrototype.unity` and press Play.
5. Assign the generated scene's score text and gameplay references if the builder reports any missing references.

## Important prototype limitations
This repository is still an early playable-prototype foundation, not a finished commercial cricket game. Player models are placeholders; animations, team selection, complete fielding/catching/run-outs, strike rotation, innings presentation, audio, commentary, replay, save data, online multiplayer and Android performance testing remain to be built and tested.

A milestone is complete only after its scene and assets are integrated in Unity, builds successfully, and is tested in the Unity Editor and on target Android devices. Repository scripts alone do not constitute a finished game.
