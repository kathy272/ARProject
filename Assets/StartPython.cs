using UnityEditor;
using UnityEditor.Scripting.Python;

public class MenuItem_StartPython_Class
{
   [MenuItem("Python Scripts/StartPython")]
   public static void StartPython()
   {
       PythonRunner.RunFile("Assets/StartPython.py");
       }
};
