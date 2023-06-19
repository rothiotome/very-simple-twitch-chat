using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TwitchController))]
public class TwitchControllerEditor : Editor
{
    private SerializedProperty autoConnect;
    private SerializedProperty defaultChannelName;
    private SerializedProperty secondsToRetry;
    private SerializedProperty debugMode;

    void OnEnable()
    {
        autoConnect = serializedObject.FindProperty("autoConnect");
        defaultChannelName = serializedObject.FindProperty("defaultChannelName");
        secondsToRetry = serializedObject.FindProperty("secondsToRetry");
        debugMode = serializedObject.FindProperty("debugMode");
    }

    public override void OnInspectorGUI()
    {
        TwitchController twitchController = (TwitchController)target;
        autoConnect.boolValue = EditorGUILayout.Toggle("Auto Connect", autoConnect.boolValue);

        if (autoConnect.boolValue)
        {
            defaultChannelName.stringValue = EditorGUILayout.TextField("Channel Name", defaultChannelName.stringValue);
        }

        secondsToRetry.floatValue = EditorGUILayout.FloatField("Seconds To Retry", secondsToRetry.floatValue);
        debugMode.boolValue = EditorGUILayout.Toggle("Debug Mode", debugMode.boolValue);

        GUIStyle greenLabelStyle = new GUIStyle(EditorStyles.label);
        greenLabelStyle.normal.textColor = Color.green;

        GUIStyle redLabelStyle = new GUIStyle(EditorStyles.label);
        redLabelStyle.normal.textColor = Color.red;

        EditorGUILayout.LabelField("Connected to IRC",
            twitchController.isConnectedToIRC ? "Connected" : "Not Connected",
            twitchController.isConnectedToIRC ? greenLabelStyle : redLabelStyle);
        EditorGUILayout.LabelField("Channel Joined", twitchController.hasJoinedChannel ? "YAY" : "Not yet",
            twitchController.hasJoinedChannel ? greenLabelStyle : redLabelStyle);

        serializedObject.ApplyModifiedProperties();
    }
}