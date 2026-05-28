using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    /// <summary>
    /// Handles file and GameObject name obfuscation within Unity projects.
    /// </summary>
    public static class ObfuscationManager
    {
        public static void ObfuscateFilesOnly()
        {
            var settings = ObfuscatorSettings.Instance;

            string[] allPaths = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories);
            var whitelist = WhitelistManager.Load();
            var alreadyObfuscated = LogManager.LoadObfuscatedFiles();
            var renamedClasses = new Dictionary<string, string>();

            string[] validExtensions = new[]
            {
                ".cs", ".shader", ".mat", ".asset", ".prefab", ".controller", ".anim",
                ".overrideController", ".physicMaterial", ".spriteatlas",
                ".fbx", ".obj", ".dae", ".3ds", ".dxf", ".blend", ".skp", ".stl",
                ".png", ".jpg", ".jpeg", ".tga", ".psd", ".bmp", ".tiff",
                ".wav", ".mp3", ".ogg", ".aiff", ".mp4", ".mov", ".webm", ".mixer", ".mask",
                ".ttf", ".otf"
            };

            int counter = 0;

            foreach (string fullPath in allPaths)
            {
                string extension = Path.GetExtension(fullPath).ToLowerInvariant();
                string fileName = Path.GetFileName(fullPath);

                if (!System.Array.Exists(validExtensions, e => e == extension)) continue;
                if (whitelist.Contains(fileName) || alreadyObfuscated.Contains(fileName)) continue;
                if (WhitelistManager.IsWhitelisted(fullPath) || alreadyObfuscated.Contains(fileName)) continue;

                string assetOld = ToAssetPath(fullPath);
                string content = File.ReadAllText(fullPath);

                if (extension == ".cs")
                {
                    string className = GetClassName(content);
                    if (string.IsNullOrEmpty(className)) continue;

                    string obfuscatedName = GenerateObfuscatedName(className, settings.useSnakeCase, settings.obfuscationSeed);
                    renamedClasses[className] = obfuscatedName;

                    string newFileName = obfuscatedName + ".cs";
                    string newPath = Path.Combine(Path.GetDirectoryName(fullPath), newFileName);
                    string newFullPath = Path.GetFullPath(newPath);

                    string oldMeta = fullPath + ".meta";
                    string newMeta = newFullPath + ".meta";

                    if (!settings.dryRunMode)
                    {
                        try
                        {
                            File.Move(fullPath, newFullPath);
                            if (File.Exists(oldMeta)) File.Move(oldMeta, newMeta);

                            content = settings.preserveStringLiterals
                                ? ReplacePreservingStrings(content, className, obfuscatedName)
                                : content.Replace(className, obfuscatedName);

                            File.WriteAllText(newFullPath, content);
                            AssetDatabase.Refresh();

                            Debug.Log($"✅ Renamed .cs class and file: {assetOld} → {ToAssetPath(newFullPath)}");
                            LogManager.LogAction($"Renamed class and file: {className} → {obfuscatedName}");

                            UpdateMonoBehaviourScriptReferences(assetOld, ToAssetPath(newFullPath));
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError($"❌ Failed to rename .cs file: {ex.Message}");
                        }
                    }
                    else
                    {
                        Debug.Log($"🟡 Dry run: Would rename .cs class and file: {fileName} → {obfuscatedName}.cs");
                        LogManager.LogAction($"[DryRun] Renamed class and file: {className} → {obfuscatedName}");
                    }
                }
                else
                {
                    string obfuscatedName = GenerateObfuscatedName(fileName, settings.useSnakeCase, settings.obfuscationSeed);
                    string newFileName = obfuscatedName + extension;
                    string newFullPath = Path.Combine(Path.GetDirectoryName(fullPath), newFileName);

                    string oldMeta = fullPath + ".meta";
                    string newMeta = newFullPath + ".meta";

                    if (!settings.dryRunMode)
                    {
                        try
                        {
                            File.Move(fullPath, newFullPath);
                            if (File.Exists(oldMeta)) File.Move(oldMeta, newMeta);
                            Debug.Log($"✅ Renamed file: {assetOld} → {ToAssetPath(newFullPath)}");
                            LogManager.LogAction($"Renamed file: {fileName} → {newFileName}");
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError($"❌ Failed to rename file: {ex.Message}");
                        }
                    }
                    else
                    {
                        Debug.Log($"🟡 Dry run: Would rename file: {fileName} → {obfuscatedName}{extension}");
                        LogManager.LogAction($"[DryRun] Renamed file: {fileName}");
                    }
                }

                counter++;
            }

            if (!settings.dryRunMode && renamedClasses.Count > 0)
            {
                foreach (string filePath in Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories))
                {
                    string content = File.ReadAllText(filePath);
                    foreach (var entry in renamedClasses)
                        content = ReplacePreservingStrings(content, entry.Key, entry.Value);
                    File.WriteAllText(filePath, content);
                }
                Debug.Log("✅ Updated all class references in .cs files.");
            }
            else if (settings.dryRunMode && renamedClasses.Count > 0)
            {
                Debug.Log("🟡 Dry run: Would update class references across all .cs files:");
                foreach (var entry in renamedClasses)
                    Debug.Log($"🟡 Would replace: {entry.Key} → {entry.Value}");
            }

            EditorUtility.DisplayDialog("Obfuscator Finished", $"✅ File obfuscation complete.\n{counter} file(s) processed.", "OK");
            AssetDatabase.Refresh();
        }

        public static void FixAllAssetMainObjectNames()
        {
            string[] allPaths = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories);
            int fixedCount = 0;

            foreach (string path in allPaths)
            {
                string extension = Path.GetExtension(path).ToLowerInvariant();
                if (extension == ".meta") continue;

                string assetPath = path.Replace("\\", "/");
                var mainObj = AssetDatabase.LoadMainAssetAtPath(assetPath);
                if (mainObj == null) continue;

                string expectedName = Path.GetFileNameWithoutExtension(assetPath);
                if (mainObj.name != expectedName)
                {
                    mainObj.name = expectedName;
                    EditorUtility.SetDirty(mainObj);
                    fixedCount++;
                    Debug.Log($"🛠 Fixed main object name in: {assetPath}");
                }
            }

            if (fixedCount > 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorUtility.DisplayDialog("Fix Asset Names", $"✅ Fixed {fixedCount} asset(s).", "OK");
        }

        public static void ObfuscateSceneGameObjectsOnly()
        {
            var settings = ObfuscatorSettings.Instance;
            if (settings.dryRunMode)
            {
                Debug.Log("🚫 Dry run mode is enabled. Skipping GameObject name obfuscation.");
                EditorUtility.DisplayDialog("Obfuscator Skipped", "Dry run mode is enabled.\nGameObject name obfuscation skipped.", "OK");
                return;
            }

            int renamedCount = 0;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;

                foreach (var go in scene.GetRootGameObjects())
                    renamedCount += ObfuscateGameObjectRecursive(go, settings);
            }

            EditorUtility.DisplayDialog("Obfuscator Finished", $"✅ GameObject name obfuscation complete.\nRenamed {renamedCount} GameObject(s).", "OK");
        }

        private static int ObfuscateGameObjectRecursive(GameObject go, ObfuscatorSettings settings)
        {
            int count = 0;
            string originalName = go.name;
            string obfuscatedName = GenerateObfuscatedName(originalName, settings.useSnakeCase, settings.obfuscationSeed);

            if (originalName != obfuscatedName)
            {
                go.name = obfuscatedName;
                Debug.Log($"🔀 Renamed GameObject: {originalName} → {obfuscatedName}");
                LogManager.LogAction($"Renamed GameObject: {originalName} → {obfuscatedName}");
                count++;
            }

            foreach (Transform child in go.transform)
                count += ObfuscateGameObjectRecursive(child.gameObject, settings);

            return count;
        }

        private static void UpdateMonoBehaviourScriptReferences(string oldScriptPath, string newScriptPath)
        {
            string newMetaPath = newScriptPath + ".meta";
            if (!File.Exists(newMetaPath)) return;

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;
                var sceneOpened = EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                var gameObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

                foreach (var go in gameObjects)
                {
                    var components = go.GetComponents<MonoBehaviour>();
                    foreach (var comp in components)
                    {
                        if (comp == null)
                        {
                            SerializedObject so = new SerializedObject(go);
                            SerializedProperty prop = so.FindProperty("m_Script");

                            if (prop != null && prop.propertyType == SerializedPropertyType.ObjectReference)
                            {
                                MonoScript newScript = AssetDatabase.LoadAssetAtPath<MonoScript>(newScriptPath);
                                if (newScript != null)
                                {
                                    prop.objectReferenceValue = newScript;
                                    so.ApplyModifiedProperties();
                                    Debug.Log($"🔁 Updated missing script reference on {go.name} in scene {scene.path}");
                                }
                            }
                        }
                    }
                }

                EditorSceneManager.SaveScene(sceneOpened);
            }

            AssetDatabase.Refresh();
        }

        private static string GetClassName(string code)
        {
            Match match = Regex.Match(code, @"public\s+class\s+(\w+)");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static string GenerateObfuscatedName(string baseName, bool toSnakeCase, string seed)
        {
            string hashed = "X" + Mathf.Abs((baseName + seed).GetHashCode());
            return toSnakeCase ? ToSnakeCase(hashed) : hashed;
        }

        private static string ReplacePreservingStrings(string content, string original, string replacement)
        {
            var output = new List<string>();
            var pattern = "\".*?\"";
            int lastPos = 0;

            foreach (Match m in Regex.Matches(content, pattern))
            {
                int start = m.Index;
                int end = m.Index + m.Length;
                string before = content.Substring(lastPos, start - lastPos);
                string quoted = content.Substring(start, end - start);

                output.Add(before.Replace(original, replacement));
                output.Add(quoted);
                lastPos = end;
            }

            output.Add(content.Substring(lastPos).Replace(original, replacement));
            return string.Join("", output);
        }

        private static string ToSnakeCase(string input)
        {
            return Regex.Replace(input, "(?<!^)([A-Z])", "_$1").ToLower();
        }

        private static string ToAssetPath(string fullPath)
        {
            var relative = fullPath.Replace(Application.dataPath, "").Replace("\\", "/");
            if (relative.StartsWith("/")) relative = relative.Substring(1);
            return "Assets/" + relative;
        }
    }
}
