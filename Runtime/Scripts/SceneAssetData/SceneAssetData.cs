using System;
using UnityEngine.SceneManagement;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneAssetData
    {
        public int BuildIndex => SceneUtility.GetBuildIndexByScenePath(Path);
        public string Name;
        public string Path;
        public string AddressableID; // todo: ???
    }
}