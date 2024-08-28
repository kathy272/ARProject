using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMesh : MonoBehaviour
{
    public Button ContinueButton;
    public string imagePath = "Assets/models/heightmap.png";
    public string meshPath = "Assets/Resources/terrain_model.fbx";

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
        // Load the image (heightmap)
       // string heightmapPath = imagePath;

        // Run the Python script asynchronously
        Task.Run(() => RunPythonScript());
        AssetDatabase.Refresh();
        //change the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
    }

    void RunPythonScript()
    {
        string blenderExecutable = @"C:\Program Files\Blender Foundation\Blender 4.0\blender.exe";
        string pythonPath = @"C:\Python311\python.exe";

        string scriptPath = @"C:\Users\kendl\OneDrive\Documents\UnityProjects\ProjectBA\BAProject_kendlbacher\Assets\blender_script.py";
        string heightmapPath = @"C:\Users\kendl\OneDrive\Documents\UnityProjects\ProjectBA\BAProject_kendlbacher\Assets\models\heightmap.png";
        string outputPath = @"C:\Users\kendl\OneDrive\Documents\UnityProjects\ProjectBA\BAProject_kendlbacher\Assets\Resources\terrain_model.fbx";

        // Build the command string with proper quoting
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

}
