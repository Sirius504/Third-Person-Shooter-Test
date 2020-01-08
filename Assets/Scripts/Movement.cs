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
        
        [Inject]
        public void Construct(Rigidbody rigidbody, Settings settings)
        {
            this.settings = settings;
            this.rigidbody = rigidbody;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 delta = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            rigidbody.MovePosition(rigidbody.position + delta * settings.speed);
        }

        [Serializable]
        public class Settings
        {
            public float speed = 0.2f;
        }
    } 
}
