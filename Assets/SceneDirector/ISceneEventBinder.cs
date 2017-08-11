namespace SceneDirector {
    public interface ISceneEventBinder {
        void AddSceneBindings(params object[] objects);
        void RemoveSceneBindings(params object[] objects);
        void Dispatch(string scene, SceneEventType sceneEventType);
    }
}