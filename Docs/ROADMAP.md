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
- [x] Generate two batter placeholders, crease markers and one/two-run controls in the prototype scene.
- [x] Generate basic catch zones, a non-striker run-out zone and a keeper stumping zone.
- [ ] Play-test and tune all trigger zones and running physics in Unity.
- [ ] Add and tune catch trigger volumes and run-out wicket zones in the generated scene.
- [x] Basic caught dismissal trigger for a descending airborne hit ball.
- [x] Basic run-out trigger while batters are actively running.
- [x] Basic keeper stumping trigger for an unhit ball reaching the keeper while striker is outside the crease tolerance.
- [ ] Keeper catches, throw animations and overthrow rules.
- [ ] Better wicket/no-ball/wide detection and dismissal validation.
- [ ] Test declarations, follow-on and day/session rules.

## Milestone 3 — Gameplay depth
- [ ] Shot timing windows: defend, drive, cut, pull, sweep and loft.
- [ ] Pace, swing, seam and spin bowling variations with tuned physics.
- [x] Fielder pickup trigger and throw toward wicket for run-out attempts.
- [ ] Fielder positioning, ground fielding and wicketkeeping AI.
- [ ] Player movement, running between wickets and replay camera.

## Milestone 4 — Presentation
- [x] Reusable Animator parameter/trigger driver for player actions.
- [ ] Original player rigs, animation clips and complete animation state machines.
- [ ] Stadium art, lighting, crowd ambience, commentary and effects.
- [x] Pause/resume controller with optional pause panel.
- [ ] Menus, team selection, toss and match summary.
- [ ] Original high-quality 3D assets and optimized materials.

## Milestone 5 — Android release preparation
- [x] Mobile UI action methods for shot selection, bowling variations and running.
- [ ] Touch-first layout and configurable sensitivity.
- [ ] Quality presets, LODs, baked lighting and performance profiling.
- [ ] Android build settings, device profiling and performance testing.
- [ ] Save data, accessibility options and release testing.

## Milestone 6 — Online features (later)
- [ ] Matchmaking and authoritative match simulation.
- [ ] Reconnect handling and fair-play/anti-cheat design.
- [ ] Online services only after offline match simulation is stable.

## Definition of done
A milestone is complete only after its scene and assets are integrated in Unity, builds successfully, and is tested in the Unity Editor and on target Android devices. Repository scripts alone do not constitute a finished game.

## Visual and gameplay target — premium 3D cricket simulation

The product target is a polished, broadcast-style mobile cricket simulation in the same broad genre as WCC3. RealCricket3D must use its own name, UI, player likenesses, stadium designs, audio, animations and other original assets; this is an inspiration target, not a recreation of proprietary WCC3 content.

### Required experience, in priority order
1. **Core batting feel:** separate selectable Loft, Stroke, Push and Leave actions; additional grounded/aerial shot types; timing windows based on ball line, length and arrival time; directional swipe aiming that never silently changes the selected shot; meaningful edge, miss, mistime and clean-hit outcomes.
2. **Bowling depth:** choose delivery type, line, length, pace and release; implement swing/seam/spin movement with readable flight and bounce.
3. **Player presentation:** rigged 3D batters, bowlers, wicketkeepers and fielders with authored run-up, delivery, batting, catch, dive, throw and appeal animations.
4. **Match broadcast:** camera cuts and follow cameras, score overlays, over/innings breaks, wickets and boundary celebrations, replay system, commentary and crowd audio.
5. **Full match modes:** team selection, toss, T20/ODI/Test rules, tournaments, career progression and local save data.
6. **Mobile polish:** landscape touch HUD, responsive layouts, graphics presets, device performance profiling and robust Android builds.
7. **Online play:** only after offline simulation is reliable; use server-authoritative match state and reconnect handling.

### Quality gates
- Do not mark a feature complete from source code alone.
- Verify scripts compile in the project's Unity 6.6 editor.
- Play-test batting, bowling, fielding and match rules in a generated scene.
- Test touch controls and frame rate on a real Android device.
- Use original or properly licensed models, animations, sounds and textures.
