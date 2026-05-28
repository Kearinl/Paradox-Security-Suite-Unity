using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HappyDayzGames.ParadoxSecuritySuite
{
    /// <summary>
    /// Stores settings for the Obfuscator Tool.
    /// </summary>
    [CreateAssetMenu(fileName = "ObfuscatorSettings", menuName = "Obfuscator/Settings")]
    public class ObfuscatorSettings : ScriptableObject
    {
        [Header("Obfuscation Options")]
        public bool preserveStringLiterals = true;
        public bool dryRunMode = false;
        public bool enableLogging = true;
        public bool useAES = true;
        public bool useSnakeCase = false;
        public string encryptTypes = ".json,.txt,.xml";
        public string obfuscationSeed = "default_seed";

        [Header("Encryption Keys")]
        public string aesKey = "MyStrongKey123456";  // Must be 16/24/32 characters
        public string aesIV = "InitializationVe";    // Must be 16 characters
        public string xorKey = "SimpleKey";

        private static ObfuscatorSettings _instance;

        /// <summary>
        /// Singleton instance of ObfuscatorSettings.
        /// </summary>
        public static ObfuscatorSettings Instance
        {
            get
            {
#if UNITY_EDITOR
                if (_instance == null)
                    _instance = LoadOrCreate();
                return _instance;
#else
                Debug.LogError("ObfuscatorSettings.Instance is only available in the Unity Editor.");
                return null;
#endif
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Loads existing settings asset or creates a new one if not found.
        /// </summary>
        public static ObfuscatorSettings LoadOrCreate()
        {
            const string path = "Assets/ParadoxSecuritySuite/Settings/ObfuscatorSettings.asset";

            var settings = AssetDatabase.LoadAssetAtPath<ObfuscatorSettings>(path);
            if (settings == null)
            {
                settings = CreateInstance<ObfuscatorSettings>();
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();
                Debug.Log("✅ Created new ObfuscatorSettings.asset at: " + path);
            }

            return settings;
        }
#endif
    }
}
