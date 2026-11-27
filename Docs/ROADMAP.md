# RealCricket3D development roadmap

## Current repository stage
Unity project structure, an Editor greybox scene generator, first-pass batting/bowling/ball scripts, basic fielding and camera scripts, and an expanded local match manager. Source foundations for the first five stages are now present; Unity integration and gameplay validation remain outstanding.

## Stage 1 — Batting and ball-contact feel
- [x] Separate selectable Loft, Stroke, Push and Leave actions.
- [x] Directional swipe aiming without silently changing shot selection.
- [x] First-pass swing timing quality and hit-power feedback.
- [ ] Ball-flight prediction, line/length-aware timing calibration and edge/miss outcomes.
- [ ] Tune contact physics and timing windows in Unity playtests.

## Stage 2 — Bowling depth
- [x] Fast, swing, off-spin, leg-spin and slow delivery profiles.
- [x] Adjustable bowling line and length controls exposed to mobile UI.
- [ ] Tune swing, seam and spin movement with realistic pitch bounce.
- [ ] Add release timing, run-up and delivery animations.

## Stage 3 — Player and fielding presentation
- [x] Reusable Animator parameter/trigger driver for player actions.
- [x] Existing prototype fielder chase, catch, throw, run-out and stumping hooks.
- [ ] Add original player rigs and authored animation clips/state machines.
- [ ] Improve fielding positioning, pickup, relay throws and wicketkeeping AI.
- [ ] Integrate animations with ball, batting and running state.

## Stage 4 — Broadcast presentation
- [x] Camera director foundation for broadcast, bowler, batting and ball-follow views.
- [x] Existing pause/resume controller.
- [ ] Build and assign original camera rigs and automatic event-driven camera cuts.
- [ ] Add score overlays, over/innings breaks, boundary/wicket celebrations and replay recording.
- [ ] Add licensed/original commentary, crowd audio, stadium art and effects.

## Stage 5 — Match modes and career foundations
- [x] Match setup controller for team selection, T20/ODI/Test selection and toss decision.
- [x] Local career profile persistence for player name and basic statistics.
- [x] Four-team knockout tournament bracket with local save.
- [x] Generated scene now creates the match setup, career-save and tournament managers together.
- [x] Completed match results are automatically passed to career statistics and an active tournament by `CricketMatchProgressionBridge` (Team A is the prototype player side).
- [x] Generated scene includes a working first-pass team/format/toss setup menu connected to match start.
- [ ] Polish the setup menu and add a dedicated match-result screen with complete callbacks.
- [ ] Integrate tournament fixtures and match results automatically.
- [ ] Expand career mode with seasons, unlocks, player progression and save migration.
- [ ] Complete Test declarations, follow-on, days and session rules.

## Stage 6 — Android release preparation
- [x] Mobile UI action methods for shot selection, bowling variations, bowling line/length and running.
- [ ] Touch-first layout and configurable sensitivity.
- [ ] Quality presets, LODs, baked lighting and performance profiling.
- [ ] Android build settings, device profiling and performance testing.
- [ ] Accessibility options, save validation and release testing.

## Stage 7 — Online features (later)
- [ ] Matchmaking and authoritative match simulation.
- [ ] Reconnect handling and fair-play/anti-cheat design.
- [ ] Online services only after offline match simulation is stable.

## Definition of done
A milestone is complete only after its scene and assets are integrated in Unity, builds successfully, and is tested in the Unity Editor and on target Android devices. Repository scripts alone do not constitute a finished game.

## Visual and gameplay target — premium 3D cricket simulation
The product target is a polished, broadcast-style mobile cricket simulation in the same broad genre as WCC3 and Real Cricket. RealCricket3D must use its own name, UI, player likenesses, stadium designs, audio, animations and other original assets; this is an inspiration target, not a recreation of proprietary content.

### Required experience
1. Core batting feel with distinct shots, timing, line/length awareness and meaningful contact outcomes.
2. Bowling depth with delivery type, line, length, pace, swing, seam and spin.
3. Rigged batters, bowlers, wicketkeepers and fielders with authored animations.
4. Broadcast cameras, score overlays, innings presentation, celebrations, replays, commentary and crowd.
5. Team selection, toss, T20/ODI/Test, tournaments, career progression and local save data.
6. Landscape touch HUD, responsive layouts, graphics presets, device profiling and robust Android builds.
7. Online play only after offline simulation is reliable, using server-authoritative match state.

### Quality gates
- Do not mark a feature complete from source code alone.
- Verify scripts compile in the project's Unity 6.6 editor.
- Play-test batting, bowling, fielding and match rules in a generated scene.
- Test touch controls and frame rate on a real Android device.
- Use original or properly licensed models, animations, sounds and textures.
