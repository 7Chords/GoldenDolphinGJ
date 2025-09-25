using UnityEngine;

namespace GJFramework
{
    public class UIGameObjPool<T> : GameObjectPoolBase<T> where T : MonoBehaviour
    {
        public UIGameObjPool(T prefab, Transform parent, int initialSize = 5, int maxSize = 50) : base(prefab, parent, initialSize, maxSize)
        {
        }

        protected override void OnGetObj()
        {
           
        }

        protected override void OnReleaseObj()
        {
            
        }
    }

}
