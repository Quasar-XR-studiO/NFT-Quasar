using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZBoom.Common.SpatialMap
{
    public class PropCollection : MonoBehaviour
    {
        public static PropCollection Instance;
        public List<TempletGroup> TempletGroups = new List<TempletGroup>();
        [HideInInspector] public List<Templet> Templets = new List<Templet>();

        private void Awake()
        {
            Instance = this;
            Templets.Clear();
            foreach (var group in PropCollection.Instance.TempletGroups)
            {
                foreach (var templet in group.Templets)
                {
                    Templets.Add(templet);
                }
            }
        }

        [Serializable]
        public class Templet
        {
            public GameObject Object;
            public Sprite Icon;
            public string Id;
            public string Name;
        }

        [Serializable]
        public class TempletGroup {
            public string Name;
            public List<Templet> Templets = new List<Templet>();
        }
    }
}
