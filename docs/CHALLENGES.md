[← Back to README](../README.md)

# Challenges & Learnings

Not everything in a project like this goes smoothly. This page is for the parts that were genuinely hard to get right — the goal is to be honest about it rather than presenting the finished code as if it always looked this clean.

## The Singleton Pattern

The trickiest part of this project wasn't the physics or the shaders — it was getting `GameManager`, `UIManager`, and `AudioManager` to reliably act as singletons.

### Why it's harder than it looks

The pattern itself is a few lines:

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

But Unity adds a few sharp edges that don't show up until something breaks at runtime:

- **Execution order isn't guaranteed.** Because `instance` is resolved lazily (on first access, via `FindAnyObjectByType`), *when* another script first touches `GameManager.instance` matters. If that happens before `GameManager`'s own `Awake()` has run, you get the object reference back, but any fields it was supposed to initialize in `Awake()` may still be at their default values. This class of bug is quiet — no error, just wrong behavior — which makes it slower to track down than a null reference.
- **"Singleton" doesn't automatically mean "persistent."** Only `AudioManager` calls `DontDestroyOnLoad(gameObject)`. `GameManager` and `UIManager` don't — meaning they're really scoped to *whatever scene currently has one placed in it*, not a single object that survives scene loads. That's a meaningful distinction that's easy to gloss over: code that assumes `GameManager.instance` is the *same* object across a scene transition (the way `AudioManager.instance` is) would be wrong.
- **No duplicate-instance guard on two of the three.** `AudioManager` explicitly checks for and destroys a duplicate:

  ```csharp
  if (_instance != null && _instance != this)
  {
      Destroy(gameObject);
      return;
  }
  ```

  `GameManager` and `UIManager` don't have this check. As long as each scene only ever has one of each manager placed in it, that's fine — but it means the "singleton" guarantee is really being upheld by scene setup discipline, not by the code itself.
- **Domain reload weirdness in the Editor.** Static fields like `_instance` can persist across the transition into/out of Play mode depending on Editor settings (Enter Play Mode Options), occasionally leaving a stale reference to a destroyed object until the next domain reload clears it. This is a Unity-Editor-specific gotcha, not a build-time issue, but it's confusing the first time you hit it.

### What this taught

- A "singleton" in Unity is really two related but separate guarantees — *only one instance exists* and *that instance survives what you need it to survive* — and it's worth being deliberate about which one you actually need per-manager, rather than copy-pasting the same boilerplate everywhere.
- Lazy-lookup singletons (`FindAnyObjectByType` on first access) trade upfront setup for a subtle dependency on scene composition and execution order — a manually-wired reference or a bootstrapped-in-`Awake` approach would remove that ambiguity at the cost of more setup.
- If this project keeps growing, the next step would be pulling the repeated singleton boilerplate into a shared generic base class (`Singleton<T>`) with the duplicate-guard built in once, rather than three hand-rolled copies that can drift out of sync with each other — which is exactly what happened here between `AudioManager` and the other two.

## Other Rough Edges

A few smaller things worth documenting for future-you (or anyone reading this repo):

- **Cursor lock state management** is currently spread across `PlayerMovement`, `UIManager`, and `GameManager`, each setting `Cursor.lockState`/`Cursor.visible` independently depending on context (pause, game over, level complete, gameplay). It works, but it's easy to introduce a state where the cursor is in the wrong mode after a specific sequence of pause/resume/scene-reload actions.
- **`Wallrunning.cs`** is currently a stub — the intent was to extend the movement system to support wall-running, but it hasn't been built out yet.

---

_Have more war stories from building this? Add them above — this page is meant to grow with the project, not just capture a snapshot._
