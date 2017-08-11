using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace SceneDirector {
    public class SceneEventBinder : ISceneEventBinder {
        private readonly Dictionary<string, SceneEvents> _sceneEvents;
        private readonly Dictionary<Type, List<ReflectionData>> _reflectionDatas;

        public SceneEventBinder() {
            _sceneEvents = new Dictionary<string, SceneEvents>();
            _reflectionDatas = new Dictionary<Type, List<ReflectionData>>();
        }

        public void AddSceneBindings(params object[] objects) {
            HandleSceneBindings(true, objects);
        }

        public void RemoveSceneBindings(params object[] objects) {
            HandleSceneBindings(false, objects);
        }

        public void Dispatch(string scene, SceneEventType sceneEventType) {
            SceneEvents sceneEvents;
            if (_sceneEvents.TryGetValue(scene, out sceneEvents)) {
                sceneEvents.Dispatch(sceneEventType);
            }
        }

        private void HandleSceneBindings(bool toAdd, params object[] objects) {
            foreach (var obj in objects) {
                var reflectionDatas = GetReflectionData(obj.GetType());
                foreach (var data in reflectionDatas) {
                    if (toAdd) {
                        AssignDelegate(obj, data);
                    }
                    else {
                        RemoveDelegate(obj, data);
                    }
                }
            }
        }

        private List<ReflectionData> GetReflectionData(Type type) {
            List<ReflectionData> data;
            if (!_reflectionDatas.TryGetValue(type, out data)) {
                data = new List<ReflectionData>();
                foreach (var methodInfo in type.GetMethods()) {
                    var attribute =
                        Attribute.GetCustomAttribute(methodInfo, typeof(SceneEventAttribute)) as SceneEventAttribute;
                    if (attribute != null) {
                        data.Add(new ReflectionData() {
                            Attribute = attribute,
                            MethodInfo = methodInfo
                        });
                    }
                }
                _reflectionDatas.Add(type, data);
            }
            return data;
        }

        private void RemoveDelegate(object target, ReflectionData reflectionData) {
            SceneEvents sceneEvents;
            if (!_sceneEvents.TryGetValue(reflectionData.Attribute.Scene, out sceneEvents)) return;

            sceneEvents.Remove(
                reflectionData.Attribute.SceneEventType,
                (Action)Delegate.CreateDelegate(typeof(Action), target, reflectionData.MethodInfo));
        }

        private void AssignDelegate(object target, ReflectionData reflectionData) {
            SceneEvents events;
            if (!_sceneEvents.TryGetValue(reflectionData.Attribute.Scene, out events)) {
                events = new SceneEvents();
                _sceneEvents.Add(reflectionData.Attribute.Scene, events);
            }
            events.Add(
                reflectionData.Attribute.SceneEventType,
                (Action) Delegate.CreateDelegate(typeof(Action), target, reflectionData.MethodInfo));
        }

        #region Nested Classes & Structs
        private class SceneEvents {
            private readonly Dictionary<SceneEventType, Binding> _eventBindings = new Dictionary<SceneEventType, Binding>();

            public void Dispatch(SceneEventType sceneEventType) {
                Binding binding;
                if (_eventBindings.TryGetValue(sceneEventType, out binding)) {
                    binding.Dispatch();
                }
            }

            public void Add(SceneEventType sceneEventType, Delegate del) {
                Binding binding;
                if (!_eventBindings.TryGetValue(sceneEventType, out binding)) {
                    binding = new Binding();
                    _eventBindings.Add(sceneEventType, binding);
                }
                binding.Add(del);
            }

            public void Remove(SceneEventType sceneEventType, Delegate del) {
                Binding binding;
                if (_eventBindings.TryGetValue(sceneEventType, out binding)) {
                    binding.Remove(del);
                }
            }
        }

        private class Binding {
            private event Action Action;

            public void Dispatch() {
                if (Action != null) {
                    Action();
                }
            }

            public void Add(Delegate del) {
                if (Action == null || (Action != null && !Action.GetInvocationList().Contains(del))) {
                    Action += (Action) del;
                }
            }

            public void Remove(Delegate del) {
                if (Action != null && Action.GetInvocationList().Contains(del)) {
                    Action -= (Action) del;
                }
            }
        }

        private struct ReflectionData {
            public MethodInfo MethodInfo;
            public SceneEventAttribute Attribute;
        }
        #endregion
    }
}
