# REAL CRICKET 3D

An original Unity 6 offline-first cricket simulation project for Android.

## Open the Unity project

- Unity Editor: **6000.6.4f1**
- Primary target: Android (ARM64)
- UI dependency: Unity UI (uGUI)
- Open the repository folder in Unity Hub and allow the Editor to finish importing and compiling.
- Use **RealCricket3D → Build Prototype Scene** to regenerate the current greybox gameplay scene. This replaces the generated scene, so back up manual scene edits first.

## Android APK from GitHub Actions

See [Docs/ANDROID_BUILD.md](Docs/ANDROID_BUILD.md) for the one-time Unity license secret setup and APK download steps. The workflow is `.github/workflows/android-apk.yml`; run it from the repository's **Actions** tab.

## Project status

This is an actively developed prototype foundation, not a finished commercial cricket game. Source implementations must still be integrated, compiled, and tested in Unity and on Android. The roadmap tracks remaining work in [Docs/ROADMAP.md](Docs/ROADMAP.md).