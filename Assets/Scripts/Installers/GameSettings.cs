using Test.Model;
using UnityEngine;
using Zenject;

namespace Test.Installers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Installers/GameSettings")]
    public class GameSettings : ScriptableObjectInstaller<GameSettings>
    {
        public Movement.Settings playerMovementSettings;
        public ThirdPersonCamera.Settings playerCameraSettings;


        public override void InstallBindings()
        {
            Container.BindInstance(playerMovementSettings);
            Container.BindInstance(playerCameraSettings);
        }
    }
}