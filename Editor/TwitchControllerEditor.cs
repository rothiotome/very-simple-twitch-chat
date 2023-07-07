using UnityEditor;
using UnityEngine;

namespace TwitchChat
{
    [CustomEditor(typeof(TwitchController))]
    public class TwitchControllerEditor : Editor
    {
        private GUIStyle greenLabelStyle = new GUIStyle(EditorStyles.label);
        private GUIStyle redLabelStyle = new GUIStyle(EditorStyles.label);

        private SerializedProperty isConnectedToIRCProp;
        private SerializedProperty hasJoinedChannelProp;
        
        private void OnEnable()
        {
            greenLabelStyle.normal.textColor = Color.green;
            redLabelStyle.normal.textColor = Color.red;

            isConnectedToIRCProp = serializedObject.FindProperty("isConnectedToIRC");
            hasJoinedChannelProp = serializedObject.FindProperty("hasJoinedChannel");
        }

        public override void OnInspectorGUI()
        {
            GUIStyle greenLabelStyle = new GUIStyle(EditorStyles.label);
            greenLabelStyle.normal.textColor = Color.green;

            GUIStyle redLabelStyle = new GUIStyle(EditorStyles.label);
            redLabelStyle.normal.textColor = Color.red;

            TwitchController twitchController = (TwitchController)target;
            EditorGUILayout.LabelField("Connected to IRC",
                twitchController.isConnectedToIRC ? "Connected" : "Not Connected",
                twitchController.isConnectedToIRC ? greenLabelStyle : redLabelStyle);
            EditorGUILayout.LabelField("Channel Joined", twitchController.hasJoinedChannel ? "YAY" : "Not yet",
                twitchController.hasJoinedChannel ? greenLabelStyle : redLabelStyle);
        }
    }
}