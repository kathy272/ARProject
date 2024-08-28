using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using UnityEditor.Scripting.Python;


public class ImageUploader : MonoBehaviour
{
    public RawImage imageDisplay; // UI element to show the uploaded image

    public void UploadImage()
    {
        string filePath = OpenFilePanel("Select Image", "", "png,jpg,jpeg");
        if (!string.IsNullOrEmpty(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            // Display the image (optional)
            if (imageDisplay != null)
                imageDisplay.texture = tex;

            // Pass the Texture2D to your heightmap conversion function
            ConvertToHeightmap(tex);
        }
    }

    public string OpenFilePanel(string title, string directory, string extension)
    {
        // You can use an open file dialog like OpenFileDialog in Windows or implement your own custom file dialog
        // Alternatively, use a third-party library for cross-platform support.
        return EditorUtility.OpenFilePanel(title, directory, extension);
    }

    private void ConvertToHeightmap(Texture2D texture)
    {
      
        PythonRunner.RunFile("Assets/Generate_heightmap.py");
    }
}
