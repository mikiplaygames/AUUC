using System;
using UnityEngine;

namespace MikiHeadDev.Core
{
    public class ObjectId : MonoBehaviour , IIdentify
    {
        [SerializeField][Tooltip("Set to -2 to force assign")] private int id = 0;
        public int Id => id;
        public static bool ShouldAssignId(GameObject go) => (go.scene.IsValid() && go.scene.name != go.name && go.GetComponent<IIdentify>().Id == 0);
        public static int GenerateID() => Guid.NewGuid().GetHashCode();
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if ((gameObject.scene.IsValid() && gameObject.scene.name != name && id == 0) || id == -2)
                id = GenerateID();
            AdditionalOnValidate();
        }
        #endif
        protected virtual void AdditionalOnValidate() { }
    }
}