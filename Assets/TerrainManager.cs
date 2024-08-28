using UnityEngine;
using UnityEditor;  // Import if using Unity Editor functionalities

public class TerrainManager : MonoBehaviour
{
    [SerializeField]
    private string terrainModelPath = "Assets/Resources/terrain_model.fbx"; // Path to the asset in the project

    private void OnEnable()
    {
        // Subscribe to the quitting event
        Application.quitting += OnApplicationQuit;
    }

    private void OnDisable()
    {
        // Unsubscribe from the quitting event
        Application.quitting -= OnApplicationQuit;
    }

    private void OnApplicationQuit()
    {
        DeleteTerrainModel();
    }

    private void DeleteTerrainModel()
    {
        // Ensure you are in the editor
#if UNITY_EDITOR
        if (System.IO.File.Exists(terrainModelPath))
        {
            AssetDatabase.DeleteAsset(terrainModelPath);
            Debug.Log("Terrain model asset deleted.");
        }
        else
        {
            Debug.LogWarning("Terrain model asset not found.");
        }
#endif
    }
}
