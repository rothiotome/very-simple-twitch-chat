using UnityEditor;
using UnityEngine;

namespace VerySimpleTwitchChat
{
    [CustomEditor(typeof(TwitchChat))]
    public class TwitchChatEditor : Editor
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

            TwitchChat twitchChat = (TwitchChat)target;
            EditorGUILayout.LabelField("Connected to IRC",
                twitchChat.isConnectedToIRC ? "Connected" : "Not Connected",
                twitchChat.isConnectedToIRC ? greenLabelStyle : redLabelStyle);
            EditorGUILayout.LabelField("Channel Joined", twitchChat.hasJoinedChannel ? "YAY" : "Not yet",
                twitchChat.hasJoinedChannel ? greenLabelStyle : redLabelStyle);
        }
    }
}