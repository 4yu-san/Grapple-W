[← Back to README](../README.md)

# Installation & Setup

## Requirements

| Requirement | Version / Notes |
|---|---|
| Unity Editor | **6000.4.0f1** (exact match recommended — grab it via Unity Hub if you don't have it) |
| Platform | Developed on Windows/Linux; should run anywhere Unity 6 does |
| Git | Any recent version |

> The project's editor version is pinned in `ProjectSettings/ProjectVersion.txt`. If Unity Hub prompts you to install a matching editor version when you open the project, let it — mismatched versions can cause materials/shaders to reimport incorrectly.

## 1. Clone the repository

```bash
git clone https://github.com/4yu-san/Grapple-W.git
cd Grapple-W
```

## 2. Open in Unity Hub

1. Open **Unity Hub**.
2. Click **Add** → **Add project from disk**.
3. Select the cloned `Grapple-W` folder.
4. If prompted for a missing editor version, install `6000.4.0f1` (or the closest available Unity 6 release) through the Hub.
5. Open the project.

First import may take a few minutes while Unity re-imports all Assets (materials, shaders, TextMesh Pro resources, etc.).

## 3. Open the entry scene

Once the editor loads:

```
Assets/Scenes/MainMenu.unity
```

This is the starting point — it drives navigation into the playable levels.

Other scenes in the project:

| Scene | Purpose |
|---|---|
| `MainMenu.unity` | Main menu / entry point |
| `SampleScene.unity` | Primary playable level |
| `Level 2.unity` | Second level |

## 4. Press Play

Hit the Play button in the Unity Editor toolbar. No additional package installs or build steps are required to run it in-editor.

## Building a Standalone Executable

1. `File → Build Settings`.
2. Make sure `MainMenu`, `SampleScene`, and `Level 2` are added to **Scenes In Build**, in that order (index 0, 1, 2 — `UIManager.NextLevel()` advances by build index).
3. Choose your target platform and hit **Build**.

## Troubleshooting

- **Pink/missing materials after opening:** Let Unity finish reimporting — the custom shaders (`Shader1.shader`, `VertexOffset.shader`) sometimes need a domain reload to compile on first open.
- **Cursor stuck locked/invisible after exiting Play mode oddly:** This is a known quirk of the current cursor-lock handling in `PlayerMovement`/`UIManager` — click back into the Game view or stop/restart Play mode.
