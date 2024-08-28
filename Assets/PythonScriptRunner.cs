using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class PythonScriptRunner : MonoBehaviour
{
    [MenuItem("Python Scripts/Run Python Script")]
    public static void RunPythonScript()
    {
       
        string pythonPath = @"C:\Python311\python.exe"; 
        string scriptPath = @"Assets/automate_conversion.py"; // Path to Python script

     
        string command = $"\"{pythonPath}\" \"{scriptPath}\"";

        // Execute the command
        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {command}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(processInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                UnityEngine.Debug.Log(result);
            }

            using (StreamReader reader = process.StandardError)
            {
                string error = reader.ReadToEnd();
                UnityEngine.Debug.LogError(error);
            }
        }
    }
}
