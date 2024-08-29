using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using System;
using System.Collections;
using UnityEditor;

public class ImageUploadHandler : MonoBehaviour
{
    public GameObject continueButton;
    public Button uploadButton;
    public Image previewImage; 

    public string uploadDirectory = "C:\\Users\\kendl\\OneDrive\\Desktop\\ARProject\\Assets/uploads";
    public Text feedbackText; 

    private string imagePath;

    void Start()
    {
        uploadButton.onClick.AddListener(OnUploadButtonClick);
        continueButton.SetActive(false);

    }

    void OnUploadButtonClick()
    {
        string path = EditorUtility.OpenFilePanel("Upload an Image", "", "png,jpg,jpeg");
        if (path.Length != 0)
        {
            // Set a fixed filename for the uploaded image
            string fixedFileName = "colored_map.png";
            imagePath = Path.Combine(uploadDirectory, fixedFileName);
          
            // Copy the selected image to the target directory with the fixed filename
            File.Copy(path, imagePath, true);
           // UnityEngine.Debug.Log("Image uploaded to: " + imagePath);
            // Display the image in the UI
            StartCoroutine(LoadImage(imagePath));

            // Force Unity to recognize the new file
            AssetDatabase.Refresh();
            // Run the Python script with the fixed image path
            // Provide feedback
            feedbackText.text = "Image uploaded and converted successfully.";

            RunPythonScript(imagePath);
            AssetDatabase.Refresh();
            //make the continue button visible when the image is uploaded
           continueButton.SetActive(true);
        }
    }
    IEnumerator LoadImage(string filePath)
    {
        WWW www = new WWW("file:///" + filePath);
        yield return www;
        Texture2D texture = new Texture2D(2, 2);
        www.LoadImageIntoTexture(texture);
        previewImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    void RunPythonScript(string imagePath)
    {
        //string imageFullPath = Path.GetFullPath(imagePath);
        string pythonPath = @"C:\Python311\python.exe";
       // string scriptFullPath = Path.GetFullPath(scriptPath);
       string scriptFullPath = @"C:\Users\kendl\OneDrive\Desktop\ARProject\Assets\generate_heightmap.py";
        string imageFullPath = @"C:\Users\kendl\OneDrive\Desktop\ARProject\Assets\uploads\colored_map.png";


        if (!File.Exists(scriptFullPath))
        {
            UnityEngine.Debug.LogError("Python script not found: " + scriptFullPath);
            return;
        }

        string outputImagePath = Path.Combine(Application.dataPath, "models/heightmap.png");
        //string outputImageFullPath = Path.GetFullPath(outputImagePath);

        string outputImageFullPath = @"\models";

        string command = $"\"{pythonPath}\" \"{scriptFullPath}\" \"{imageFullPath}\" \"{outputImageFullPath}\"";

      //s  UnityEngine.Debug.Log("Command: " + command);

        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe", // Use PowerShell for compatibility
            Arguments = $"-Command \"{command}\"",
            WorkingDirectory = Path.GetDirectoryName(scriptFullPath),
            EnvironmentVariables = { ["PYTHONPATH"] = @"C:\Python311" },

            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();

                // Wait for the process to exit
                process.WaitForExit();

                // Read the output and error streams
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                   UnityEngine.Debug.Log("Python Output: " + result);
                }

                using (StreamReader reader = process.StandardError)
                {
                    string error = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(error))
                    {
                        UnityEngine.Debug.LogError("Python Error: " + error);
                    }
                }

                // Check if the process has exited and get the exit code
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