using System.Collections.Generic;
using UnityEngine;

namespace SceneDirector {
    public static class PeakDirectorExtension {
        public static void ChangeScene (this Camera camera, string scene, IDictionary<string, object> parameters = null) {
            SceneDirector director = camera.GetComponent<SceneDirector>();
            director.ChangeScene(scene, parameters);
        }

        public static void UnloadTopScene (this Camera camera, IDictionary<string, object> parameters = null) {
            SceneDirector director = camera.GetComponent<SceneDirector>();
            director.UnloadTopScene(parameters);
        }

        public static string GetTopScene (this Camera camera) {
            SceneDirector director = camera.GetComponent<SceneDirector>();
            return director.TopScene;
        }

        public static string GetPreviousScene (this Camera camera) {
            SceneDirector director = camera.GetComponent<SceneDirector>();
            return director.PreviousScene;
        }
    }
}