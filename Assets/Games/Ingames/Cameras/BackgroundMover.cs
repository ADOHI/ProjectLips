using Cinemachine;
using PL.Systems.Grids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Ingames.Backgrounds
{
    public class BackgroundMover : MonoBehaviour
    {
        private Camera camera => Camera.main;
        private Vector3 baseForward;
        public CinemachineVirtualCamera cinemachineCamera;
        public float baseWidth => ((float)(GridMapManager.Instance.width - 1) * 0.5f);
        public float depth;
        public float height;
        public float distance = 50;
        public float reactivity;

        public GameObject background;


        // Update is called once per frame
        void Update()
        {
            baseForward = new Vector3((float)((GridMapManager.Instance.width) * 0.5f), 0f, (float)((GridMapManager.Instance.height) * 0.5f)) - new Vector3(baseWidth, height, depth);
            baseForward = baseForward.normalized;
            cinemachineCamera.transform.position = new Vector3(baseWidth, height, depth);

            AttachBackground();
        }

        void AttachBackground()
        {
            var angle = Vector3.Angle(baseForward, transform.forward);
            var correctedForward = Vector3.LerpUnclamped(baseForward, camera.transform.forward, angle * reactivity);

            background.transform.position = transform.position + correctedForward * distance;
            background.transform.forward = -transform.forward;
        }
    }

}
