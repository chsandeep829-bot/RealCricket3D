# Gameplay scripts

This folder contains the first Unity C# gameplay prototypes.

## Scripts
- `Gameplay/CricketBall.cs`: Rigidbody-based ball state and velocity.
- `Bowling/BowlingController.cs`: creates and releases a delivery.
- `Batting/BattingController.cs`: swing action and ball contact.
- `Match/MatchManager.cs`: basic runs, wickets, overs and innings state.

## Unity setup
1. Create a **3D Core** project in Unity Hub.
2. Copy the `Assets/Scripts` folder into the Unity project's `Assets` folder.
3. Add a Rigidbody and suitable collider to the ball prefab. Assign the Rigidbody through CricketBall.
4. Assign the ball prefab and release point to BowlingController.
5. Assign the bat transform and optional shot direction to BattingController.
6. Add TextMeshPro to the project and assign a TMP text object if you want the sample scoreboard.

The B and Space keys are editor prototype controls only. For Android, connect the public `BowlDelivery()` and `Swing()` methods to on-screen UI buttons.

This is starter code, not a complete playable cricket game. It still needs calibrated collision geometry, crease and wicket rules, fielding, animations, camera controls, UI, assets and device testing.
