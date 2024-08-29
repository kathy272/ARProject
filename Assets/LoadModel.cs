using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TriLibCore;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using UnityEditor;
using System.IO;

public class LoadModel : MonoBehaviour
{
    [SerializeField]
    private string modelPath = "Assets/Resources/terrain_model.fbx"; // Path to your model file

    [SerializeField]
    private ObjectSpawner objectSpawner; // Reference to the object spawner

    [SerializeField]
    private Image loadingImage; // Reference to the UI Image component for progress

    [SerializeField]
    private GameObject loadingUI; // Reference to the loading UI object

    private GameObject spawnedObject; // Reference to the spawned object

    [SerializeField]
    private string sceneToLoad = "SampleScene";

    private bool isSceneLoaded = false; 
    public static LoadModel Instance { get; private set; }



    public string imagePath = "C:\\Users\\kendl\\OneDrive\\Desktop\\ARProject\\Assets/models/heightmap.png";
    public string meshPath = "C:\\Users\\kendl\\OneDrive\\Desktop\\ARProject\\Assets/Resources/terrain_model.fbx";
    public float checkInterval = 1.0f; // Check every 1 second
    public float timeout = 30.0f; // Timeout after 30 seconds


    private void Awake()
    {
        // Ensure only one instance of LoadModel exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this instance across scene loads
        }
    }
    private void Start()
    {
        GenerateMeshFromImage();
    }
    void GenerateMeshFromImage()
    {

        // Run the Python script in a separate thread
        Task.Run(() => RunPythonScript());

        // Start checking for the generated FBX file
        StartCoroutine(CheckForGeneratedMesh());

    }
    IEnumerator CheckForGeneratedMesh()
    {

        UnityEngine.Debug.Log("Checking for generated mesh...");
        float elapsedTime = 0f;

        while (elapsedTime < timeout)
        {
            if (File.Exists(meshPath))
            {
                // The FBX file is found, refresh assets in the Editor and change the scene
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
                ModelLoad();
                yield break;
            }

            // Wait for the next check interval
            yield return new WaitForSeconds(checkInterval);
            AssetDatabase.Refresh();

            elapsedTime += checkInterval;
        }

        // If the loop exits, it means the file wasn't found within the timeout
        UnityEngine.Debug.LogError("Failed to find the generated FBX file within the timeout period.");
    }


    void RunPythonScript()
    {
        string blenderExecutable = @"C:\Program Files\Blender Foundation\Blender 4.0\blender.exe";
        string scriptPath = @"C:\Users\kendl\OneDrive\Desktop\ARProject\Assets\blender_script.py";
        string heightmapPath = @"C:\Users\kendl\OneDrive\Desktop\ARProject\Assets\models\heightmap.png";
        string outputPath = @"C:\Users\kendl\OneDrive\Desktop\ARProject\Assets\Resources\terrain_model.fbx";
        string pythonPath = @"C:\Python311\python.exe";

        // Build the command string, encapsulated within a script block {}
        string command = $"\"{blenderExecutable}\" --background --python \"{scriptPath}\" -- \"{heightmapPath}\" \"{outputPath}\"";

        UnityEngine.Debug.Log("Command: " + command);

        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = Process.Start(processInfo))
            {
                process.WaitForExit();

                // Read the output and error streams
                string result = process.StandardOutput.ReadToEnd();
                UnityEngine.Debug.Log("Python Output: " + result);

                string error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    UnityEngine.Debug.LogError("Python Error: " + error);
                }

                int exitCode = process.ExitCode;
                UnityEngine.Debug.Log("Process Exit Code: " + exitCode);
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Exception: {ex.Message}");
        }
    }


    public void ModelLoad()
    {
        AssetLoaderOptions options = AssetLoader.CreateDefaultLoaderOptions();
        // Load the model
        AssetLoader.LoadModelFromFile(modelPath, OnLoad, OnMaterialsLoad, OnProgress, OnError, gameObject, options);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        // Destroy the existing object if any
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }

        GameObject loadedGameObject = assetLoaderContext.RootGameObject;
        UnityEngine.Debug.Log("Model loaded");

        // Assign the new loaded model
        spawnedObject = loadedGameObject;

        // Set the parent or other properties
        loadedGameObject.transform.SetParent(transform);

        if (objectSpawner != null)
        {
            UnityEngine.Debug.Log("ObjectSpawner");
            // Optionally interact with the objectSpawner
        }

        // Hide the loading UI
        if (loadingUI != null)
        {
            loadingUI.SetActive(false);
        }

        if (!isSceneLoaded)
        {
            isSceneLoaded = true;
            AssetDatabase.Refresh();

            StartCoroutine(LoadSceneAsync(sceneToLoad));
            
        }
    }

    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        UnityEngine.Debug.Log("Materials loaded");
    }

    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
        UnityEngine.Debug.Log($"Progress: {progress * 100}%");

        if (loadingImage != null)
        {
            // Update the fill amount based on the progress of model loading
            loadingImage.fillAmount = progress;
        }
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        UnityEngine.Debug.LogError($"Failed to load model: {contextualizedError.GetInnerException()}");

        // Hide the loading UI
        if (loadingUI != null)
        {
            loadingUI.SetActive(false);
        }

        // Optionally show an error message
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Start loading the scene
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        sceneLoading.allowSceneActivation = false;
        UnityEngine.Debug.Log("Loading scene: " + sceneName);

        while (sceneLoading.progress < 0.9f)
        {
            if (loadingImage != null)
            {
                loadingImage.fillAmount = sceneLoading.progress / 0.9f;
            }

            yield return null; // Wait until the next frame
        }

        // Activate the scene once it's fully loaded
        sceneLoading.allowSceneActivation = true;
        UnityEngine.Debug.Log("Scene activation: " + sceneLoading.allowSceneActivation);
        //refresh assets

    }
}
