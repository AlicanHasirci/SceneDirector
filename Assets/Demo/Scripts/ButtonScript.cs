using SceneDirector;
using UnityEngine;

namespace Demo.Scripts {
	public class ButtonScript : MonoBehaviour {
		public void ChangeScene(string scene) {
			Camera.main.ChangeScene(scene);
		}
	}
}
