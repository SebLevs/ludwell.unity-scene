using System;
using UnityEngine;

namespace Ludwell.Scene
{
    public class DropdownData
    {
        public DropdownVisualElement VisualElement { get; set; }
        public string Name { get; set; } = "";
        public Action Action { get; set; } = () => { Debug.LogError("Action was clicked"); };
    }
}