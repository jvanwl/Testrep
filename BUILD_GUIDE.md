# Build Guide

## Prerequisites

### 1. Development Environment
- Unity 2022.3 LTS
- Android SDK API Level 33
- Android Build Tools 33.0.0
- Android NDK 21.3.6528147
- JDK 11.0.16
- Gradle 7.1.1

### 2. Project Configuration
- Universal Render Pipeline
- IL2CPP Scripting Backend
- .NET Standard 2.1 API Compatibility

## Build Configuration

### 1. Player Settings

#### Identification
```
Company Name: YourCompany
Product Name: Civilization Game
Version: 1.0.0
Bundle Version Code: 1
Package Name: com.yourcompany.civilizationgame
```

#### Configuration
```
Scripting Backend: IL2CPP
API Compatibility Level: .NET Standard 2.1
Target Architectures: ARM64
```

#### Optimizations
```
Strip Engine Code: Enabled
Optimize Mesh Data: Enabled
```

### 2. Quality Settings

#### Low Quality (Tier 1)
```
Pixel Light Count: 2
Texture Quality: Half Res
Anisotropic Textures: Per Texture
Anti Aliasing: Disabled
Soft Particles: Disabled
Realtime Reflection Probes: Disabled
Billboards Face Camera Position: Disabled
```

#### Medium Quality (Tier 2)
```
Pixel Light Count: 4
Texture Quality: Full Res
Anisotropic Textures: Per Texture
Anti Aliasing: 2x Multi Sampling
Soft Particles: Enabled
Realtime Reflection Probes: Disabled
Billboards Face Camera Position: Enabled
```

#### High Quality (Tier 3)
```
Pixel Light Count: 8
Texture Quality: Full Res
Anisotropic Textures: Force Enable
Anti Aliasing: 4x Multi Sampling
Soft Particles: Enabled
Realtime Reflection Probes: Enabled
Billboards Face Camera Position: Enabled
```

## Build Process

### 1. Pre-Build Checklist

```bash
# 1. Clear Library folder
rm -rf Library/

# 2. Clear Build folder
rm -rf Build/

# 3. Clear PlayerData folder
rm -rf PlayerData/

# 4. Update asset bundles
Assets > Build AssetBundles

# 5. Run all tests
Window > General > Test Runner > Run All
```

### 2. Build Steps

#### Development Build
1. File > Build Settings
2. Switch Platform to Android
3. Check Development Build
4. Check Script Debugging
5. Build

#### Release Build
1. File > Build Settings
2. Switch Platform to Android
3. Uncheck Development Build
4. Set Compression Method to LZ4HC
5. Build

### 3. Post-Build Steps

```bash
# 1. Verify APK/AAB size
ls -lh Build/Android/

# 2. Install on test device
adb install Build/Android/Game.apk

# 3. Check logcat for errors
adb logcat -s Unity
```

## Keystore Management

### 1. Debug Keystore
```bash
keytool -genkey -v -keystore debug.keystore -storepass android -alias androiddebugkey -keypass android -keyalg RSA -keysize 2048 -validity 10000
```

### 2. Release Keystore
```bash
keytool -genkey -v -keystore release.keystore -alias civilization_game -keyalg RSA -keysize 2048 -validity 10000
```

### 3. Keystore Verification
```bash
keytool -list -v -keystore release.keystore
```

## Publishing

### 1. Google Play Store
- Create release in Google Play Console
- Upload AAB file
- Fill store listing
- Set up pricing & distribution
- Submit for review

### 2. Version Management
```json
{
  "major": 1,
  "minor": 0,
  "patch": 0,
  "buildNumber": "1"
}
```

## Troubleshooting

### 1. Common Issues

#### Build Failed
```
- Check Unity logs in Editor.log
- Verify Android SDK/JDK paths
- Clear Library folder and rebuild
```

#### APK Size Too Large
```
- Check texture compression settings
- Verify asset bundle configuration
- Review included libraries
```

#### Crash on Launch
```
- Check ADB logcat
- Verify minimum API level
- Check IL2CPP build settings
```

## Automated Build Setup

### 1. Jenkins Configuration
```groovy
pipeline {
    agent any
    stages {
        stage('Build') {
            steps {
                sh '/Applications/Unity/Hub/Editor/2022.3/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath . -executeMethod Builder.Build'
            }
        }
    }
}
```

### 2. GitHub Actions
```yaml
name: Build
on: [push]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    - uses: game-ci/unity-builder@v2
      env:
        UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
      with:
        targetPlatform: Android
```

## Release Checklist

### 1. Pre-Release
- [ ] All tests passing
- [ ] Performance benchmarks met
- [ ] Asset bundles updated
- [ ] Version numbers updated
- [ ] Release notes prepared

### 2. Release Build
- [ ] Clean build folder
- [ ] Create release build
- [ ] Test on multiple devices
- [ ] Verify all features
- [ ] Check crash reports

### 3. Store Submission
- [ ] Screenshots updated
- [ ] Store listing reviewed
- [ ] Content rating verified
- [ ] Privacy policy updated
- [ ] Release notes finalized
