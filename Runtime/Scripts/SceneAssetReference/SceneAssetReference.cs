using System;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneAssetReference
    {
        // todo: make a dictionary to keep track of scenes? Dictionary in scriptable? Have only a hash key here and utility methods?
        public int BuildIndex = -1;
        public string Name;
        public string path;
        public string addressablePath;
    }
}
