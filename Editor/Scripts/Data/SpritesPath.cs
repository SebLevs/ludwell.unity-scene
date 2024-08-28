using System.IO;

namespace Ludwell.SceneManagerToolkit.Editor
{
    internal static class SpritesPath
    {
        private static readonly string SpritesName = "Sprites";
        
        public static readonly string OpenAdditive = Path.Combine(SpritesName, "icon_open-additive");
        public static readonly string RemoveAdditive = Path.Combine(SpritesName, "icon_remove-additive");
        public static readonly string Load = Path.Combine(SpritesName, "icon_load");
        public static readonly string Stop = Path.Combine(SpritesName, "icon_stop");
        public static readonly string AddToBuildSettings = Path.Combine(SpritesName, "icon_add-build-settings");
        public static readonly string RemoveBuildSettings = Path.Combine(SpritesName, "icon_remove-build-settings");
        public static readonly string EnableInBuildSettings = Path.Combine(SpritesName, "icon_enable-build-settings");
        public static readonly string DisableInBuildSettings = Path.Combine(SpritesName, "icon_disable-build-settings");
        public static readonly string AddToAddressables = Path.Combine(SpritesName, "icon_add-addressables");
        public static readonly string RemoveFromAddressables = Path.Combine(SpritesName, "icon_remove-addressables");
        public static readonly string Settings = Path.Combine(SpritesName, "icon_settings");
        
        public static readonly string SettingsDark = Path.Combine(SpritesName, "icon_settings-dark");
        public static readonly string AddToBuildSettingsDark = Path.Combine(SpritesName, "icon_add-build-settings-dark");
        public static readonly string EnableInBuildSettingsDark = Path.Combine(SpritesName, "icon_enable-build-settings-dark");
        public static readonly string AddToAddressablesDark = Path.Combine(SpritesName, "icon_add-addressables-dark");
        public static readonly string RemoveFromAddressablesDark = Path.Combine(SpritesName, "icon_remove-addressables-dark");
    }
}
