# Interface and Android validation

Validated with Unity 6000.6.5f1 on October 15, 2026.

## Responsibilities

- Core and Content retain puzzle rules, solving, campaign generation and progression.
- GameRoot coordinates game state, navigation, board presentation and persistence.
- BoardInput handles mouse/touch gestures, UI raycast exclusion, drag rejection and cancellation on navigation, focus loss or pause.
- GameUI composes screens and presents game state. UIFactory owns reusable controls and generated rounded assets; UITheme centralizes shared colors.
- GlassSurface supplies local mesh coordinates to the shared glass material and animates button presses. FrostedUI uses nine backdrop samples from the existing quarter-resolution scene capture, rounded edges and restrained refraction.
- ReadableText recalculates minimum screen text size only when layout, transform, projection or screen size changes. Montserrat Medium 500 and Bold 700 use their actual font files, with no synthetic bold. Their OFL license is kept beside the fonts.

## Launcher icons

Beltz > Apply branding and icons regenerates legacy and round icons at 36, 48, 72, 96, 144 and 192 pixels, adaptive layers at 81, 108, 162, 216, 324 and 432 pixels, a safe-zone foreground, monochrome layer and 512-pixel store icon. Source artwork is in Assets/Beltz/Art; generated assets are in AndroidIcons. The build hook configures Unity slots and adds Android density resources, adaptive XML, API 33 monochrome XML and icon/roundIcon manifest references. The icon importer is restricted to icon assets.

The adaptive and monochrome foreground radius is 0.235 of the canvas (previously 0.295), reducing the white gear by about 20% for Android launcher masks. Legacy artwork remains unchanged.

Supported Android versions begin at API 29; target and compile SDK are 37. Additional density variants do not lower minSdk.

## Checks

- Windows release build succeeded.
- Existing full gameplay smoke run passed all 120 levels, section progression, tutorials, settings, undo/restart, victory animation and text bounds, with zero errors.
- Smoke runs passed at 720 x 1280, 1080 x 1440 and 540 x 1230 after input extraction. Windows capped the requested 1080 x 2460 window at 1440 pixels high; the separate 540 x 1230 run verified the exact phone aspect ratio. Saved PNG dimensions were checked. These are desktop renders, not physical-device touch or performance tests.
- Release README screenshot run passed font and overflow checks; English screenshots were refreshed.
- Android development APK built successfully. Its manifest and resource table contain all six icon densities and both adaptive variants, including the monochrome layer.

Local evidence: Logs/refactor-build.log, Logs/refactor-smoke.log, Logs/refactor-smoke-phone.log, Logs/refactor-smoke-phone-aspect.log, Logs/refactor-screenshots.log, Logs/android-icons-build.log and Logs/apk-resources.txt. Generated builds, logs and smoke captures are ignored by Git.

A signed release update with the smaller adaptive icon was built and installed over the existing app on a Tecno Pova 5, preserving app data. Local evidence: Logs/android-icon-size-release.log. No Play publication, physical-phone GPU/thermal measurement or iOS validation is claimed.

Release APK (Builds/Android/Beltz-2.0-release.apk) SHA-256: 80D20A0A7E41D027A187F962C9DE07D1A74E3E11866982832E3BC74251C41010. APK signature verification passed.
