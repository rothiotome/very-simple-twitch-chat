using UnityEditor;
using UnityEngine;

public class TwitchSettingsWindowEditor : EditorWindow
{
    private TwitchSettings defaultTwitchSettings;
    private TwitchSettingsEditor twitchSettingsEditor;
    
    private bool defaultValuesFoldout;

    [MenuItem("Window/Twitch Chat/Setup")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TwitchSettingsWindowEditor window = (TwitchSettingsWindowEditor) GetWindow(typeof(TwitchSettingsWindowEditor));
        window.Show();
        window.titleContent = new GUIContent("Twitch Chat Setup");
        window.LoadOrCreateTwitchSettings();
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        defaultValuesFoldout = EditorGUILayout.Foldout(defaultValuesFoldout,
            new GUIContent("Change default values", "This settings will be stored in the resources folder"), true);
        
        // Get the existing editor for defaultTwitchSettings
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
        defaultTwitchSettings = Resources.Load<TwitchSettings>("defaultTwitchSettings");
        
        // If the asset doesn't exist, create a new one and store it
        if (defaultTwitchSettings == null)
        {
            defaultTwitchSettings = CreateInstance<TwitchSettings>();
            // Optionally, you can initialize defaultTwitchSettings with default values
            // defaultTwitchSettings.InitializeDefaultValues();

            // Create a new asset file and save defaultTwitchSettings to Resources folder
            AssetDatabase.CreateAsset(defaultTwitchSettings, "Assets/Resources/defaultTwitchSettings.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        twitchSettingsEditor = Editor.CreateEditor(defaultTwitchSettings) as TwitchSettingsEditor;
    }
}
