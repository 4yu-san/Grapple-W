[← Back to README](../README.md)

# Features

## 🪝 Grappling Hook

`Scripts/Mechanics/GrapplingGun.cs`

- **Left Click** raycasts from the camera against a `whatIsGrappleable` layer mask; on hit, attaches a `SpringJoint` between the player and the hit point.
- The joint's `maxDistance`/`minDistance` are derived from the initial distance to the grapple point, giving a rope-like range rather than a fixed-length tether.
- A `LineRenderer` draws the rope, with the visible end lerping toward the true grapple point (`DrawRope()`) for a smoother, less snappy-looking line.
- **Right Click (hold)** applies an additional pull force along the direction to the grapple point, letting you actively reel yourself in rather than just swinging passively.
- **E** fires a separate projectile (`bulletPrefab`) from `bulletSpawnPoint` in the camera's forward direction — a lightweight shooting mechanic layered on top of the grapple.
- `RotateGun.cs` smoothly rotates the gun model to face the current grapple point while attached, and returns to resting orientation otherwise.

## 🏃 Momentum-Based Movement

`Scripts/Mechanics/PlayerMovement.cs`

Rigidbody-driven FPS controller (based on Dani's well-known movement tutorial) with:

- **Counter-movement** — actively cancels sideways/backward drift when there's no matching input, so the character doesn't feel like it's sliding on ice, while still preserving momentum when you *want* it (e.g. bunnyhopping).
- **Crouch-slide** — holding `Left Ctrl` while moving fast applies a forward slide force and lowers the collider scale; great for maintaining speed through tight spaces or off ramps.
- **Slope-aware grounding** — `OnCollisionStay` checks contact normals against `maxSlopeAngle` to decide what counts as "floor" versus a wall, feeding into both grounded state and jump direction.
- **Air control** — movement force is halved in the air, so grapple-swings and jumps aren't trivially overridden by air-strafing.
- **Max speed clamping** that only kicks in along the axis you're actively pushing, so momentum from other sources (like the grapple) isn't clipped unexpectedly.

## 🧗 Wallrunning *(planned)*

`Scripts/Mechanics/Wallrunning.cs`

Currently a stub `MonoBehaviour` with empty `Start()`/`Update()` — scaffolded for a future wall-run mechanic to complement the grapple and slide.

## ⏱️ Timer & Best Time

`Scripts/Stopwatch.cs`

- Runs a live stopwatch (`mm:ss.ms`) during a level attempt.
- On level completion, freezes the time, formats it for the end screen, and compares it against a per-level best time stored in `PlayerPrefs` (`BestTime_Level{buildIndex}`), updating it if you've beaten your record.

## 🏁 Level Flow

`LevelEnd.cs`, `LevelFailed.cs`, `Managers/GameManager.cs`, `Managers/UIManager.cs`

- Trigger colliders tagged for the player call into a central `GameManager` singleton on **level complete** or **level failed**, which drives a simple state machine (`Playing` / `Paused` / `GameOver`), including freezing `Time.timeScale` on end states.
- `UIManager` (also a singleton) reacts to state changes to show/hide the pause menu, game-over panel, and level-complete panel, and handles cursor lock/visibility so the mouse is only free when it needs to be (menus, pause, end screens).
- `NextLevel()` advances by Unity build index, so scene order in Build Settings matters.

## 🎨 Custom Shaders

`Shaders/Shader1.shader`, `Shaders/VertexOffset.shader`

Two hand-written Unlit shaders (ShaderLab + CGPROGRAM/HLSL, no Shader Graph):

- **`VertexOffset`** — offsets vertex height based on a radial wave function centered on the mesh's UVs, and colors the surface using the same wave value. This is the animated "pulsing portal" look used on the end-zone material.
- **`Shader1`** — an additive, transparent gradient shader that blends between two colors based on an animated, UV-driven wave pattern, masked to avoid the top/bottom caps of the mesh — used for glow/ring-style effects.

_See it animated in [Screenshots](SCREENSHOTS.md)._

## 🔊 Audio

`Managers/AudioManager.cs`

Singleton audio manager separating music (looping, crossfade-free swap between menu/game tracks) from one-shot SFX (level complete, game over, grapple), with simple volume setters for future settings-menu hookup.

## 🖥️ UI

- **How To Play panel** (`HowToPlayPanel.cs`) — toggle-able help overlay, dismissible with `Esc`.
- **Pause menu** — accessible mid-run via `Esc`, freezes time and frees the cursor.
- **Game Over / Level Complete panels** — driven by `UIManager`, show final/best times.
