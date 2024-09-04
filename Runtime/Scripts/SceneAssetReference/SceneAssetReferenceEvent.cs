using UnityEngine;
using UnityEngine.Events;

namespace Ludwell.SceneManagerToolkit
{
    /// <summary>
    /// Use this script instead of a SceneAsset to bypass the serialization limitation of SceneAsset.<br/><br/>
    /// Add logic to the UnityEvent that uses the <see cref="SceneAssetReference"/> (<see cref="_reference"/>) as parameter.<br/><br/>
    /// Refer to the script in a serialized field and <see cref="Raise"/> the event to invoke a behaviour that uses the <see cref="_reference"/>.
    /// </summary>
    public class SceneAssetReferenceEvent : MonoBehaviour
    {
        [SerializeField] private SceneAssetReference _reference;

        [SerializeField] private UnityEvent<SceneAssetReference> _event;

        public void Raise()
        {
            _event?.Invoke(_reference);
        }

        public void AddListener(UnityAction<SceneAssetReference> listener)
        {
            _event.AddListener(listener);
        }

        public void RemoveListener(UnityAction<SceneAssetReference> listener)
        {
            _event.RemoveListener(listener);
        }
    }
}
