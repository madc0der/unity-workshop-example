using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.common
{
    public static class HierarchyHelper
    {
        public static void ClearChildren(GameObject root, params GameObject[] except)
        {
            var toDelete = new List<GameObject>(10);
            for (var i = 0; i < root.transform.childCount; i++)
            {
                var childGO = root.transform.GetChild(i).gameObject;
                if (!except.Contains(childGO))
                {
                    toDelete.Add(childGO);
                }
            }
            toDelete.ForEach(Object.DestroyImmediate);
        }
    }
}