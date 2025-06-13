using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace MikiHeadDev.Helpers
{
    public static class AdvancedMethods
    {
        public static readonly Vector3 SideMirror = new(-1,1,1);
        public static Color GetAvgColorFromSprite(Sprite sprite)
        {
            Color supposedColor = Color.white;
            try
            {
                Color[] colors = sprite.texture.GetPixels();
                for (int i = 0; i < colors.Length; i++)
                {
                    supposedColor += colors[i];
                }

                supposedColor /= colors.Length;
                supposedColor.a = 1;
            }catch{supposedColor = Color.gray;}

            return supposedColor;
        }
        public static float GetAngle(Vector2 me, Vector2 target) => Mathf.Atan2(target.y - me.y, target.x - me.x) * Mathf.Rad2Deg;
        public static T GetComponentInObject<T>(GameObject gameObject)
        {
            return gameObject.GetComponentInChildren<T>(true) ?? gameObject.GetComponentInParent<T>(true);
        }
        public static T GetComponentInObject<T>(Component component)
        {
            return component.GetComponentInChildren<T>(true) ?? component.GetComponentInParent<T>(true);
        }
        public static bool TryGetComponentInObject<T>(GameObject gameObject, out T component)
        {
            component = GetComponentInObject<T>(gameObject);
            return component != null;
        }
        public static bool TryGetComponentInObject<T>(Component component, out T componentOut)
        {
            componentOut = GetComponentInObject<T>(component);
            return componentOut != null;
        }
        public static void ShuffleBySeed<T>(IList<T> list, int seed)
        {
            var rng = new Random(seed);
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void SetListSize<T>(IList<T> list, int newLimit)
        {
            while (list.Count != newLimit)
            {
                if (list.Count > newLimit)
                    list.RemoveAt(list.Count - 1);
                else
                    list.Add(default);
            }
        }
        
        public static Vector3 GetPositionOnCircle(float angle, float radius)
        {
            return new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }
    }
}