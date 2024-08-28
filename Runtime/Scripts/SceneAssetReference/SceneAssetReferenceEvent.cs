using UnityEngine;
using UnityEngine.Events;

namespace Ludwell.SceneManagerToolkit
{
    public class SceneAssetReferenceEvent : MonoBehaviour
    {
        [SerializeField] private SceneAssetReference _reference;

        [SerializeField] private UnityEvent<SceneAssetReference> _event;

        public void Raise()
        {
            _event?.Invoke(_reference);
        }
    }
}
