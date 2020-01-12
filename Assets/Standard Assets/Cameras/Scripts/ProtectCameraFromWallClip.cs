using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace UnityStandardAssets.Cameras
{
    public class ProtectCameraFromWallClip : MonoBehaviour
    {
        Settings settings;

        private Transform pivot;                // the point at which the camera pivots around
        private Transform camera;               // the transform of the camera
        private float originalDist;             // the original distance to the camera before any modification are made
        private float noveVelocity;             // the velocity at which the camera moved
        private float currentDist;              // the current distance from the camera to the target
        private Ray ray = new Ray();            // the ray used in the lateupdate for casting between the camera and the target
        private RaycastHit[] hits;              // the hits between the camera and the target
        private RayHitComparer rayHitComparer;  // variable to compare raycast hit distances

        public bool Protecting { get; private set; }    // used for determining if there is an object between the target and the camera

        [Inject]
        private void Construct(Camera camera, Settings settings)
        {
            this.settings = settings;
            this.camera = camera.transform;
            pivot = this.camera.parent;
            originalDist = this.camera.localPosition.magnitude;
            currentDist = originalDist;

            // create a new RayHitComparer
            rayHitComparer = new RayHitComparer();
        }


        private void LateUpdate()
        {
            // initially set the target distance
            float targetDist = originalDist;

            ray.origin = pivot.position + pivot.forward * settings.sphereCastRadius;
            ray.direction = -pivot.forward;

            // initial check to see if start of spherecast intersects anything
            var cols = Physics.OverlapSphere(ray.origin, settings.sphereCastRadius);

            bool initialIntersect = false;
            bool hitSomething = false;

            // loop through all the collisions to check if something we care about
            for (int i = 0; i < cols.Length; i++)
            {
                if ((!cols[i].isTrigger) &&
                    !(cols[i].attachedRigidbody != null && cols[i].attachedRigidbody.CompareTag(settings.dontClipTag)))
                {
                    initialIntersect = true;
                    break;
                }
            }

            // if there is a collision
            if (initialIntersect)
            {
                ray.origin += pivot.forward * settings.sphereCastRadius;

                // do a raycast and gather all the intersections
                hits = Physics.RaycastAll(ray, originalDist - settings.sphereCastRadius);
            }
            else
            {
                // if there was no collision do a sphere cast to see if there were any other collisions
                hits = Physics.SphereCastAll(ray, settings.sphereCastRadius, originalDist + settings.sphereCastRadius);
            }

            // sort the collisions by distance
            Array.Sort(hits, rayHitComparer);

            // set the variable used for storing the closest to be as far as possible
            float nearest = Mathf.Infinity;

            // loop through all the collisions
            for (int i = 0; i < hits.Length; i++)
            {
                // only deal with the collision if it was closer than the previous one, not a trigger, and not attached to a rigidbody tagged with the dontClipTag
                if (hits[i].distance < nearest && (!hits[i].collider.isTrigger) &&
                    !(hits[i].collider.attachedRigidbody != null &&
                      hits[i].collider.attachedRigidbody.CompareTag(settings.dontClipTag)))
                {
                    // change the nearest collision to latest
                    nearest = hits[i].distance;
                    targetDist = -pivot.InverseTransformPoint(hits[i].point).z;
                    hitSomething = true;
                }
            }

            // hit something so move the camera to a better position
            Protecting = hitSomething;
            float lerpFactor = currentDist > targetDist
                ? settings.clipMoveTime
                : settings.returnTime;
            currentDist = Mathf.Lerp(currentDist, targetDist, lerpFactor);
            currentDist = Mathf.Clamp(currentDist, settings.closestDistance, originalDist);
            camera.localPosition = -Vector3.forward * currentDist;
        }


        // comparer for check distances in ray cast hits
        public class RayHitComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
            }
        }

        [Serializable]
        public class Settings
        {
            public float clipMoveTime = 0.05f;              // time taken to move when avoiding cliping (low value = fast, which it should be)
            public float returnTime = 0.2f;                 // time taken to move back towards desired position, when not clipping (typically should be a higher value than clipMoveTime)
            public float sphereCastRadius = 0.3f;           // the radius of the sphere used to test for object between camera and target
            public float closestDistance = 1.5f;            // the closest distance the camera can be from the target
            public string dontClipTag = "Player";           // don't clip against objects with this tag (useful for not clipping against the targeted object
        }
    }
}
