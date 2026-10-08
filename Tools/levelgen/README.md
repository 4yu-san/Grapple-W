# levelgen

Generates Grapple-W levels with a local LLM and writes them as JSON that `LevelBuilder` loads in Unity.
The game itself never talks to a model, so shipped builds (including Quest) stay offline.

## Setup

```
pip install -r requirements.txt
```

Install a local model runner. Ollama is the default:

```
ollama pull qwen2.5:14b
```

Pick a model that fits your VRAM. Larger models give better layouts; the validator and retry loop catch the mistakes small ones make.

## Generate

```
python generate.py --count 5 --difficulty 3 --length 10 --theme "neon rooftops" --model qwen2.5:14b
```

Output goes to `Assets/Resources/Levels/`. Useful flags:

| flag | meaning |
|---|---|
| `--difficulty 1-5` | higher values require more grapple-only transitions |
| `--length N` | steps including start and goal |
| `--backend openai --host http://localhost:1234` | LM Studio or a llama.cpp server |
| `--seed N` | reproducible numbering and sampling seed |
| `--mock` | skip the LLM, use the procedural planner (pipeline test) |

## How it works

1. The model returns a chain of steps (`schema.py`), each placed relative to the previous one. Ollama's `format` / OpenAI `json_schema` constrains the output to the schema.
2. `layout.py` converts the chain to absolute positions.
3. `validate.py` checks sizes, bounds, overlaps, headroom, and that every transition is reachable by jump or grapple (with line of sight). Failures go back to the model as a list of specific problems, up to `--retries` times.
4. Valid levels are saved. Check any file later with `python validate.py path/to/level.json`.

## Tuning

`movement_profile.json` holds the player limits the validator and prompt use. The numbers are conservative guesses. Playtest a few levels, then adjust `jumpGapMax`, `jumpUpMax` and `grappleRange` to match how the controller actually feels.

## Unity setup

1. Create the registry: Assets > Create > Grapple-W > Level Prefab Registry. Assign `Ground` as the block prefab and `WinWall` as the goal trigger prefab.
2. In a scene that has the player, camera and managers (duplicate `SampleScene` and delete its level geometry), add an empty object with `LevelBuilder`. Assign the registry and the player transform.
3. Leave `Level Name` empty for a random level, or enter a file name without `.json`.
4. Press Play, or right-click the component and choose "Build Level".
