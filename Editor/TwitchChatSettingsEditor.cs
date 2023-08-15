using UnityEditor;
using UnityEngine;

    [CustomEditor(typeof(TwitchChatSettings))]
    public class TwitchChatSettingsEditor : Editor
    {
        private bool showPassword = false;
        private bool showInternalSettings = false;

        private bool initStyle = false;

        private static readonly Color redColor = new Color(0.99f, 0.45f, 0.45f);

        private SerializedProperty debugModeProp;
        private SerializedProperty preserveAcrossScenesProp;
        private SerializedProperty secondsToRetryProp;
        private SerializedProperty maxRetryProp;
        private SerializedProperty useAnonymousConnectionProp;
        private SerializedProperty usernameProp;
        private SerializedProperty oAuthTokenProp;
        private SerializedProperty portProp;
        private SerializedProperty serverProp;
        private SerializedProperty anonUsernameProp;
        private SerializedProperty anonPasswordProp;
        private SerializedProperty useRandomColorForUndefinedProp;

        private GUIStyle redLabelStyle;

        private void OnEnable()
        {
            debugModeProp = serializedObject.FindProperty("debugMode");
            preserveAcrossScenesProp = serializedObject.FindProperty("preserveAcrossScenes");
            secondsToRetryProp = serializedObject.FindProperty("secondsToRetry");
            maxRetryProp = serializedObject.FindProperty("maxRetry");
            useAnonymousConnectionProp = serializedObject.FindProperty("useAnonymousConnection");
            usernameProp = serializedObject.FindProperty("username");
            oAuthTokenProp = serializedObject.FindProperty("oAuthToken");
            portProp = serializedObject.FindProperty("port");
            serverProp = serializedObject.FindProperty("server");
            anonUsernameProp = serializedObject.FindProperty("anonUsername");
            anonPasswordProp = serializedObject.FindProperty("anonPassword");
            useRandomColorForUndefinedProp = serializedObject.FindProperty("useRandomColorForUndefined");
        }

        public override void OnInspectorGUI()
        {
            if (!initStyle)
            {
                InitStyle();
                initStyle = true;
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(debugModeProp,
                new GUIContent("Debug / Verbose mode",
                    "Enables de Debug mode. All the messages received from Twitch will be printed in the console"));
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Connection Settings can't be changed on PlayMode.", MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(preserveAcrossScenesProp,
                new GUIContent("Preserve across Scenes",
                    "Once you connect to the Twitch Channel, the connection will remain active across the scenes"));
            EditorGUILayout.PropertyField(useRandomColorForUndefinedProp,
                new GUIContent("Use Random Color For Undefined",
                    "Return a Random default Twitch color instead white when the color is undefined"));
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Retry Connection");


            EditorGUILayout.BeginVertical();

            EditorGUILayout.PropertyField(secondsToRetryProp,
                new GUIContent("Seconds to retry",
                    "Time in seconds to retry the connection if it fails during login"));
            if (secondsToRetryProp.floatValue < 0.5f) secondsToRetryProp.floatValue = 0.5f;

            EditorGUILayout.PropertyField(maxRetryProp,
                new GUIContent("Max retries",
                    "How many times the plugin tries to connect to the twitch server before stopping"));
            if (maxRetryProp.intValue < 0) maxRetryProp.intValue = 0;

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(useAnonymousConnectionProp,
                new GUIContent("Use Anonymous Connection",
                    "Enables de use of Anonymous Connection. There's no need of OAuth token, but it's less stable and only has reading permission"));
            if (!useAnonymousConnectionProp.boolValue)
            {
                EditorGUILayout.BeginVertical("HelpBox");
                EditorGUILayout.PropertyField(usernameProp,
                    new GUIContent("Username",
                        "It has to be the same account that authorizes the App and retrieved the OAuthToken"));

                EditorGUILayout.BeginHorizontal();
                oAuthTokenProp.stringValue = showPassword
                    ? EditorGUILayout.TextField(new GUIContent("OAuth Token", "Retrieved from the App authorization"),
                        oAuthTokenProp.stringValue)
                    : EditorGUILayout.PasswordField(
                        new GUIContent("OAuth Token", "Retrieved from the App authorization"),
                        oAuthTokenProp.stringValue);

                showPassword = EditorGUILayout.ToggleLeft(
                    new GUIContent("Show Token", "Caution: Token should be private"),
                    showPassword, redLabelStyle, GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

            }

            EditorGUILayout.Space();
            if (GUILayout.Button(showInternalSettings ? "Hide Internal Settings" : "Show Internal Settings"))
                showInternalSettings = !showInternalSettings;

            if (showInternalSettings)
            {
                EditorGUILayout.BeginVertical("HelpBox");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(
                    "Caution: This information is not intended for modification. Any alterations made are done so at your own risk.",
                    MessageType.Warning);
                if (!EditorPrefs.HasKey("TwitchAcceptedRisk"))
                {
                    if (GUILayout.Button("I understand the Risk"))
                    {
                        EditorPrefs.SetBool("TwitchAcceptedRisk", true);
                    }

                    EditorGUI.BeginDisabledGroup(true);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(portProp, new GUIContent("Port"));
                EditorGUILayout.PropertyField(serverProp, new GUIContent("Server"));
                EditorGUILayout.PropertyField(anonUsernameProp, new GUIContent("Anonymous connection username"));
                EditorGUILayout.PropertyField(anonPasswordProp, new GUIContent("Anonymous connection password"));

                EditorGUILayout.EndVertical();
            }

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void InitStyle()
        {
            redLabelStyle = new GUIStyle(EditorStyles.label);
            redLabelStyle.normal.textColor = redColor;
            redLabelStyle.focused.textColor = redColor;
            redLabelStyle.hover.textColor = redColor;
        }
    }