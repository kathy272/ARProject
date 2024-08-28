using UnityEngine;
using System.Diagnostics;
using System.IO;

public class HeightmapGenerator : MonoBehaviour
{
    public string pythonPath = "path/to/python"; // Path to Python executable
    public string scriptPath = "path/to/generate_heightmap.py"; // Path to your Python script

    public void GenerateHeightmap(string inputImagePath)
    {
        string outputImagePath = Path.Combine(Application.dataPath, "models/heightmap.png");

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = pythonPath;
        start.Arguments = string.Format("{0} \"{1}\"", scriptPath, inputImagePath);
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                UnityEngine.Debug.Log(result);
            }

            using (StreamReader reader = process.StandardError)
            {
                string error = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    UnityEngine.Debug.LogError(error);
                }
            }

            process.WaitForExit();
        }

        // Optionally, load the heightmap in Unity
        LoadHeightmap(outputImagePath);
    }

    private void LoadHeightmap(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);

        // Use the texture, e.g., apply to a terrain
        // Example: GetComponent<Renderer>().material.mainTexture = tex;
    }
}
