using UnityEngine;

namespace TransitionsPlus.Demos {

    public class MoveSphere : MonoBehaviour {

        public Vector3 destination;

        Vector3 originalPosition;

        private void Start() {
            originalPosition = transform.position;
        }

        void Update() {
            transform.position = Vector3.Lerp(originalPosition, destination, Mathf.PingPong(Time.time, 1));
        }
    }

}