using System.IO;

namespace Ludwell.Scene.Editor
{
    public static class SpritesPath
    {
        private static string SpritesName = "Sprites";
        public static readonly string AddBuildSettings = Path.Combine(SpritesName, "icon_add-build-settings");
        public static readonly string RemoveBuildSettings = Path.Combine(SpritesName, "icon_remove-build-settings");
        public static readonly string OpenAdditive = Path.Combine(SpritesName, "icon_open-additive");
        public static readonly string RemoveAdditive = Path.Combine(SpritesName, "icon_remove-additive");
        public static readonly string Load = Path.Combine(SpritesName, "icon_load");
        public static readonly string Stop = Path.Combine(SpritesName, "icon_stop");
        public static readonly string AddToAddressables = Path.Combine(SpritesName, "icon_add-addressables");
        public static readonly string RemoveFromAddressables = Path.Combine(SpritesName, "icon_remove-addressables");
    }
}