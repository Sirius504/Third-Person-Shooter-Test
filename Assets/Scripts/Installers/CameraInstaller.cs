using UnityEngine;
using UnityStandardAssets.Cameras;
using Zenject;

namespace Test.Installers
{
    public class CameraInstaller : MonoInstaller
    {
        [SerializeField] Transform target;

        public override void InstallBindings()
        {
            Container
                .Bind<Camera>()
                .FromComponentInChildren()
                .AsSingle();

            Container
                .Bind<Transform>()
                .FromInstance(target)
                .WhenInjectedInto<FreeLookCam>();
        }
    } 
}
