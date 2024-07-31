using System;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneAssetData
    {
        public int BuildIndex = -1;
        public string Name;
        public string Path;
        public string AddressableID; // todo: ???
    }
}