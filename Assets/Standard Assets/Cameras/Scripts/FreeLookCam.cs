using System;
using UnityEngine;
using Zenject;

namespace UnityStandardAssets.Cameras
{
    public class FreeLookCam : MonoBehaviour
    {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera
        [Serializable]
        public class Settings
        {
            [Range(0f, 10f)] public float sensitivity = 1.5f;   // How fast the rig will rotate from user input.
            public float turnSmoothing = 0.0f;                  // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
            public float tiltMax = 75f;                         // The maximum value of the x axis rotation of the pivot.
            public float tiltMin = 45f;                         // The minimum value of the x axis rotation of the pivot.
            public bool lockCursor = false;                     // Whether the cursor should be hidden and locked.
        }

        private Settings settings;
        private float lookAngle;                    // The rig's y axis rotation.
        private float tiltAngle;                    // The pivot's x axis rotation.
        private const float lookDistance = 100f;    // How far in front of the pivot the character's look target is.
        private Vector3 pivotEulers;
        private Quaternion pivotTargetRot;
        private Quaternion rigTargetRot;

        protected Transform target;
        protected Transform camera; // the transform of the camera
        protected Transform pivot; // the point at which the camera pivots around
        protected Transform rig;        

        [Inject]
        protected void Construct(Settings settings, Transform target, Camera cameraComponent)
        {
            this.target = target;
            this.rig = transform;
            // find the camera in the object hierarchy
            this.camera = cameraComponent.transform;
            this.pivot = this.camera.parent;
            this.settings = settings;

            Cursor.lockState = settings.lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !settings.lockCursor;
            pivotEulers = pivot.rotation.eulerAngles;

            pivotTargetRot = pivot.transform.localRotation;
            rigTargetRot = transform.localRotation;
        }

        protected void FixedUpdate()
        {
            if (rig == null) return;
            // Move the rig towards target position.
            rig.position = target.position;
            HandleRotationMovement();
            if (settings.lockCursor && Input.GetMouseButtonUp(0))
            {
                Cursor.lockState = settings.lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !settings.lockCursor;
            }
        }
        
        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void HandleRotationMovement()
        {
            if (Time.timeScale < float.Epsilon)
                return;

            // Read the user input
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");

            // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
            lookAngle += x * settings.sensitivity;

            // Rotate the rig (the root object) around Y axis only:
            rigTargetRot = Quaternion.Euler(0f, lookAngle, 0f);

            // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            tiltAngle -= y * settings.sensitivity;
            // and make sure the new value is within the tilt range
            tiltAngle = Mathf.Clamp(tiltAngle, -settings.tiltMin, settings.tiltMax);

            // Tilt input around X is applied to the pivot (the child of this object)
            pivotTargetRot = Quaternion.Euler(tiltAngle, pivotEulers.y, pivotEulers.z);

            if (settings.turnSmoothing > 0)
            {
                pivot.localRotation = Quaternion.Slerp(pivot.localRotation, pivotTargetRot, settings.turnSmoothing * Time.deltaTime);
                rig.localRotation = Quaternion.Slerp(transform.localRotation, rigTargetRot, settings.turnSmoothing * Time.deltaTime);
            }
            else
            {
                pivot.localRotation = pivotTargetRot;
                rig.localRotation = rigTargetRot;
            }
        }
    }
}