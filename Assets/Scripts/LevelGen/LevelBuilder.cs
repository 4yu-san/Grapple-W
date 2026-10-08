using UnityEngine;

// Builds a level from a JSON file made by Tools/levelgen.
// Levels live in Assets/Resources/Levels/*.json, which Resources loads on every platform
// (including Android/Quest, where StreamingAssets is not a normal folder).
public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private LevelPrefabRegistry registry;
    [Tooltip("Optional. Moved to the level's start point after building.")]
    [SerializeField] private Transform player;

    [Header("Which level")]
    [Tooltip("File name without .json, inside Resources/Levels. Leave empty to pick a random one.")]
    [SerializeField] private string levelName = "";
    [SerializeField] private bool buildOnStart = true;

    private const string resourcesFolder = "Levels";
    private const float goalTriggerHeight = 4f;

    private GameObject levelRoot;
    private LevelData currentLevel;

    void Start()
    {
        if (buildOnStart)
            BuildFromResources();
    }

    [ContextMenu("Build Level")]
    public void BuildFromResources()
    {
        TextAsset json = FindLevelAsset();
        if (json == null)
        {
            Debug.LogError("LevelBuilder: no level JSON found in Resources/" + resourcesFolder);
            return;
        }

        LevelData data = JsonUtility.FromJson<LevelData>(json.text);
        if (data == null || data.pieces == null || data.pieces.Length == 0)
        {
            Debug.LogError("LevelBuilder: " + json.name + " is empty or malformed.");
            return;
        }

        Build(data);
    }

    public void Build(LevelData data)
    {
        if (registry == null || registry.GetBlockPrefab() == null)
        {
            Debug.LogError("LevelBuilder: assign a LevelPrefabRegistry with a block prefab.");
            return;
        }

        Clear();
        currentLevel = data;
        levelRoot = new GameObject("GeneratedLevel_" + data.name);
        levelRoot.transform.SetParent(transform, false);

        int layer = LayerMask.NameToLayer(registry.GetBlockLayerName());
        if (layer < 0)
            Debug.LogWarning("LevelBuilder: layer '" + registry.GetBlockLayerName() + "' does not exist, pieces stay on Default and cannot be grappled.");

        foreach (LevelPieceData piece in data.pieces)
            SpawnPiece(piece, layer);

        SpawnKillFloor(data);
        PlacePlayer(data);
        Debug.Log("LevelBuilder: built '" + data.name + "' (" + data.pieces.Length + " pieces)");
    }

    public void Clear()
    {
        if (levelRoot != null)
            Destroy(levelRoot);
        levelRoot = null;
        currentLevel = null;
    }

    public LevelData GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SetLevelName(string newName)
    {
        levelName = newName;
    }

    private TextAsset FindLevelAsset()
    {
        if (!string.IsNullOrEmpty(levelName))
            return Resources.Load<TextAsset>(resourcesFolder + "/" + levelName);

        TextAsset[] all = Resources.LoadAll<TextAsset>(resourcesFolder);
        if (all.Length == 0)
            return null;
        return all[Random.Range(0, all.Length)];
    }

    private void SpawnPiece(LevelPieceData piece, int layer)
    {
        Vector3 position = ToVector3(piece.position);
        Vector3 size = ToVector3(piece.size);
        Quaternion rotation = Quaternion.Euler(0f, piece.yaw, 0f);

        GameObject block = Instantiate(registry.GetBlockPrefab(), position, rotation, levelRoot.transform);
        block.name = piece.id + "_" + piece.type;
        block.transform.localScale = size;
        if (layer >= 0)
            block.layer = layer;

        Material material = registry.GetMaterial(piece.type);
        if (material != null)
        {
            Renderer blockRenderer = block.GetComponent<Renderer>();
            if (blockRenderer != null)
                blockRenderer.sharedMaterial = material;
        }

        if (piece.type == "goal")
            SpawnGoalTrigger(piece, position, size, rotation);
    }

    private void SpawnGoalTrigger(LevelPieceData piece, Vector3 blockCentre, Vector3 blockSize, Quaternion rotation)
    {
        // Sits on top of the goal platform. Stays on the Default layer so grapple rays hit the platform below.
        Vector3 triggerSize = new Vector3(blockSize.x, goalTriggerHeight, blockSize.z);
        Vector3 triggerCentre = blockCentre + Vector3.up * (blockSize.y / 2f + goalTriggerHeight / 2f);

        GameObject trigger;
        if (registry.GetGoalTriggerPrefab() != null)
        {
            trigger = Instantiate(registry.GetGoalTriggerPrefab(), triggerCentre, rotation, levelRoot.transform);
            trigger.transform.localScale = triggerSize;
        }
        else
        {
            trigger = new GameObject();
            trigger.transform.SetParent(levelRoot.transform, false);
            trigger.transform.SetPositionAndRotation(triggerCentre, rotation);
            BoxCollider box = trigger.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = Vector3.one;
            trigger.transform.localScale = triggerSize;
            trigger.AddComponent<LevelEnd>();
        }
        trigger.name = piece.id + "_goalTrigger";
    }

    private void SpawnKillFloor(LevelData data)
    {
        Vector3 centre = Vector3.zero;
        foreach (LevelPieceData piece in data.pieces)
            centre += ToVector3(piece.position);
        centre /= data.pieces.Length;
        centre.y = data.killFloorY;

        GameObject floor = new GameObject("KillFloor");
        floor.transform.SetParent(levelRoot.transform, false);
        floor.transform.position = centre;
        BoxCollider box = floor.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(2000f, 2f, 2000f);
        floor.AddComponent<LevelFailed>();
    }

    private void PlacePlayer(LevelData data)
    {
        if (player == null || data.start == null || data.start.position == null)
            return;

        player.position = ToVector3(data.start.position);
        Rigidbody body = player.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    private static Vector3 ToVector3(float[] values)
    {
        return new Vector3(values[0], values[1], values[2]);
    }
}
