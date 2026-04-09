# Solitaire Expert - Publish To All Stores (Windows-first)

This project is already on Unity `6000.4.1f1`.

## Can Apple App Store be published from Windows?

Short answer: **not with Docker alone**.

- Apple requires Xcode/macOS for final iOS archive/signing/upload.
- Running macOS/Xcode in normal Docker on Windows is not a supported/legal production path.

Workable alternatives from Windows:

1. Use **cloud macOS CI** (GitHub Actions macOS runner, Codemagic, Bitrise, Appcircle, MacStadium-hosted runner).
2. Push code from Windows, let cloud macOS build/sign/upload.
3. Manage App Store Connect metadata/review from browser on Windows.

## Store publish order (recommended)

1. Google Play (fastest)
2. Apple App Store (cloud macOS workflow)
3. itch.io
4. Steam

## 1) Google Play (from Windows)

1. In Unity:
   - Switch platform: Android
   - Build App Bundle (`.aab`)
   - Confirm package id: `com.SolitaireExperts.SolitaireExpert`
2. In Play Console:
   - Create app
   - Upload `.aab` to Internal testing
   - Complete Data safety + content rating + target audience
   - Add listing assets and publish Production release

GitHub Actions secrets for Android auto-upload:

- `GOOGLE_PLAY_SERVICE_ACCOUNT_JSON` (full service account JSON from Play Console API access)

Workflow toggle:

- Run `.github/workflows/unity-store-builds.yml`
- Set `upload_android_to_internal=true` to upload AAB to internal track

## 2) Apple App Store (workaround from Windows)

Use cloud macOS CI with your Apple credentials in CI secrets:

- `APPLE_TEAM_ID`
- `APPLE_ID`
- `APP_SPECIFIC_PASSWORD` (or API key based setup)
- signing certificates/profiles (if using manual signing)

Flow:

1. Commit/push from Windows.
2. Trigger macOS CI job that:
   - switches to iOS
   - generates Xcode project
   - builds/signs archive
   - uploads to App Store Connect
3. Complete metadata/review submission in App Store Connect web UI.

GitHub Actions secrets for iOS upload:

- `APP_STORE_CONNECT_ISSUER_ID`
- `APP_STORE_CONNECT_API_KEY_ID`
- `APP_STORE_CONNECT_API_PRIVATE_KEY` (contents of `.p8` key)

Workflow toggle:

- Run `.github/workflows/unity-store-builds.yml`
- Set `upload_ios_to_testflight=true` to archive/export/upload to TestFlight

## 3) itch.io

1. Build Windows desktop player.
2. Create itch.io project page.
3. Upload build with Butler or web upload.

## 4) Steam

1. Build Windows standalone.
2. Upload via SteamPipe (Steamworks SDK).
3. Complete store page and release checklist.

## Required app metadata (prepare once)

- App name: `Solitaire Expert`
- Short description (<= 80 chars)
- Full description
- Privacy policy URL
- Support URL/contact email
- Store screenshots and icons

## Important project notes

- Existing identifiers:
  - Android: `com.SolitaireExperts.SolitaireExpert`
  - iOS: `com.SolitaireExperts.SolitaireExpert`
- Keep the same bundle id forever after first store release.
- Never lose the Android keystore used for release signing.

