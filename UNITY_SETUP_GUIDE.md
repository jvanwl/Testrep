# Unity Setup Guide

## Required Software
1. Unity Hub
2. Unity 2022.3 LTS
3. Android SDK and JDK (will be installed through Unity Hub)

## Initial Setup Steps

### 1. Unity Installation
1. Install Unity Hub from [https://unity.com/download](https://unity.com/download)
2. Open Unity Hub
3. Go to Installs > Install Editor
4. Select Unity 2022.3 LTS
5. In the Modules selection, check:
   - Android Build Support
   - Android SDK & NDK Tools
   - OpenJDK

### 2. Project Setup
1. In Unity Hub, click "Add" and select this project folder
2. When prompted, select Unity 2022.3 LTS
3. Wait for Unity to generate all project files

### 3. Editor Configuration
1. Open Edit > Project Settings
2. Configure the following:

#### Player Settings (Edit > Project Settings > Player)
- Company Name: YourCompany
- Product Name: Civilization Game
- Default Icon: Set your game icon
- Android Tab:
  - Package Name: com.yourcompany.civilizationgame
  - Minimum API Level: Android 8.0 (API 26)
  - Target API Level: Android 13 (API 33)
  - Scripting Backend: IL2CPP
  - Target Architectures: ARM64

#### Quality Settings
1. Go to Edit > Project Settings > Quality
2. Set up three quality levels:
   - Low (for low-end devices)
   - Medium (default)
   - High (for high-end devices)

#### Graphics Settings
1. Go to Edit > Project Settings > Graphics
2. Set Universal Render Pipeline asset

### 4. Asset Import
1. Open Window > Package Manager
2. Import all required assets listed in ASSETS_LIST.md
3. Wait for all imports to complete

### 5. Scene Setup

#### Main Menu Scene
1. Open Scenes/MainMenu.unity
2. Verify Canvas is set to scale with screen size
3. Check all UI elements are properly connected
4. Ensure GameManager is present with correct references

#### Main Game Scene
1. Open Scenes/MainGame.unity
2. Setup main camera:
   - Position: (0, 10, -10)
   - Rotation: (45, 0, 0)
   - Clear Flags: Skybox
3. Add Directional Light
4. Add EventSystem
5. Setup UI Canvas
6. Add GameManager prefab

### 6. Build Setup
1. Open File > Build Settings
2. Add scenes in order:
   - MainMenu
   - MainGame
3. Switch Platform to Android
4. Player Settings:
   - Verify keystores are set up
   - Check "Development Build" for testing

## Testing

### In-Editor Testing
1. Open MainMenu scene
2. Press Play
3. Test basic functionality:
   - Menu navigation
   - Scene loading
   - Basic gameplay

### Android Testing
1. Enable USB Debugging on your Android device
2. Connect device to computer
3. In Build Settings, click "Build And Run"
4. Test on device:
   - Performance
   - Touch controls
   - UI scaling

## Common Issues

### Missing References
- Check Console window for errors
- Verify all prefabs have required components
- Ensure all scriptable objects are properly assigned

### Android Build Failed
- Verify Android SDK/JDK paths in Preferences
- Check Platform-specific settings
- Validate keystore settings

### Performance Issues
- Use Unity Profiler to identify bottlenecks
- Check Graphics settings
- Verify asset import settings

## Next Steps

1. Review ASSETS_LIST.md for required assets
2. Follow build instructions in BUILD_GUIDE.md
3. Test thoroughly using TEST_CHECKLIST.md
