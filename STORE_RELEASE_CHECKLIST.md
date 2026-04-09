# Solitaire Expert - Store Release Checklist

This project is a Unity game (`2019.2.21f1`). Use this checklist to publish to:
- Apple App Store (iOS)
- Google Play (Android)
- Optional PC stores (Steam / itch.io)

## 1) Prepare the project once

1. Open project with Unity `2019.2.21f1`.
2. Set a unique company + product name:
   - `Edit > Project Settings > Player`
3. Set versioning:
   - `Version`: `1.0.0`
   - `Bundle Version Code` (Android): `1`
   - `Build` (iOS): `1`
4. Set app identifiers:
   - iOS: `com.yourcompany.solitaireexpert`
   - Android: `com.yourcompany.solitaireexpert`
5. Configure icons + splash screens for both iOS and Android.
6. Confirm orientation (portrait/landscape) and target devices.
7. Verify no obvious missing permissions are requested.
8. Replace test ads/analytics keys with production keys.

## 2) Apple App Store (iOS)

Important: iOS builds require a Mac with Xcode.

1. Apple setup:
   - Enroll in Apple Developer Program.
   - Create App ID in Apple Developer portal.
   - Create the app record in App Store Connect.
2. In Unity:
   - `File > Build Settings > iOS > Switch Platform`
   - Set `Target minimum iOS Version` to a currently supported value.
   - Build Xcode project.
3. In Xcode:
   - Open generated Xcode project.
   - Set Signing Team + Bundle Identifier.
   - Set deployment target and capabilities if needed.
   - Archive + Validate + Upload to App Store Connect.
4. In App Store Connect:
   - Fill metadata (title, subtitle, description, keywords, support URL, privacy URL).
   - Upload screenshots for required iPhone sizes.
   - Complete App Privacy questionnaire.
   - If tracking users/ads, configure ATT prompts and tracking declaration.
   - Add age rating and content rights info.
   - Submit for review.

## 3) Google Play (Android)

1. Google setup:
   - Create Google Play Console developer account.
   - Create a new app entry.
2. In Unity:
   - `File > Build Settings > Android > Switch Platform`
   - Build `Android App Bundle (.aab)`.
   - Ensure package name matches Play Console app.
3. Signing:
   - Use a stable keystore and keep backups securely.
   - Use same keystore for all future updates.
4. In Play Console:
   - Upload `.aab` to internal testing first.
   - Fill store listing (short/full description, icon, feature graphic, screenshots).
   - Complete Data safety form.
   - Complete content rating + target audience.
   - Set pricing and countries.
   - Promote to production.

## 4) Other stores

## Steam (PC)

1. Build standalone Windows (and optionally macOS/Linux) from Unity.
2. Create Steamworks app + store page.
3. Integrate Steamworks SDK only if needed (achievements/cloud saves/etc.).
4. Upload build with SteamPipe.
5. Complete capsule art, screenshots, trailer, description, legal fields.
6. Submit for store review.

## itch.io (PC/Mobile)

1. Build desktop/mobile binaries from Unity.
2. Create itch.io project page.
3. Upload builds and set channels (`windows`, `mac`, `linux`, `android`).
4. Add screenshots, pricing, tags, and release notes.

## 5) Quality checks before submission

1. Test install + first launch on real devices (not only editor).
2. Verify resume/pause, audio, orientation, and low-memory behavior.
3. Verify no crashes at startup and no missing assets/fonts.
4. Confirm privacy policy URL is live and accurate.
5. Confirm ads/IAP (if used) work in production mode.

## 6) Recommended next technical upgrades

This project uses older Unity (`2019.2.21f1`). Store requirements evolve, so consider:

1. Upgrade to the latest Unity LTS (`Unity 6.3 LTS` at time of writing).
2. Re-test ads/analytics/purchasing packages after upgrade.
3. Rebuild and validate with latest Xcode + Android SDK.

If you want, we can do this next in the project:
- create exact Player Settings values,
- prepare iOS/Android build profiles,
- and generate a release checklist specific to your company details.
