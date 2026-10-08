using UnityEngine;

// Maps level piece types to the prefabs and materials LevelBuilder spawns.
// Create it with Assets > Create > Grapple-W > Level Prefab Registry.
[CreateAssetMenu(fileName = "LevelPrefabRegistry", menuName = "Grapple-W/Level Prefab Registry")]
public class LevelPrefabRegistry : ScriptableObject
{
    [Header("Prefabs")]
    [Tooltip("A unit cube with a BoxCollider, e.g. the existing Ground prefab. Its scale is overwritten per piece.")]
    [SerializeField] private GameObject blockPrefab;
    [Tooltip("Trigger volume with LevelEnd on it, e.g. the existing WinWall prefab. Optional.")]
    [SerializeField] private GameObject goalTriggerPrefab;

    [Header("Layer")]
    [Tooltip("Layer the blocks go on. Must be part of the grapple gun's whatIsGrappleable and the player's whatIsGround.")]
    [SerializeField] private string blockLayerName = "Ground";

    [Header("Materials (optional)")]
    [SerializeField] private Material platformMaterial;
    [SerializeField] private Material pillarMaterial;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material goalMaterial;

    public GameObject GetBlockPrefab()
    {
        return blockPrefab;
    }

    public GameObject GetGoalTriggerPrefab()
    {
        return goalTriggerPrefab;
    }

    public string GetBlockLayerName()
    {
        return blockLayerName;
    }

    public Material GetMaterial(string pieceType)
    {
        switch (pieceType)
        {
            case "platform": return platformMaterial;
            case "pillar": return pillarMaterial;
            case "wall": return wallMaterial;
            case "goal": return goalMaterial;
            default: return null;
        }
    }
}
