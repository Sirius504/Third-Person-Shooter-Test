using System;
using UnityEngine;
using Zenject;

namespace Test.Model
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        private Transform target;
        private Settings settings = new Settings();

        [Inject]
        public void Construct(Transform target, Settings settings)
        {
            this.target = target;
        }

        public void Update()
        {
            float xAngleDelta = Input.GetAxis("Mouse X") * settings.sensitivity;
            float yAngleDelta = CalculateYAngleDelta();
            transform.RotateAround(target.position, Vector3.up, xAngleDelta);
            transform.RotateAround(target.position, transform.TransformDirection(Vector3.right), yAngleDelta);
            transform.LookAt(target);
        }

        private float CalculateYAngleDelta()
        {
            float yAngleDelta = Input.GetAxis("Mouse Y") * settings.sensitivity;
            float currentAngle = Vector3.Angle(transform.position - target.position, Vector3.up);
            // To prevent camera from rotating overhead, we clamp yAngleDelta
            float maxDeltaValue = currentAngle - settings.cameraIndentDegrees;
            float minDeltaValue = currentAngle - (180f - settings.cameraIndentDegrees);
            yAngleDelta = Mathf.Clamp(yAngleDelta, minDeltaValue, maxDeltaValue);
            return yAngleDelta;
        }

        [Serializable]
        public class Settings
        {
            public float sensitivity = 0.2f;
            // Indent from top and bottom position for camera (on Y axis)
            public float cameraIndentDegrees = 0.1f;
        }
    }
}
