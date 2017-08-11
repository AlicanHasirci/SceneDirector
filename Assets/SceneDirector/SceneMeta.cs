using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace SceneDirector {
    [CustomEditor(typeof(SceneMeta))]
    public class SceneMetaInspector : Editor {
        
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            GUILayout.Space(5f);
            if (GUILayout.Button("Get Current Camera")) {
                var meta = (SceneMeta) target;
                var camera = Camera.main;
                meta.Position = camera.transform.position;
                meta.Rotation = camera.transform.eulerAngles;
                meta.Fov = camera.fieldOfView;
            }
        }
    }

}
#endif

namespace SceneDirector {
    [CreateAssetMenu(menuName = "Peak Director/Scene Meta")]
    public class SceneMeta : ScriptableObject {
        public SceneField SceneField;
        public float Fov = 60;
        public Vector3 Position;
        public Vector3 Rotation;
        public bool IsAdditive;
        public LoadingScene LoadingScene;

        public string Scene {
            get { return SceneField.Name; }
        }
    }
}
