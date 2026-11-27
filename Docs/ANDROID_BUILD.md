# Android APK build with GitHub Actions

The repository includes `.github/workflows/android-apk.yml`. It uses Unity **6000.6.4f1** and the `RealCricketAndroidBuild.BuildAndroid` Editor build method.

## One-time Unity license setup

1. Activate Unity 6.6 on your development computer using Unity Hub.
2. In Unity Hub, confirm that the Editor is licensed and can open this project.
3. Obtain the Unity license file for the account/license you are entitled to use. Do not commit the license file to the repository.
4. In GitHub, open **RealCricket3D → Settings → Secrets and variables → Actions → New repository secret**.
5. Name the secret exactly `UNITY_LICENSE` and paste the contents of your Unity license file into its value. Keep it private; never post it in chat, issues, commits, or screenshots.

A valid Unity license is required by the hosted Unity build runner. GitHub Actions minutes and runner availability are subject to the account's limits.

## Start a build

- Open the repository's **Actions** tab.
- Select **Build RealCricket3D Android APK**.
- Select **Run workflow** on the `main` branch.
- Open the completed workflow run and download the artifact named **RealCricket3D-Android-APK**.

The workflow also runs on pushes to `main` that change `Assets/**`, `Packages/**`, `ProjectSettings/**`, or the workflow itself.

## Output

The build method creates:

`Builds/Android/RealCricket3D.apk`

The workflow uploads that APK as a 14-day artifact. The APK is an installable development build, not a Play Store release. Test it on a supported Android device before sharing or publishing.

## If the workflow fails

- **License activation error:** verify the `UNITY_LICENSE` repository secret is present and contains the correct license file contents. Do not expose the license publicly.
- **Android module/toolchain error:** verify the Unity 6000.6.4f1 Android Build Support module is available to the build runner.
- **C# compile error:** open the Unity project in the matching Editor version, wait for compilation, and inspect the first error in the Console. Fix the earliest error before chasing follow-on errors.
- **Build succeeds but gameplay is broken:** an APK build only proves the project compiled and packaged. It does not prove match rules, touch controls, device performance, or gameplay have been tested.

## Current project status

The build workflow and editor build entry point are present. The generated match scene is still a greybox foundation; original production-quality player/stadium assets, complete cricket laws, extensive gameplay balancing, and device testing remain unfinished. Do not treat an APK build as proof that the full game is complete.
