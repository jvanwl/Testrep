# Required Unity Assets

## Core Assets (Required)

### 1. 3D Models and Environment
- POLYGON Starter Pack - Low Poly 3D Art by Synty
  - Asset Store Link: https://assetstore.unity.com/packages/3d/props/polygon-starter-pack-low-poly-3d-art-by-synty-156819
  - Required for: Basic buildings, props, and environment
  - Key components:
    - Building models (houses, barracks, farms)
    - Props (trees, rocks, decorations)
    - Terrain textures
  - Import settings:
    - Model scale: 1
    - Generate colliders: Yes
    - Import materials: Yes

### 2. Character Models
- RPG Tiny Hero Duo PBR Polyart
  - Asset Store Link: https://assetstore.unity.com/packages/3d/characters/humanoids/rpg-tiny-hero-duo-pbr-polyart-225148
  - Required for: Unit models and animations
  - Components needed:
    - Character models
    - Basic animations (walk, idle, attack)
    - Materials and textures
  - Import settings:
    - Rig type: Humanoid
    - Optimize Game Objects: Yes
    - Animation compression: Optimal

### 3. UI Elements
- Clean Vector Icons
  - Asset Store Link: https://assetstore.unity.com/packages/2d/gui/icons/clean-vector-icons-132084
  - Required for: Interface icons and buttons
  - Setup needed:
    - Create UI sprite atlas
    - Set to 'Sprite' texture type
    - Use RGBA32 format for transparency

- Customizable UI Pack
  - Asset Store Link: https://assetstore.unity.com/packages/2d/gui/customizable-ui-pack-157691
  - Required for: Menu systems and HUD
  - Components:
    - Menu backgrounds
    - Progress bars
    - Button templates
    - Resource icons

### 4. Visual Effects
- Polygon Battle FX
  - Asset Store Link: https://assetstore.unity.com/packages/vfx/particles/polygon-battle-fx-low-poly-particle-effects-215157
  - Required for: Combat and ability effects
  - Key effects needed:
    - Attack impacts
    - Building construction
    - Resource gathering
    - Magic effects
  - Import settings:
    - Enable GPU instancing
    - Keep materials in Materials folder

### 5. Audio Assets
- FREE Casual Game SFX Pack
  - Asset Store Link: https://assetstore.unity.com/packages/audio/sound-fx/free-casual-game-sfx-pack-54116
  - Required for: Basic sound effects
  - Key sounds:
    - Button clicks
    - Resource collection
    - Building placement
    - Combat sounds

- RPG Music Pack
  - Asset Store Link: https://assetstore.unity.com/packages/audio/music/rpg-music-pack-215147
  - Required for: Background music
  - Setup needed:
    - Convert to compressed formats
    - Set up audio mixers
    - Create playlists per game state

## Optional Enhancement Assets

### 1. Advanced Visual Effects
- Ultimate VFX
  - Asset Store Link: https://assetstore.unity.com/packages/vfx/particles/ultimate-vfx-126079
  - Optional for: Enhanced visual effects
  - Recommended for high-end devices

### 2. Additional Character Models
- RPG Monster Duo PBR Polyart
  - Asset Store Link: https://assetstore.unity.com/packages/3d/characters/creatures/rpg-monster-duo-pbr-polyart-157762
  - Optional for: Enemy variations
  - Adds diversity to unit types

### 3. Environment Enhancement
- POLYGON Nature Pack
  - Asset Store Link: https://assetstore.unity.com/packages/3d/vegetation/trees/polygon-nature-pack-120152
  - Optional for: Enhanced environment
  - Adds variety to landscapes

## Asset Organization Structure

```
Assets/
├── Models/
│   ├── Buildings/
│   ├── Characters/
│   ├── Props/
│   └── Terrain/
├── Materials/
│   ├── Buildings/
│   ├── Characters/
│   └── Effects/
├── Textures/
│   ├── UI/
│   ├── Environment/
│   └── Characters/
├── Animations/
│   ├── Characters/
│   └── UI/
├── Audio/
│   ├── Music/
│   ├── SFX/
│   └── Ambient/
└── VFX/
    ├── Combat/
    ├── Environment/
    └── UI/
```

## Import Guidelines

1. Create the folder structure above before importing
2. Import assets one category at a time
3. Verify import settings for each asset type
4. Create appropriate prefabs after import
5. Set up material instances as needed
6. Configure and test LODs for models
7. Optimize textures for mobile

## Performance Considerations

1. Texture Sizes
   - UI: 512x512 max
   - Characters: 1024x1024 max
   - Environment: 512x512 max

2. Polygon Counts
   - Buildings: 2000 tris max
   - Characters: 1500 tris max
   - Props: 500 tris max

3. Audio
   - Music: 128kbps MP3
   - SFX: 64kbps MP3
   - Max length: 3 minutes per track
