# Android APK build with GitHub Actions

The repository uses Unity **6000.6.4f1** and GameCI to build an Android APK.

## Required GitHub Actions secrets

Open **RealCricket3D → Settings → Secrets and variables → Actions** and add these repository secrets:

- `UNITY_LICENSE`: the contents of the Unity license file for the account used to activate Unity Personal.
- `UNITY_EMAIL`: the email address of that Unity account.
- `UNITY_PASSWORD`: the Unity account password (not the Google account password).

Do not commit license files or passwords. Never paste them into issues, chat, source files, or screenshots. If the Unity account uses Google sign-in, first make sure a separate Unity account password has been configured through Unity account settings. The workflow cannot create or read these secrets for you.

## Start a build

1. Open the repository's **Actions** tab.
2. Select **Build RealCricket3D Android APK**.
3. Select **Run workflow** on the `main` branch.
4. Open the completed run and download **RealCricket3D-Android-APK**.

The workflow targets Android API 35 and creates `Builds/Android/RealCricket3D.apk`. The APK is a development build, not a Play Store release.

## Troubleshooting

- **License activation strategy error:** verify all three Unity secrets exist and that the email/password belong to the account associated with the license file.
- **Android SDK error:** check that the Unity Android Build Support module is available in the GameCI image and that the project's target SDK is set.
- **C# compile error:** open the project in Unity 6000.6.4f1, wait for compilation, and fix the first Console error before later errors.
- **Build succeeds but gameplay is broken:** a successful build proves packaging only. It does not prove gameplay, touch controls, match rules, or device performance have been tested.

## Project status

The generated match scene is a greybox prototype. Original production-quality player/stadium assets, complete cricket rules, gameplay balancing, and Android device testing remain unfinished. Do not treat an APK build as proof that the full game is complete.
