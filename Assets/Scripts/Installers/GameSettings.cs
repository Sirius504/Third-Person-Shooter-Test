using Test.Model;
using UnityEngine;
using UnityStandardAssets.Cameras;
using Zenject;

namespace Test.Installers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Installers/GameSettings")]
    public class GameSettings : ScriptableObjectInstaller<GameSettings>
    {
        public Movement.Settings playerMovementSettings;
        public FreeLookCam.Settings playerCameraSettings;
        public ProtectCameraFromWallClip.Settings wallClipSettings;


        public override void InstallBindings()
        {
            Container.BindInstance(playerMovementSettings).AsSingle();
            Container.BindInstance(playerCameraSettings).AsSingle();
            Container.BindInstance(wallClipSettings).AsSingle();
        }
    }
}