using System;

namespace SceneDirector {
    public enum SceneEventType {
        Loading = 0,
        Loaded,
        Shown,
        Unloading,
        Unloaded
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SceneEventAttribute : Attribute {
        public SceneEventType SceneEventType { get; set; }
        public string Scene { get; set; }
        public int Priority { get; set; }
    
        public SceneEventAttribute(SceneEventType sceneEventType, string scene, int priority = 0) {
            SceneEventType = sceneEventType;
            Scene = scene;
            Priority = priority;
        }
    }
}



