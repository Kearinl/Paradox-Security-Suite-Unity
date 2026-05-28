using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    public class ParadoxSecuritySuiteWindow : EditorWindow
    {
        private Vector2 scroll;

        // ----------------- Section foldouts -----------------
        private bool showBackup = true;               // Expanded by default
        private bool showWhitelist = false;           // Collapsed
        private bool showObfuscation = false;         // Collapsed
        private bool showObfuscationLog = false;      // Collapsed
        private bool showIntegrity = false;           // Collapsed
        private bool showAntiCheat = false;           // Collapsed
        private bool showObfuscatorSettings = false;  // Collapsed
        private bool showFAQ = false;                 // Collapsed

        private const string PatchNumber = "1.0.0"; // Update this for new patches

        [MenuItem("Tools/Paradox Security Suite/Control Panel %#e")] // Ctrl+Shift+E
        public static void ShowWindow()
        {
            GetWindow<ParadoxSecuritySuiteWindow>("Paradox Security Suite");
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);

            // ----------------- PATCH / VERSION INFO (Top) -----------------
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Paradox Security Suite - Patch: {PatchNumber}", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            DrawBackupSection();
            DrawWhitelistSection();
            DrawObfuscationSection();
            DrawObfuscationLogSection();
            DrawIntegritySection();
            DrawAntiCheatSection();
            DrawObfuscatorSettingsSection();
            DrawFAQSection();

            EditorGUILayout.EndScrollView();
        }

        #region Sections

        private void DrawBackupSection()
        {
            GUI.backgroundColor = new Color(0.2f, 0.6f, 0.9f);
            showBackup = EditorGUILayout.Foldout(showBackup, "💾 BACKUP", true);
            GUI.backgroundColor = Color.white;

            if (!showBackup) return;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Create a backup of your Assets folder before obfuscation.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(new GUIContent("Export Backup (.unitypackage)", "Exports the entire Assets folder as a Unity package for backup.")))
            {
                ExportBackup();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawWhitelistSection()
        {
            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.4f, 0.7f, 0.3f);
            showWhitelist = EditorGUILayout.Foldout(showWhitelist, "📜 WHITELIST", true);
            GUI.backgroundColor = Color.white;

            if (!showWhitelist) return;

            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button(new GUIContent("Open Whitelist File", "Opens the whitelist.json file for editing.")))
            {
                OpenWhitelistFile();
            }
            EditorGUILayout.LabelField($"Whitelist entries: {GetWhitelistCount()}", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
        }

        private void DrawObfuscationSection()
        {
            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.8f, 0.6f, 0.2f);
            showObfuscation = EditorGUILayout.Foldout(showObfuscation, "📂 FILE OBFUSCATION", true);
            GUI.backgroundColor = Color.white;

            if (!showObfuscation) return;

            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button(new GUIContent("Obfuscate Files (Ctrl+Shift+O)", "Obfuscates all valid project files.")))
            {
                ObfuscationManager.ObfuscateFilesOnly();
                Debug.Log("✅ Obfuscation of files completed.");
            }

            if (GUILayout.Button(new GUIContent("Fix All Asset Object Names", "Fixes all main asset object names to match their file names.")))
            {
                ObfuscationManager.FixAllAssetMainObjectNames();
                Debug.Log("🛠 Asset main object names fixed.");
            }

            if (GUILayout.Button(new GUIContent("Obfuscate GameObjects In Scene", "Obfuscates all GameObjects in the currently loaded scenes.")))
            {
                ObfuscationManager.ObfuscateSceneGameObjectsOnly();
                Debug.Log("🔀 GameObject obfuscation complete.");
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawObfuscationLogSection()
        {
            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.7f, 0.5f, 0.9f);
            showObfuscationLog = EditorGUILayout.Foldout(showObfuscationLog, "📄 OBFUSCATION LOG", true);
            GUI.backgroundColor = Color.white;

            if (!showObfuscationLog) return;

            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button(new GUIContent("Open Obfuscation Log", "Opens the obfuscation_log.txt file to view changes.")))
            {
                OpenObfuscationLog();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawIntegritySection()
        {
            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.6f, 0.4f, 0.8f);
            showIntegrity = EditorGUILayout.Foldout(showIntegrity, "🔐 INTEGRITY & HASHING", true);
            GUI.backgroundColor = Color.white;

            if (!showIntegrity) return;

            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button(new GUIContent("Generate SHA-256 Hashes", "Generates SHA-256 hashes for all supported project files.")))
            {
                GenerateHashes();
            }

            if (GUILayout.Button(new GUIContent("Validate SHA-256 Hashes", "Checks all files against the generated hashes.")))
            {
                CheckIntegrity();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawAntiCheatSection()
        {
            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.9f, 0.4f, 0.4f);
            showAntiCheat = EditorGUILayout.Foldout(showAntiCheat, "🛡 ANTI-CHEAT", true);
            GUI.backgroundColor = Color.white;

            if (!showAntiCheat) return;

            EditorGUILayout.BeginVertical("box");
            bool current = AntiCheatManager.AntiCheatEnabled;
            bool enabled = EditorGUILayout.Toggle("Enable Anti-Cheat", current);
            if (enabled != current)
            {
                AntiCheatManager.SetAntiCheatEnabled(enabled);
                Debug.Log($"🛡 Anti-Cheat {(enabled ? "Enabled" : "Disabled")}");
            }
            EditorGUILayout.LabelField("Prevents external injectors and debuggers from tampering with the game at runtime.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();
        }

        private void DrawObfuscatorSettingsSection()
        {
            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.5f, 0.6f, 0.9f);
            showObfuscatorSettings = EditorGUILayout.Foldout(showObfuscatorSettings, "⚙ OBFUSCATOR SETTINGS", true);
            GUI.backgroundColor = Color.white;

            if (!showObfuscatorSettings) return;

            EditorGUILayout.BeginVertical("box");

            var settings = ObfuscatorSettings.Instance;
            if (settings == null)
            {
                EditorGUILayout.LabelField("⚠ Unable to load ObfuscatorSettings.asset", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                settings.preserveStringLiterals = EditorGUILayout.Toggle("Preserve String Literals", settings.preserveStringLiterals);
                settings.dryRunMode = EditorGUILayout.Toggle("Dry Run Mode", settings.dryRunMode);
                settings.enableLogging = EditorGUILayout.Toggle("Enable Logging", settings.enableLogging);
                settings.useAES = EditorGUILayout.Toggle("Use AES Encryption", settings.useAES);

                settings.encryptTypes = EditorGUILayout.TextField("Encrypt File Types", settings.encryptTypes);
                settings.obfuscationSeed = EditorGUILayout.TextField("Obfuscation Seed", settings.obfuscationSeed);

                settings.aesKey = EditorGUILayout.TextField("AES Key", settings.aesKey);
                settings.aesIV = EditorGUILayout.TextField("AES IV", settings.aesIV);
                settings.xorKey = EditorGUILayout.TextField("XOR Key", settings.xorKey);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(settings);
                    AssetDatabase.SaveAssets();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawFAQSection()
        {
            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.9f, 0.7f, 0.3f);
            showFAQ = EditorGUILayout.Foldout(showFAQ, "❓ FAQ / HELP", true);
            GUI.backgroundColor = Color.white;

            if (!showFAQ) return;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Q: Why should I backup before obfuscation?", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("A: Obfuscation renames files and objects. Backup ensures you can restore original assets if something goes wrong.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Q: What files are obfuscated?", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("A: Only supported source and asset files like .cs, .prefab, .mat, .shader, etc.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Q: How do I whitelist files/folders?", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("A: Open the whitelist file using the button above and add filenames or folder paths you want to exclude.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Q: Do third-party assets need special handling?", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("A: Assets like Steamworks, plugins, or other third-party folders might need to be added to the whitelist to continue functioning properly after obfuscation.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Q: How do I restore original files?", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("A: Use the backup you exported before obfuscation.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Helper Methods

        private void ExportBackup()
        {
            string path = EditorUtility.SaveFilePanel("Export Backup", "", "AssetsBackup.unitypackage", "unitypackage");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.ExportPackage("Assets", path, ExportPackageOptions.Recurse);
                Debug.Log($"💾 Backup exported to: {path}");
                EditorUtility.DisplayDialog("Backup Complete", $"Assets backup exported successfully to:\n{path}", "OK");
            }
        }

        private void OpenWhitelistFile()
        {
            string path = Path.Combine(Application.dataPath, "ParadoxSecuritySuite/Data/whitelist.json");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "{\n  \"entries\": []\n}");
                AssetDatabase.Refresh();
                Debug.Log("📄 Created new whitelist.json file.");
            }
            EditorUtility.OpenWithDefaultApp(path);
            Debug.Log("📂 Opened whitelist.json for editing.");
        }

        private void OpenObfuscationLog()
        {
            string path = Path.Combine(Application.dataPath, "ParadoxSecuritySuite/Data/obfuscation_log.txt");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, ""); // create empty log if missing
                AssetDatabase.Refresh();
                Debug.Log("📄 Created new obfuscation_log.txt file.");
            }
            EditorUtility.OpenWithDefaultApp(path);
            Debug.Log("📂 Opened obfuscation_log.txt for viewing.");
        }

        private int GetWhitelistCount()
        {
            var whitelist = WhitelistManager.Load();
            return whitelist != null ? whitelist.Count : 0;
        }

        private void GenerateHashes()
        {
            string[] allPaths = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories);
            var validPaths = new List<string>();
            string excludePath = Path.GetFullPath("Assets/ParadoxSecuritySuite/Data/integrity_hash.json").Replace("\\", "/");

            string[] validExtensions = new[]
            {
                ".cs", ".json", ".txt", ".xml", ".csv", ".bytes", ".ini", ".resx",
                ".shader", ".mat", ".asset", ".prefab", ".controller", ".anim",
                ".overrideController", ".unity", ".scene", ".physicMaterial", ".spriteatlas",
                ".fbx", ".obj", ".dae", ".3ds", ".dxf", ".blend", ".skp", ".stl",
                ".png", ".jpg", ".jpeg", ".tga", ".psd", ".bmp", ".tiff",
                ".wav", ".mp3", ".ogg", ".aiff", ".mixer", ".mask",
                ".mp4", ".mov", ".webm",
                ".ttf", ".otf"
            };

            foreach (var path in allPaths)
            {
                string fullPath = Path.GetFullPath(path).Replace("\\", "/");
                string ext = Path.GetExtension(fullPath).ToLowerInvariant();
                if (fullPath == excludePath) continue;
                if (System.Array.Exists(validExtensions, e => e == ext))
                    validPaths.Add(fullPath);
            }

            IntegrityHashManager.GenerateHashes(validPaths.ToArray());
            EditorUtility.DisplayDialog("Hashing Complete", $"SHA-256 hashes generated for {validPaths.Count} file(s).", "OK");
            Debug.Log($"🔐 Generated hashes for {validPaths.Count} files.");
        }

        private void CheckIntegrity()
        {
            bool valid = IntegrityHashManager.Validate();
            EditorUtility.DisplayDialog("Integrity Check", valid ? "✅ All files match hashes!" : "❌ File tampering detected!", "OK");
            Debug.Log("🔎 Integrity check completed.");
        }

        #endregion
    }
}
