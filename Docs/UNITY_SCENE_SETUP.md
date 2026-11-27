# Unity scene assembly guide

This guide assembles the scripts into a basic local test scene. It does not supply 3D art assets or a finished match scene.

## 1. Create the project
- Install Unity Hub and a supported Unity Editor with Android Build Support if Android is the target.
- Create a **3D Core** project named `RealCricket3D`.
- Copy the repository's `Assets/Scripts` directory into the Unity project's `Assets` directory.
- Open the project and wait for Unity to compile scripts. Resolve any compile errors before continuing.

## 2. Block out the cricket ground
- Use scaled cubes and planes as temporary greybox geometry.
- Create a flat pitch, outfield, boundary marker, two sets of stumps and a simple camera.
- Use Unity's meter-scale convention consistently. Keep the ball small relative to the pitch.
- Add colliders to the pitch and stumps. Do not use trigger colliders for solid ground.

## 3. Ball prefab
- Create a sphere, scale it to a cricket ball size appropriate for the chosen scene scale.
- Add Sphere Collider, Rigidbody and `CricketBall`.
- Set Rigidbody collision detection to Continuous Dynamic and interpolation to Interpolate.
- Drag the object into a Prefabs folder to create a prefab, then remove the scene copy if not needed.

## 4. Bowling
- Create an empty GameObject at the bowler's release point, oriented down the pitch.
- Add `BowlingController` or `BowlingVariationController` to a controller object.
- Assign the ball prefab and release-point Transform.
- For BowlingVariationController, add profiles for Fast, Swing, OffSpin, LegSpin and Slow, then tune speed and spin in Play Mode.

## 5. Batting and shot selection
- Create a temporary batter root and a bat object with a Collider.
- Add `BattingController` to the batter root; assign the bat Transform and a shot-direction Transform.
- Add `ShotSelectionController`; assign the BattingController and shot-direction Transform.
- The shot selection script changes the aim Transform and calls the batting swing. It is an early prototype: the shot categories currently change aim only; unique animations and tuned shot physics must still be implemented.

## 6. Score and rules
- Create a GameObject with `MatchManager`; optionally add a TextMeshPro UI label and assign it.
- Add `MatchScoringRules` and assign the MatchManager.
- Connect umpire/gameplay events to the public scoring methods. Ensure exactly one scoring method is called per delivery outcome.
- Place a boundary trigger around the rope and assign the MatchManager to `BoundaryScoringZone`.
- Place a wicket trigger behind the stumps and assign the MatchManager to `WicketTrigger`.
- The current score system is a prototype. It does not yet handle every dismissal, overthrow, strike rotation, innings target, or all official Laws of Cricket.

## 7. Fielders
- Add `FielderAI` to a fielder root.
- Call `Chase(CricketBall)` from a match/ball event when the ball is hit toward that fielder.
- Connect the On Ball Collected event to your own return-throw or ball-reset logic. The current script is a simple movement prototype, not full fielding or catching AI.

## 8. Mobile controls
- Create a Canvas with buttons for Bowl, shot selection and Swing.
- Add `MobileCricketControls` and wire its public methods to the buttons.
- Wire ShotSelectionController's Select methods to shot buttons and PlaySelectedShot to the main shot button.
- Set Canvas Scaler to Scale With Screen Size and test multiple aspect ratios.

## 9. Camera and testing
- Assign the batter or a camera target to `CricketCameraFollow`.
- Test ball release, bounce, bat contact, scoring and wicket triggers in the Editor.
- Tune all serialized values with the actual models and scale. Then profile on a physical Android device.

## Still required for a complete game
Original player and stadium models, rigs, animations, crowd, audio, menus, team selection, full cricket rules, match flow, save system, performance optimization, Android builds and QA are not included in this starter repository yet.
