# RealCricket3D development roadmap

## Current repository stage
Unity project structure, an Editor greybox scene generator, first-pass batting/bowling/ball scripts, basic fielding and camera scripts, and an expanded local match manager.

## Milestone 1 — Playable local prototype
- [x] Unity project structure and prototype scene generator.
- [x] First-pass ball Rigidbody, batting and bowling controls.
- [x] Basic scoreboard, boundary and wicket trigger prototypes.
- [x] T20/ODI/Test format configuration and innings score tracking.
- [x] Two-innings limited-overs flow and target chase.
- [x] Simplified four-innings Test flow and final-innings target.
- [ ] Generate the scene in Unity and resolve any Editor compile/import issues.
- [ ] Play-test ball physics, scoring triggers and delivery timing.

## Milestone 2 — Cricket rules and match flow
- [x] Legal delivery, wide, no-ball, bye and leg-bye scoring entry points.
- [x] Innings transitions, target chase and basic match result.
- [x] Odd-run strike rotation and end changes after six legal balls.
- [x] Manual running-between-wickets controller with queued one/two-run requests.
- [ ] Connect running controls and batter crease markers in the generated scene.
- [ ] Add and tune catch trigger volumes and run-out wicket zones in the generated scene.
- [x] Basic caught dismissal trigger for a descending airborne hit ball.
- [x] Basic run-out trigger while batters are actively running.
- [ ] Stumping, keeper catches, throw animations and overthrow rules.
- [ ] Better wicket/no-ball/wide detection and dismissal validation.
- [ ] Test declarations, follow-on and day/session rules.

## Milestone 3 — Gameplay depth
- [ ] Shot timing windows: defend, drive, cut, pull, sweep and loft.
- [ ] Pace, swing, seam and spin bowling variations with tuned physics.
- [ ] Fielder positioning, catching, ground fielding and wicketkeeping.
- [ ] Player movement, running between wickets and replay camera.

## Milestone 4 — Presentation
- [ ] Original player rigs and animation state machines.
- [ ] Stadium art, lighting, crowd ambience, commentary and effects.
- [ ] Menus, team selection, toss, pause and match summary.
- [ ] Original high-quality 3D assets and optimized materials.

## Milestone 5 — Android release preparation
- [ ] Touch-first controls and configurable sensitivity.
- [ ] Quality presets, LODs, baked lighting and performance profiling.
- [ ] Android build settings, device profiling and performance testing.
- [ ] Save data, accessibility options and release testing.

## Milestone 6 — Online features (later)
- [ ] Matchmaking and authoritative match simulation.
- [ ] Reconnect handling and fair-play/anti-cheat design.
- [ ] Online services only after offline match simulation is stable.

## Definition of done
A milestone is complete only after its scene and assets are integrated in Unity, builds successfully, and is tested in the Unity Editor and on target Android devices. Repository scripts alone do not constitute a finished game.
