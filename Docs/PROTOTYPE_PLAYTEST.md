# Prototype scene playtest

## Generate the greybox scene
1. Create a Unity **3D Core** project using a supported Unity Editor.
2. Copy this repository's `Assets/Scripts` folder and `Assets/Editor/PrototypeSceneBuilder.cs` into the new project's `Assets` folder.
3. If Unity prompts for TextMeshPro resources, import the TMP Essential Resources.
4. Wait for script compilation to finish.
5. In Unity, select **RealCricket3D → Build Prototype Scene** from the top menu.
6. Open `Assets/Scenes/CricketPrototype.unity` if it is not already open, then press Play.

## Controls
- **BOWL**: release a delivery from the bowler end.
- **DRIVE / PULL / LOFT**: select a basic shot direction/loft profile.
- **SHOT**: swing the bat.
- The score label displays the prototype innings score.

## What this builder creates
A greybox outfield and pitch, primitive batter and bowler, stumps, a ball prefab with Rigidbody physics, controller references, boundary and wicket trigger prototypes, a follow camera, basic lighting, and a simple touch-oriented HUD.

## Important limitations
This is an editor-time scene generator, not a complete game. It uses primitive capsules/cubes/spheres, placeholder materials and simplified controls. It does not generate realistic player rigs, animations, detailed stadiums, crowds, commentary, complete cricket laws, team selection, career mode, save progression or multiplayer. Boundary scoring and wicket logic are basic prototypes and need playtesting and correction. Use this scene to validate the first loop before investing in final art.
