using UnityEditor;
using UnityEditor.PackageManager;

namespace Ludwell.SceneManagerToolkit.Editor
{
    [InitializeOnLoad]
    public class AddressablesPackageListener
    {
        private const string AddressablesPackageName = "com.unity.addressables";

        static AddressablesPackageListener()
        {
            Events.registeredPackages -= OnPackagesRegistered;
            Events.registeredPackages += OnPackagesRegistered;
        }

        private static void OnPackagesRegistered(PackageRegistrationEventArgs args)
        {
            foreach (var package in args.added)
            {
                if (package.name != AddressablesPackageName) continue;
#if USE_ADDRESSABLES_EDITOR
                AddressablesProcessor.SolveBindersAddressableID();
                SceneAssetReferenceDrawer.RepaintInspectorWindows();
#endif
                return;
            }

            foreach (var package in args.removed)
            {
                if (package.name != AddressablesPackageName) continue;
                ResourcesLocator.GetSceneAssetDataBinders().ResetAddresses();
                SceneAssetReferenceDrawer.RepaintInspectorWindows();
                return;
            }
        }
    }
}
