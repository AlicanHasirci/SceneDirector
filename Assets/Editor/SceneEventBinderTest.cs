using NSubstitute;
using NUnit.Framework;
using SceneDirector;

[TestFixture]
public class SceneEventBinderTest {

    private SceneEventBinder _eventBinder;
    private Scene1Mock _object1;
    private Scene2Mock _object2;
    private string _scene1 = "Scene1";

    [SetUp]
    public void SetUp() {
        _eventBinder = new SceneEventBinder();
        _object1 = Substitute.For<Scene1Mock>();
        _object2 = Substitute.For<Scene2Mock>();
    }

    [Test]
    public void AddingSceneEventsTest() {
        _eventBinder.AddSceneBindings(true, _object1);
        _eventBinder.Dispatch(_scene1, SceneEventType.Loaded);
        _object1.Received(1).Scene1Loaded();
    }

    [Test]
    public void AllEventsTest() {
        _eventBinder.AddSceneBindings(true, _object1);
        _eventBinder.Dispatch(_scene1, SceneEventType.Loading);
        _eventBinder.Dispatch(_scene1, SceneEventType.Loaded);
        _eventBinder.Dispatch(_scene1, SceneEventType.Shown);
        _eventBinder.Dispatch(_scene1, SceneEventType.Unloading);
        _eventBinder.Dispatch(_scene1, SceneEventType.Unloaded);
        _object1.Received(1).Scene1Loading();
        _object1.Received(1).Scene1Loaded();
        _object1.Received(1).Scene1Shown();
        _object1.Received(1).Scene1Unloading();
        _object1.Received(1).Scene1Unloaded();
    }

    [Test]
    public void DuplicateObjectEventsTest() {
        _eventBinder.AddSceneBindings(_object1, _object1, _object1);
        _eventBinder.Dispatch(_scene1, SceneEventType.Loaded);
        _object1.Received(1).Scene1Loaded();
    }

    [Test]
    public void RemoveObjectTest() {
        _eventBinder.AddSceneBindings(_object1);
        _eventBinder.Dispatch(_scene1, SceneEventType.Loaded);
        _object1.Received(1).Scene1Loaded();

        _object1.ClearReceivedCalls();
        _eventBinder.RemoveSceneBindings(_object1);
        _eventBinder.Dispatch(_scene1, SceneEventType.Loaded);
        _object1.DidNotReceive().Scene1Loaded();
    }

    [Test]
    public void MultipleRemoveTest() {
        _eventBinder.AddSceneBindings(_object1, _object2);
        _eventBinder.RemoveSceneBindings(_object1, _object2);
    }

    public class Scene1Mock {
        [SceneEvent(SceneEventType.Loading, "Scene1")]
        public virtual void Scene1Loading() {}
        [SceneEvent(SceneEventType.Loaded, "Scene1")]
        public virtual void Scene1Loaded() {}
        [SceneEvent(SceneEventType.Shown, "Scene1")]
        public virtual void Scene1Shown() {}
        [SceneEvent(SceneEventType.Unloading, "Scene1")]
        public virtual void Scene1Unloading() {}
        [SceneEvent(SceneEventType.Unloaded, "Scene1")]
        public virtual void Scene1Unloaded() {}
    }

    public class Scene2Mock {
        [SceneEvent(SceneEventType.Loading, "Scene2")]
        public virtual void Scene2Loading() {}
        [SceneEvent(SceneEventType.Loaded, "Scene2")]
        public virtual void Scene2Loaded() {}
        [SceneEvent(SceneEventType.Shown, "Scene2")]
        public virtual void Scene2Shown() {}
        [SceneEvent(SceneEventType.Unloading, "Scene2")]
        public virtual void Scene2Unloading() {}
        [SceneEvent(SceneEventType.Unloaded, "Scene2")]
        public virtual void Scene2Unloaded() {}
    }
}