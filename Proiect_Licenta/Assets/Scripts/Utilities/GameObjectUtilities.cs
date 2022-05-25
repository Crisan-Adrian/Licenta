using UnityEngine;

namespace Utilities
{
    public class GameObjectUtilities
    {
        public static Transform FindChildTransformWithName(GameObject parent, string name)
        {
            Transform foundChild = parent.transform.Find(name);
            if (foundChild != null)
            {
                return foundChild;
            }

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                foundChild = FindChildTransformWithName(child, name);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            return null;
        }

        public static GameObject FindChildWithName(GameObject parent, string name)
        {
            Transform childTransform = FindChildTransformWithName(parent, name);
            if (childTransform != null)
            {
                return childTransform.gameObject;
            }

            return null;
        }
    }
}