using System;

namespace Ludwell.Scene
{
    [Serializable]
    public class SceneAssetDataBinder : IComparable
    {
        public string Key;
        public SceneAssetData Data;

        public int CompareTo(object obj)
        {
            if (obj is SceneAssetDataBinder objectAs)
            {
                return string.Compare(Key, objectAs.Key, StringComparison.Ordinal);
            }
            
            throw new ArgumentException($"Parameter is not a {nameof(SceneAssetDataBinder)}");
        }
    }
}
