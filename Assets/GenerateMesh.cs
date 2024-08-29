using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMesh : MonoBehaviour
{
    public Button ContinueButton;
    public string imagePath = "C:\\Users\\kendl\\OneDrive\\Desktop\\ARProject\\Assets/models/heightmap.png";
    public string meshPath = "C:\\Users\\kendl\\OneDrive\\Desktop\\ARProject\\Assets/Resources/terrain_model.fbx";
    public float checkInterval = 1.0f; // Check every 1 second
    public float timeout = 30.0f; // Timeout after 30 seconds

    void Start()
    {
        ContinueButton.onClick.AddListener(OnContinueButtonClick);
    }

    void OnContinueButtonClick()
    {
        // Generate the mesh
        GenerateMeshFromImage();
    }

    void GenerateMeshFromImage()
    {
        // Run the Python script asynchronously
        Task.Run(() => RunPythonScript());

        // Start checking for the generated FBX file
        StartCoroutine(CheckForGeneratedMesh());
    }

    IEnumerator CheckForGeneratedMesh()
    {
        float elapsedTime = 0f;

        while (elapsedTime < timeout)
        {
            if (File.Exists(meshPath))
            {
                // The FBX file is found, refresh assets and change the scene
                AssetDatabase.Refresh();
                UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
                yield break;
            }

            // Wait for the next check interval
            yield return new WaitForSeconds(checkInterval);
            elapsedTime += checkInterval;
        }

        // If the loop exits, it means the file wasn't found within the timeout
        UnityEngine.Debug.LogError("Failed to find the generated FBX file within the timeout period.");
    }

    void RunPythonScript()
    {
        string blenderExecutable = @"C:\Program Files\Blender Foundation\Blender 4.0\blender.exe";
        string pythonPath = @"C:\Python311\python.exe";

        string scriptPath = @"C:\Users\kendl\OneDrive\Desktop\ARProject\Assets\blender_script.py";
        string heightmapPath = @"C:\Users\kendl\OneDrive\Desktop\ARProject\Assets\models\heightmap.png";
        string outputPath = @"C:\Users\kendl\OneDrive\Desktop\ARProject\Assets\Resources\terrain_model.fbx";

        // Build the command string with PowerShell & operator and correct quotes
        string command = $"& \"{blenderExecutable}\" --background --python \"{scriptPath}\" -- \"{heightmapPath}\" \"{outputPath}\"";

        UnityEngine.Debug.Log("Command: " + command);

        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
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
}
