using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace SceneDirector {
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            SerializedProperty sceneAsset = property.FindPropertyRelative("Asset");
            SerializedProperty sceneName = property.FindPropertyRelative("Name");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneAsset != null) {
                sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue,
                    typeof(SceneAsset), false);
                if (sceneAsset.objectReferenceValue != null) {
                    sceneName.stringValue = ((SceneAsset) sceneAsset.objectReferenceValue).name;
                }
            }
            EditorGUI.EndProperty();
        }
    }
}
#endif

namespace SceneDirector {
    [Serializable]
    public class SceneField {
    #if UNITY_EDITOR
        public SceneAsset Asset;
    #endif
        public string Name;
    }
}