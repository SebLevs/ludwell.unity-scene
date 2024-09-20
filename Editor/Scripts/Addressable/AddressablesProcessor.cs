#if USE_ADDRESSABLES_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Ludwell.Architecture;
using Ludwell.EditorUtilities;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ludwell.SceneManagerToolkit.Editor
{
    [InitializeOnLoad]
    internal class AddressablesProcessor
    {
        private static readonly DelayedEditorUpdateAction DelayedRefreshViewDispatch;

        static AddressablesProcessor()
        {
            DelayedRefreshViewDispatch = new DelayedEditorUpdateAction(0, Signals.Dispatch<UISignals.RefreshView>);

            EnsureDefaultAddressablesSettingsExistence();
            AddressableAssetSettingsDefaultObject.Settings.OnModification -= SubscribeToAddressableChange;
            AddressableAssetSettingsDefaultObject.Settings.OnModification += SubscribeToAddressableChange;
        }

        private static AddressableAssetSettings EnsureDefaultAddressablesSettingsExistence()
        {
            if (AddressableAssetSettingsDefaultObject.Settings)
                return AddressableAssetSettingsDefaultObject.Settings;

            var settings = AddressableAssetSettings.Create(
                AddressableAssetSettingsDefaultObject.kDefaultConfigFolder,
                AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName,
                true,
                true
            );

            AddressableAssetSettingsDefaultObject.Settings = settings;

            AssetDatabase.SaveAssets();

            Debug.Log("Ludwell Studio | Scene Manager Toolkit | " +
                      "Missing required addressables asset settings were created.");

            return AddressableAssetSettingsDefaultObject.Settings;
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

            DelayedRefreshViewDispatch.StartOrRefresh();
            SceneAssetReferenceDrawer.RepaintInspectorWindows();

            return true;
        }

        private static bool TrySolveAsSingular(AddressableAssetSettings.ModificationEvent modificationEvent,
            object parameter)
        {
            if (parameter is not AddressableAssetEntry entry || !entry.AssetPath.Contains(".unity")) return false;


            var sceneAssetDataBinders = ResourcesLocator.GetSceneAssetDataBinders();

            var guid = entry.guid;

            var binder = sceneAssetDataBinders.GetBinderFromId(guid);

            if (binder == null) return false;

            UpdateBinder(modificationEvent, binder, entry);
            DelayedRefreshViewDispatch.StartOrRefresh();
            SceneAssetReferenceDrawer.RepaintInspectorWindows();

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
                var address = GetAddressableIDForGUID(binder.Data.GUID);
                binder.Data.AddressableID = address;
            }
        }

        public static string GetAddressableIDForObject(Object obj)
        {
            var settings = EnsureDefaultAddressablesSettingsExistence();
            if (settings == null) return SceneAssetDataBinders.NotAddressableName;

            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
            var entry = settings.FindAssetEntry(guid);

            return entry == null ? SceneAssetDataBinders.NotAddressableName : entry.address;
        }

        public static string GetAddressableIDForGUID(string guid)
        {
            var settings = EnsureDefaultAddressablesSettingsExistence();
            if (settings == null) return SceneAssetDataBinders.NotAddressableName;

            var entry = settings.FindAssetEntry(guid);

            return entry == null ? SceneAssetDataBinders.NotAddressableName : entry.address;
        }

        public static void AddToAddressables(string guid)
        {
            var settings = EnsureDefaultAddressablesSettingsExistence();
            var defaultGroup = settings.DefaultGroup;
            settings.CreateOrMoveEntry(guid, defaultGroup);
        }

        public static void RemoveFromAddressables(string address)
        {
            var settings = EnsureDefaultAddressablesSettingsExistence();

            for (var groupIndex = settings.groups.Count - 1; groupIndex >= 0; groupIndex--)
            {
                var group = settings.groups[groupIndex];
                for (var entryIndex = group.entries.Count - 1; entryIndex >= 0; entryIndex--)
                {
                    var entry = group.entries.ToList()[entryIndex];
                    if (!string.Equals(entry.address, address, StringComparison.InvariantCulture)) continue;
                    group.RemoveAssetEntry(entry);
                    return;
                }
            }
        }
    }
}
#endif
