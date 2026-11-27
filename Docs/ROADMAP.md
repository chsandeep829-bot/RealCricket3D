# RealCricket3D development roadmap

## Current repository stage
Starter Unity C# gameplay foundation: ball Rigidbody control, basic bowling and batting actions, a simple innings score manager, boundary and wicket trigger prototypes, mobile button bridge and camera follow.

## Milestone 1 — Playable local prototype
- Create Unity 3D Core project and a test ground scene.
- Add batter, bowler, stumps, pitch and ball prefabs.
- Wire batting, bowling, scoreboard, boundary and wicket triggers.
- Add legal-delivery tracking and a basic innings reset flow.
- Test ball physics and tune scale, gravity, bounce and friction.

## Milestone 2 — Cricket rules
- Legal and illegal deliveries, wides and no-balls.
- Byes, leg-byes, boundaries, overthrows and run-outs.
- Strike rotation, over changes, innings transitions and target chasing.
- T20, ODI and Test match configuration.

## Milestone 3 — Gameplay depth
- Shot selection and timing windows: defend, drive, cut, pull, sweep and loft.
- Pace, swing, seam and spin bowling variations.
- Fielder positioning, catching, ground fielding and wicketkeeping.
- Replay camera and match highlights.

## Milestone 4 — Presentation
- Original player, bat, ball, pitch and stadium assets.
- Player rigs, batting/bowling/fielding animations and blend trees.
- Stadium lighting, crowd ambience, commentary and effects.
- Menus, team selection, settings, pause and match summary.

## Milestone 5 — Android release preparation
- Touch-first controls and configurable sensitivity.
- Quality presets, LODs, baked lighting and optimized textures.
- Android build settings, device profiling and performance testing.
- Save data, accessibility options and release testing.

## Milestone 6 — Online features (later)
- Matchmaking, authoritative match simulation, reconnect handling and anti-cheat design.
- Online services should be added only after offline match simulation is stable.

## Definition of done
A milestone is complete only after its scene and assets are integrated in Unity, builds successfully, and is tested in the Unity Editor and on target Android devices. Repository scripts alone do not constitute a finished game.
