using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ludwell.Scene.Editor
{
    [Serializable]
    public class JoinTable : ScriptableObject
    {
        [Serializable]
        private class Binding
        {
            public string tagId;
            public List<string> quickLoadElementIds = new();
        }

        [SerializeField] private List<Binding> bindings = new();

        public void Bind(string tagId, string subscriberId)
        {
            var binding = bindings.FirstOrDefault(b => b.tagId == tagId);
            if (binding == null)
            {
                binding = new Binding { tagId = tagId };
                bindings.Add(binding);
            }
            if (!binding.quickLoadElementIds.Contains(subscriberId))
            {
                binding.quickLoadElementIds.Add(subscriberId);
            }
        }

        public void Unbind(string tagId, string subscriberId)
        {
            var binding = bindings.FirstOrDefault(b => b.tagId == tagId);
            if (binding != null)
            {
                binding.quickLoadElementIds.Remove(subscriberId);
                if (binding.quickLoadElementIds.Count == 0)
                {
                    bindings.Remove(binding);
                }
            }
        }

        public List<string> GetSubscriberIds(string tagId)
        {
            var binding = bindings.FirstOrDefault(b => b.tagId == tagId);
            return binding?.quickLoadElementIds ?? new List<string>();
        }

        public List<string> GetTagIds(string subscriberId)
        {
            return bindings
                .Where(b => b.quickLoadElementIds.Contains(subscriberId))
                .Select(b => b.tagId)
                .ToList();
        }
    }
}
