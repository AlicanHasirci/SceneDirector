using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SceneDirector {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class SceneDirector : MonoBehaviour {
        private IDictionary<string, SceneMeta> _sceneMetas;
        private IEnumerator _dequeueCoroutine;
        private ISceneEventBinder _eventBinder;
        private Queue<IEnumerator> _sceneChanges;

        public SceneField FirstScene;
        public string TopScene { get { return SceneManager.GetSceneAt(StackCount - 1).name; } }
        public string PreviousScene { get { return SceneManager.GetSceneAt(StackCount - 2).name; } }
        public int StackCount { get { return SceneManager.sceneCount; } }

        private void Awake() {
            _eventBinder = new SceneEventBinder();
            _sceneChanges = new Queue<IEnumerator>();
            
            var metas = Resources.LoadAll<SceneMeta>("");
            _sceneMetas = new Dictionary<string, SceneMeta>(metas.Length);
            
            SceneMeta firstSceneMeta = null;
            foreach (var meta in metas) {
                _sceneMetas.Add(meta.Scene, meta);
                if (meta.Scene == TopScene) firstSceneMeta = meta;
            }
            
            if (firstSceneMeta != null) {
                ChangeActiveScene(firstSceneMeta.name);
            }
        }

        public void ChangeScene (string scene, IDictionary<string, object> param = null) {
            if (scene == TopScene) return;
            SceneMeta target;
            if (_sceneMetas.TryGetValue(scene, out target)) {
                EnqueueSceneChange(SceneChangeCoroutine(target));
            }
        }

        public void UnloadTopScene(IDictionary<string, object> param = null) {
            if (StackCount > 2)
                EnqueueSceneChange(UnloadTopSceneCoroutine ());
        }

        private void EnqueueSceneChange(IEnumerator sceneChange) {
            _sceneChanges.Enqueue(sceneChange);
            if (_dequeueCoroutine == null) {
                _dequeueCoroutine = DequeueCoroutine();
                StartCoroutine(_dequeueCoroutine);
            }
        }

        private void ChangeActiveScene(string scene) {
            SceneMeta meta;
            if (_sceneMetas.TryGetValue(scene, out meta)) {
                var parentCamera = GetComponent<Camera>();
                parentCamera.transform.position = meta.Position;
                parentCamera.transform.eulerAngles = meta.Rotation;
                parentCamera.fieldOfView = meta.Fov;
                
                var sceneObject = SceneManager.GetSceneByName(meta.Scene);
                SceneManager.SetActiveScene(sceneObject);
                
                _eventBinder.Dispatch(meta.Scene, SceneEventType.Shown);
            }
        }

        private void HandleBindings(string scene, Action<object[]> bindingMethod) {
            var rootGameObjects = SceneManager.GetSceneByName(scene).GetRootGameObjects();
            foreach (var o in rootGameObjects) {
                var components = o.GetComponents<MonoBehaviour>();
                // ReSharper disable once CoVariantArrayConversion
                bindingMethod(components);
            }
        }

        private IEnumerator DequeueCoroutine() {
            while  (_sceneChanges.Count > 0) {
                yield return _sceneChanges.Dequeue();
            }
            _dequeueCoroutine = null;
        }

        private IEnumerator UnloadTopSceneCoroutine() {
            yield return UnloadSceneCoroutine (TopScene);
            ChangeActiveScene(TopScene);
        }

        private IEnumerator SceneChangeCoroutine(SceneMeta targetScene) {
            var loadingScene = targetScene.LoadingScene;
            if (StackCount > 2 && loadingScene.Enabled) {
                yield return LoadSceneCoroutine(loadingScene.Scene.Name);
                _eventBinder.Dispatch(loadingScene.Scene.Name, SceneEventType.Shown);
                yield return new WaitForSeconds(loadingScene.Duration);
                if (!targetScene.IsAdditive)
                    yield return UnloadSceneCoroutine(PreviousScene);
                yield return LoadSceneCoroutine(targetScene.Scene);
                yield return new WaitForSeconds(loadingScene.Duration);
                yield return UnloadSceneCoroutine(loadingScene.Scene.Name);
            }
            else {
                yield return LoadSceneCoroutine(targetScene.Scene);
                if (StackCount > 2 && !targetScene.IsAdditive)
                    yield return UnloadSceneCoroutine(PreviousScene);
            }
            ChangeActiveScene(TopScene);
        }

        private IEnumerator LoadSceneCoroutine(string scene) {
            _eventBinder.Dispatch(scene, SceneEventType.Loading);
            var operation = SceneManager.LoadSceneAsync (scene, LoadSceneMode.Additive);
            while (!operation.isDone) {
                yield return null;
            }

            HandleBindings(scene, _eventBinder.AddSceneBindings);

            _eventBinder.Dispatch(scene, SceneEventType.Loaded);

        }

        private IEnumerator UnloadSceneCoroutine (string scene) {
            _eventBinder.Dispatch(scene, SceneEventType.Unloading);

            HandleBindings(scene, _eventBinder.RemoveSceneBindings);

            var unloadScene = SceneManager.UnloadSceneAsync(scene);
            while (!unloadScene.isDone) {
                yield return null;
            }

            var unloadAssets = Resources.UnloadUnusedAssets();
            while (!unloadAssets.isDone) {
                yield return null;
            }

            _eventBinder.Dispatch(scene, SceneEventType.Unloaded);
        }
    }
}