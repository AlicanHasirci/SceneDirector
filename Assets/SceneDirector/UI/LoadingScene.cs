using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace SceneDirector {
    [CustomPropertyDrawer(typeof(LoadingScene))]
    public class LoadingSceneFieldDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            SerializedProperty enabled = property.FindPropertyRelative("Enabled");
            var enabledRect = new Rect(position.x, position.y, position.width, position.height);
            enabled.boolValue = EditorGUI.Toggle(enabledRect, "Loading Enabled",enabled.boolValue);
            if (enabled.boolValue) {
                SerializedProperty scene = property.FindPropertyRelative("Scene");
                SerializedProperty duration = property.FindPropertyRelative("Duration");
                var sceneRect = new Rect(position.x, position.y + 20, position.width, 16f);
                var durationRect = new Rect(position.x, position.y + 40, position.width, 16f);
                EditorGUI.PropertyField(sceneRect, scene);
                duration.floatValue = EditorGUI.FloatField(durationRect, "Animation Duration",duration.floatValue);

            }
            EditorGUI.EndProperty( );
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty enabled = property.FindPropertyRelative("Enabled");
            return enabled.boolValue ? 35f : 16f;
        }
    }
}
#endif

namespace SceneDirector {
    [Serializable]
    public struct LoadingScene {
        public bool Enabled;
        public SceneField Scene;
        public float Duration;
    }
}
