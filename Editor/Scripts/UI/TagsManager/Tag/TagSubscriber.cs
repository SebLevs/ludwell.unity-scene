using System;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public class TagSubscriber : IListable
    {
        [SerializeField] private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
            }
        }
    }
}