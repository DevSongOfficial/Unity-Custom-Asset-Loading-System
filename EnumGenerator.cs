using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


namespace CustomResourceManagement
{
    #if UNITY_EDITOR
    public class EnumGenerator : EditorWindow
    {
        // Selected scrip that saves generated enums.
        private string scriptPath;
        private string scriptName;

        private Stack<StringBuilder> previousSources = new Stack<StringBuilder>();
        string editorLogMessage = string.Empty;

        private StringBuilder GenerateNewScript(bool saveThisScript = true)
        {
            var sourceCode = new StringBuilder("// This script's automatically generated by EnumGenerator.cs\n");
            sourceCode.AppendLine($"namespace {typeof(EnumGenerator).Namespace}\n{{");

            var folderInfo = new DirectoryInfo($"{Application.dataPath}/Resources");
            ProcessFolder(folderInfo, sourceCode);

            sourceCode.AppendLine("}"); 

            return GenerateNewScript(sourceCode, saveThisScript);
        }

        private void ProcessFolder(DirectoryInfo directoryInfo, StringBuilder sourceCode, int callCount = 0)
        {
            var subFolders = directoryInfo.GetDirectories(); 
            var tabSpace = new string('\t', callCount);

            if (subFolders.Length > 0)
            {
                if (callCount > 0)
                {
                    var structName = directoryInfo.Name;
                    sourceCode.AppendLine($"{tabSpace}public struct {structName}{{");
                }
                 
                foreach (var subFolder in subFolders)
                    ProcessFolder(subFolder, sourceCode, callCount + 1);


                if (callCount > 0)
                    sourceCode.AppendLine($"{tabSpace}}}");

            }
            else
            {
                var enumName = directoryInfo.Name;
                sourceCode.AppendLine($"{tabSpace}public enum {enumName}{{");

                var files = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if (file.Extension != ".meta")
                    {
                        var enumMember = file.Name.Replace(file.Extension, string.Empty);
                        sourceCode.AppendLine($"{tabSpace}\t{enumMember},");
                    }
                }

                sourceCode.AppendLine($"{tabSpace}}}");
            }
        }

        private StringBuilder GenerateNewScript(StringBuilder sourceCode, bool saveThisScript = true)
        {
            if (File.Exists(scriptPath))
                File.WriteAllText(scriptPath, string.Empty); // This is how you should clear contents of a text file. (better than File.Delete();)
            else
            {
                editorLogMessage = " <color=red>File Missing!</color>";
                return null;
            }

            File.AppendAllText(scriptPath, sourceCode + "\n");


            if (!saveThisScript)
            {
                editorLogMessage = " <color=cyan>Reverted Successfully</color>";
                return null;
            }

            AssetDatabase.Refresh();

            return sourceCode;
        }

        [MenuItem("Window/Custom Asset Manager")]
        public static void ShowWindow()
        {
            GetWindow(typeof(EnumGenerator));
        }

        void OnGUI()
        {
            if (!string.IsNullOrEmpty(scriptPath))
            {
                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.normal.textColor = Color.green;

                GUILayout.Label($"Selected File: {scriptName}.cs", labelStyle);
            }
            else
            {
                GUILayout.Label("Drag & Drop C# script here", EditorStyles.boldLabel);
            }

            Rect labelRect = GUILayoutUtility.GetLastRect();

            HandleDragAndDrop(labelRect);

            GUIStyle logStyle = new GUIStyle();
            logStyle.fontStyle = FontStyle.Italic;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontStyle = FontStyle.Bold;

            GUILayout.Label(editorLogMessage, logStyle);

            if (GUILayout.Button("Save", buttonStyle))
            {
                editorLogMessage = " <color=yellow>Saving...</color>";
                var newScript = GenerateNewScript();

                if (newScript != null)
                {
                    editorLogMessage = " <color=cyan>Saved Successfully</color>";
                    previousSources.Push(newScript);
                } 
            }

            if (GUILayout.Button("Revert", buttonStyle))
            {
                if (previousSources.Count == 0)
                {
                    editorLogMessage = " <color=orange>Can't revert - no longer have saved data";
                    return;
                }

                GenerateNewScript(previousSources.Pop(), saveThisScript: false);
            }
        }

        private void HandleDragAndDrop(Rect dropArea)
        {
            Event currentEvent = Event.current;

            if (currentEvent.type != EventType.DragUpdated && currentEvent.type != EventType.DragPerform) return;
            if (!dropArea.Contains(currentEvent.mousePosition)) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (currentEvent.type != EventType.DragPerform) return;

            DragAndDrop.AcceptDrag();
            currentEvent.Use();

            foreach (var draggedObject in DragAndDrop.objectReferences)
            {
                if (draggedObject is MonoScript script)
                {
                    scriptPath = AssetDatabase.GetAssetPath(script);
                    scriptName = script.name;
                }
            }
        }
    }
    #endif
}
