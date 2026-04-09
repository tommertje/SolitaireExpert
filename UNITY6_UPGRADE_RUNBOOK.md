# Solitaire Expert - Unity 6.3 LTS Upgrade Runbook

This project currently targets Unity `2019.2.21f1`.

The safe upgrade target is `Unity 6.3 LTS`.

## 1) Create safety point

1. Commit all current work to git.
2. Create a backup copy of the full project folder.

## 2) Install required tools

Install in Unity Hub:
- Unity Editor `6.3 LTS`
- Android Build Support (SDK/NDK + OpenJDK)
- iOS Build Support (Mac only)
- Windows Build Support (IL2CPP)

## 3) Open and migrate project

1. Open project from Unity Hub with `6.3 LTS`.
2. Let Unity perform automatic project upgrade.
3. Wait for import/compile to finish.

## 4) Resolve package migration

Open `Window > Package Manager` and check for:
- Ads package migration prompts
- Analytics package migration prompts
- Purchasing package updates
- Any package with warning/error badges

Apply updates one package at a time and re-test after each update.

## 5) Player settings sanity check

In `Project Settings > Player`:
- Set company and product name
- Set iOS and Android bundle identifiers
- Set version and build numbers
- Verify orientation and graphics API
- Verify scripting backend is IL2CPP for release builds

## 6) Fix compile/runtime issues

1. Clear all Console compile errors first.
2. Enter Play Mode and test:
   - card drag/drop
   - stack movement
   - undo
   - new game
   - menu popups

## 7) Build verification

1. Android: create a test `.aab`.
2. iOS: create Xcode project and compile on Mac.
3. Run smoke tests on real devices.

## 8) Release prep

After successful migration, continue with `STORE_RELEASE_CHECKLIST.md` for submission.

## Notes for this repository

- Most gameplay scripts rely on mouse/touch logic and should still work in Unity 6.
- The highest upgrade risk is package compatibility (ads/analytics/purchasing), not core card logic.
- Do not change package versions in bulk; migrate incrementally.
