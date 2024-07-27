using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class MoreInformationView
    {
        private readonly VisualElement _root;

        private readonly VisualElement[] _stars = new VisualElement[5];

        public MoreInformationView(VisualElement root)
        {
            _root = root.Q<VisualElement>(nameof(MoreInformationView));


            for (var index = 1; index < _stars.Length + 1; index++)
            {
                _stars[index - 1] = _root.Q<VisualElement>(index.ToString());
            }
        }
        
        public void Show()
        {
        }

        public void Hide()
        {
        }

        public void ScaleStarsUpTo(VisualElement starElement)
        {
            ScaleDownAllStars();

            foreach (var star in _stars)
            {
                Debug.LogError($"scale up star: {star}");
                if (star == starElement) break;
            }
        }

        public void ScaleDownAllStars()
        {
            foreach (var star in _stars)
            {
                Debug.LogError("scale down all stars");
            }
        }
    }
}