using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace BrawlShooter
{
    public static class SceneExtensions
    {
        public static T FindObjectOfType<T>(this Scene scene, bool includeInactive = false) where T : class
        {
            var roots = scene.GetRootGameObjects();

            T component = default;

            for (int i = 0; i < roots.Length; ++i)
            {
                component = roots[i].GetComponentInChildren<T>(includeInactive);
                if (component != null)
                    break;
            }

            return component;
        }

        public static List<T> GetComponents<T>(this Scene scene, bool includeInactive = false) where T : class
        {
            var roots = scene.GetRootGameObjects();

            List<T> components = new List<T>();
            List<T> objectComponents = new List<T>();
            for (int i = 0; i < roots.Length; ++i)
            {
                roots[i].GetComponentsInChildren(includeInactive, objectComponents);
                components.AddRange(objectComponents);
                objectComponents.Clear();
            }

            return components;
        }
    }
}
