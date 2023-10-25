using System;
using UnityEngine;

namespace Ludwell.Scene
{
    public class DropdownData
    {
        public DropdownElement Element { get; set; }
        public string Name { get; set; } = "";
        public Action Action { get; set; } = () => { Debug.LogError("Action was clicked"); };
    }
}