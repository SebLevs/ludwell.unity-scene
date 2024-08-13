#if USE_ADDRESSABLES_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using Object = UnityEngine.Object;

namespace Ludwell.Scene.Editor
{
    [InitializeOnLoad]
    public class AddressablesProcessor
    {
        static AddressablesProcessor()
        {
            AddressableAssetSettingsDefaultObject.Settings.OnModification -= SubscribeToAddressableChange;
            AddressableAssetSettingsDefaultObject.Settings.OnModification += SubscribeToAddressableChange;
        }

        private static void SubscribeToAddressableChange(AddressableAssetSettings addressableAssetSettings,
            AddressableAssetSettings.ModificationEvent modificationEvent, object parameter)
        {
            if (parameter == null) return;

            if (TrySolveAsList(modificationEvent, parameter)) return;
            TrySolveAsSingular(modificationEvent, parameter);
        }

        private static bool TrySolveAsList(AddressableAssetSettings.ModificationEvent modificationEvent,
            object parameter)
        {
            var entries = parameter as List<AddressableAssetEntry>;

            if (entries == null) return false;

            foreach (var entry in entries)
            {
                if (!entry.AssetPath.Contains(".unity")) continue;

                var sceneAssetDataBinders = ResourcesLocator.GetSceneAssetDataBinders();

                var guid = entry.guid;

                var binder = sceneAssetDataBinders.GetBinderFromId(guid);

                if (binder == null) continue;

                UpdateBinder(modificationEvent, binder, entry);
            }

            return true;
        }

        private static bool TrySolveAsSingular(AddressableAssetSettings.ModificationEvent modificationEvent,
            object parameter)
        {
            if (parameter is not AddressableAssetEntry entry || !entry.AssetPath.Contains(".unity")) return false;

            var sceneAssetDataBinders = ResourcesLocator.GetSceneAssetDataBinders();

            var guid = entry.guid;

            var binder = sceneAssetDataBinders.GetBinderFromId(guid);

            if (binder == null) return true;

            UpdateBinder(modificationEvent, binder, entry);

            return true;
        }

        private static void UpdateBinder(AddressableAssetSettings.ModificationEvent modificationEvent,
            SceneAssetDataBinder binder,
            AddressableAssetEntry entry)
        {
            binder.Data.AddressableID = modificationEvent switch
            {
                AddressableAssetSettings.ModificationEvent.EntryCreated
                    or AddressableAssetSettings.ModificationEvent.EntryAdded
                    or AddressableAssetSettings.ModificationEvent.EntryModified
                    or AddressableAssetSettings.ModificationEvent.EntryMoved => entry.address,
                AddressableAssetSettings.ModificationEvent.EntryRemoved => SceneAssetDataBinders.NotAddressableName,
                _ => throw new ArgumentOutOfRangeException(nameof(modificationEvent), modificationEvent, null)
            };
        }

        public static void SolveBindersAddressableID()
        {
            foreach (var binder in ResourcesLocator.GetSceneAssetDataBinders().Elements)
            {
                var address = GetAddressableIDForGUID(binder.GUID);
                binder.Data.AddressableID = address;
            }
        }

        public static string GetAddressableIDForObject(Object obj)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return SceneAssetDataBinders.NotAddressableName;

            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
            var entry = settings.FindAssetEntry(guid);

            return entry == null ? SceneAssetDataBinders.NotAddressableName : entry.address;
        }

        public static string GetAddressableIDForGUID(string guid)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return SceneAssetDataBinders.NotAddressableName;

            var entry = settings.FindAssetEntry(guid);

            return entry == null ? SceneAssetDataBinders.NotAddressableName : entry.address;
        }
    }
}
#endif
