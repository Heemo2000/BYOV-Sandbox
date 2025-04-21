using UnityEngine;

namespace Game.InversionOfControlManagement
{
    [AddComponentMenu("ServiceLocator/ ServiceLocator Scene")]
    public class ServiceLocatorSceneBootstrapper : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureForScene();
        }
    }
}
