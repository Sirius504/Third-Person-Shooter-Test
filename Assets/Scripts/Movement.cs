using System;
using UnityEngine;
using Zenject;

namespace Test.Model
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        private Rigidbody rigidbody;
        private Settings settings;
        private Camera camera;        

        [Inject]
        public void Construct(Rigidbody rigidbody, Camera camera, Settings settings)
        {
            this.settings = settings;
            this.rigidbody = rigidbody;
            this.camera = camera;
        }

        void FixedUpdate()
        {
            Vector3 delta = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            RotateForward();
            rigidbody.MovePosition(Vector3.Lerp(rigidbody.position, rigidbody.position + transform.TransformVector(delta), Time.fixedDeltaTime * settings.speed));
        }

        private void RotateForward()
        {
            Vector3 forward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);
            Quaternion newRotation = Quaternion.LookRotation(forward, Vector3.up);
            rigidbody.MoveRotation(newRotation);    
        }

        [Serializable]
        public class Settings
        {
            public float speed = 0.2f;
            public float rotationSpeed = 0.2f;
        }
    } 
}
