[← Back to README](../README.md)

# Architecture

## Folder Structure

```
Grapple-W/
├── Assets/
│   ├── Scripts/
│   │   ├── Managers/
│   │   │   ├── GameManager.cs        # Central game-state singleton
│   │   │   ├── UIManager.cs          # Panel/UI state singleton
│   │   │   └── AudioManager.cs       # Music/SFX singleton
│   │   ├── Mechanics/
│   │   │   ├── GrapplingGun.cs       # Grapple hook + shoot
│   │   │   ├── RotateGun.cs          # Gun-model orientation
│   │   │   ├── PlayerMovement.cs     # Rigidbody FPS controller
│   │   │   ├── MoveCamera.cs         # Camera-follows-player rig
│   │   │   └── Wallrunning.cs        # Stub — not yet implemented
│   │   ├── LevelEnd.cs               # Trigger → level complete
│   │   ├── LevelFailed.cs            # Trigger → level failed
│   │   ├── Stopwatch.cs              # Timer + best-time tracking
│   │   └── HowToPlayPanel.cs         # Help overlay toggle
│   ├── Shaders/
│   │   ├── Shader1.shader            # Gradient/glow, additive transparent
│   │   └── VertexOffset.shader       # Radial wave / portal effect
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   ├── SampleScene.unity
│   │   └── Level 2.unity
│   ├── Materials/                    # Cubes, END ZONE, Ground color, etc.
│   └── TextMesh Pro/                 # TMP package assets (imported, not authored)
├── Packages/
├── ProjectSettings/
└── README.md
```

## Design Patterns

### Singleton Managers

`GameManager`, `UIManager`, and `AudioManager` all follow the same lazy-singleton pattern:

```csharp
public static GameManager instance
{
    get
    {
        if (_instance == null)
            _instance = UnityEngine.Object.FindAnyObjectByType<GameManager>();
        return _instance;
    }
}
```

This keeps cross-script communication simple for a project this size (no dependency injection or service locator overhead) — any script can call `GameManager.instance.SomeMethod()` without needing a scene reference wired up in the Inspector. `AudioManager` additionally uses `DontDestroyOnLoad` so music can persist across scene loads.

### State Machine

`GameManager.GameState` (`Playing` / `GameOver` / `Paused`) is the single source of truth for what the game is currently doing. `SetState()` centralizes the side effects of each transition (`Time.timeScale`, flags), so UI and gameplay scripts react to state changes rather than pushing state themselves.

### Trigger-Driven Level Flow

Level completion/failure is entirely collider-driven: `LevelEnd`/`LevelFailed` are dumb trigger components that just tag-check for `"Player"` and delegate to `GameManager`. This keeps level geometry/design decoupled from game logic — designers can drop trigger volumes anywhere without touching code.

## Script Responsibility Overview

| Script | Layer | Responsibility |
|---|---|---|
| `GameManager.cs` | Core | Game state, start/reset/game-over/level-complete orchestration |
| `UIManager.cs` | Core | Panel visibility, cursor lock state, scene navigation |
| `AudioManager.cs` | Core | Music + SFX playback and volume |
| `GrapplingGun.cs` | Mechanics | Grapple raycast/joint, pull force, rope rendering, shoot |
| `RotateGun.cs` | Mechanics | Visual gun rotation toward grapple point |
| `PlayerMovement.cs` | Mechanics | Rigidbody movement, jump, crouch/slide, ground detection |
| `MoveCamera.cs` | Mechanics | Keeps camera rig positioned on the player |
| `Wallrunning.cs` | Mechanics | Placeholder for future wall-run mechanic |
| `Stopwatch.cs` | Gameplay | Live timer, per-level best time via `PlayerPrefs` |
| `LevelEnd.cs` / `LevelFailed.cs` | Gameplay | Trigger volumes that report outcomes to `GameManager` |
| `HowToPlayPanel.cs` | UI | Help overlay open/close |

## Notes on the Custom Shaders

Both shaders are written directly in ShaderLab with `CGPROGRAM` blocks (not Shader Graph), targeting Unity's built-in pipeline shader syntax. They're time-driven (`_Time.y`) and UV/normal-based rather than texture-sampled, which keeps them lightweight and easy to tweak via exposed `Color`/`Range` properties in the Inspector.
