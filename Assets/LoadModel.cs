using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TriLibCore;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.SceneManagement;

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

    private bool isSceneLoaded = false; // Flag to check if the scene has already been loaded
    public static LoadModel Instance { get; private set; }

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
        // Show the loading UI
        if (loadingUI != null)
        {
            loadingUI.SetActive(true);
        }

        ModelLoad();
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
        Debug.Log("Model loaded");

        // Assign the new loaded model
        spawnedObject = loadedGameObject;

        // Set the parent or other properties
        loadedGameObject.transform.SetParent(transform);

        if (objectSpawner != null)
        {
            Debug.Log("ObjectSpawner");
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
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
    }

    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Materials loaded");
    }

    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
        Debug.Log($"Progress: {progress * 100}%");

        if (loadingImage != null)
        {
            // Update the fill amount based on the progress of model loading
            loadingImage.fillAmount = progress;
        }
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Failed to load model: {contextualizedError.GetInnerException()}");

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
        Debug.Log("Loading scene: " + sceneName);

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
        Debug.Log("Scene activation: " + sceneLoading.allowSceneActivation);
    }
}
