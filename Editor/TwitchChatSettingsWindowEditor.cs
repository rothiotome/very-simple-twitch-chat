using UnityEditor;
using UnityEngine;

namespace VerySimpleTwitchChat
{

    public class TwitchChatSettingsWindowEditor : EditorWindow
    {
        private TwitchChatSettings defaultChatTwitchSettings;
        private TwitchChatSettingsEditor twitchSettingsEditor;

        private bool defaultValuesFoldout;

        [MenuItem("Window/Twitch Chat/Setup")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            TwitchChatSettingsWindowEditor window =
                (TwitchChatSettingsWindowEditor)GetWindow(typeof(TwitchChatSettingsWindowEditor));
            window.Show();
            window.titleContent = new GUIContent("Twitch Chat Setup");
            window.LoadOrCreateTwitchSettings();
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            defaultValuesFoldout = EditorGUILayout.Foldout(defaultValuesFoldout,
                new GUIContent("Change default values", "This settings will be stored in the resources folder"), true);

            // Get the existing editor for defaultChatTwitchSettings
            if (defaultValuesFoldout)
            {
                EditorGUI.indentLevel++;
                twitchSettingsEditor.OnInspectorGUI();

            }
        }

        void LoadOrCreateTwitchSettings()
        {
            string resourcesFolderPath = "Assets/Resources/";
            if (!AssetDatabase.IsValidFolder(resourcesFolderPath))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.Refresh();
            }

            // Attempt to load the TwitchSettings asset from Resources folder
            defaultChatTwitchSettings = Resources.Load<TwitchChatSettings>("defaultTwitchChatSettings");

            // If the asset doesn't exist, create a new one and store it
            if (defaultChatTwitchSettings == null)
            {
                defaultChatTwitchSettings = CreateInstance<TwitchChatSettings>();
                // Optionally, you can initialize defaultChatTwitchSettings with default values
                // defaultChatTwitchSettings.InitializeDefaultValues();

                // Create a new asset file and save defaultChatTwitchSettings to Resources folder
                AssetDatabase.CreateAsset(defaultChatTwitchSettings,
                    "Assets/Resources/defaultTwitchChatSettings.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            twitchSettingsEditor = Editor.CreateEditor(defaultChatTwitchSettings) as TwitchChatSettingsEditor;
        }
    }
}