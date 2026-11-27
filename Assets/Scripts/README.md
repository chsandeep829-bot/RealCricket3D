# Gameplay scripts

This folder contains the first Unity C# gameplay prototypes.

## Scripts
- `Gameplay/CricketBall.cs`: Rigidbody-based ball state and velocity.
- `Gameplay/CricketCameraFollow.cs`: smooth camera follow for a selected player.
- `Bowling/BowlingController.cs`: creates and releases a delivery.
- `Batting/BattingController.cs`: swing action and ball contact.
- `Match/MatchManager.cs`: basic runs, wickets, overs and innings state.
- `Match/BoundaryScoringZone.cs`: awards configured boundary runs when a hit ball enters its trigger.
- `Match/WicketTrigger.cs`: records a wicket when an unhit delivery enters the wicket trigger.
- `UI/MobileCricketControls.cs`: connects Unity UI buttons to batting and bowling actions.

## Scene setup
1. Create a **3D Core** project in Unity Hub.
2. Copy this repository's `Assets/Scripts` folder into the Unity project's `Assets` folder.
3. Create a ball prefab with a Sphere Collider, Rigidbody and CricketBall component. Set Rigidbody collision detection to Continuous Dynamic and assign the Rigidbody.
4. Create a release-point Transform near the bowler and assign it with the ball prefab to BowlingController.
5. Add a bat model with a collider to the batter. Assign its Transform to BattingController. Tune swingRotation and hitImpulse for your model.
6. Create a trigger volume around the boundary and attach BoundaryScoringZone. Set its run value to 4 or 6.
7. Add a trigger collider behind the stumps and attach WicketTrigger. Assign the MatchManager.
8. Add a Canvas with Bat and Bowl buttons. Add MobileCricketControls to a scene object, assign both controllers, and connect the buttons to OnBatPressed and OnBowlPressed.
9. Add a TextMeshPro text object and assign it to MatchManager's scoreText field.
10. Assign a player Transform to CricketCameraFollow.

## Important prototype limitations
These scripts are a foundation, not a finished cricket rules engine. Boundary zones need careful placement so the ball scores only after crossing the rope. Wicket detection, run-outs, wides, no-balls, byes, leg-byes, overs, strike rotation, innings transitions and match formats still need full rules implementation. The sample batting and bowling controls are deliberately simple and require animation, timing windows, shot selection, input tuning and testing.

The B and Space keys in the individual controllers are editor prototype controls. Android gameplay should use the Canvas buttons or a later gesture/timing input system.
