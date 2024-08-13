using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Ludwell.Scene.Editor
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
#endif
                return;
            }

            foreach (var package in args.removed)
            {
                if (package.name != AddressablesPackageName) continue;
                ResourcesLocator.GetSceneAssetDataBinders().ResetAddresses();
                return;
            }
        }

        private static void OnInstallAddressablesPackage()
        {
            Debug.LogError("todo: set any data as scriptable is their respective scene asset is an addressable");
#if USE_ADDRESSABLES_EDITOR
            AddressablesProcessor.SolveBindersAddressableID();
#endif

        }

        private static void OnUninstallAddressablesPackage()
        {
            ResourcesLocator.GetSceneAssetDataBinders().ResetAddresses();
        }
    }
}