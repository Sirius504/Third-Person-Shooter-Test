using Test.Model;
using Zenject;

namespace Test.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Movement>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ThirdPersonCamera>().FromComponentInHierarchy().AsSingle();
        }
    } 
}
