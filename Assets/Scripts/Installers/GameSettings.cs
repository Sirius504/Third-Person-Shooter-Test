using Test.Model;
using UnityEngine;
using Zenject;

namespace Test.Installers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Installers/GameSettings")]
    public class GameSettings : ScriptableObjectInstaller<GameSettings>
    {
        public Movement.Settings movementSettings;


        public override void InstallBindings()
        {
            Container.BindInstance(movementSettings);
        }
    }
}