using UnityEngine;
using System.Collections.Generic;

namespace GJFramework
{
    // 对象池基类 GameObj的 如果是纯数据类的自己再去写一个约束是 T : New() 的类
    public abstract class GameObjectPoolBase<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly int _maxSize;

        public GameObjectPoolBase(T prefab, Transform parent, int initialSize = 5, int maxSize = 50)
        {
            _prefab = prefab;
            _parent = parent;
            _maxSize = maxSize;

            // 预创建初始对象
            for (int i = 0; i < initialSize; i++)
            {
                T obj = CreateNewObject();
                _pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// 从对象池获取一个对象
        /// </summary>
        public T Get()
        {
            T obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                obj = CreateNewObject();
            }

            obj.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 回收对象到对象池
        /// </summary>
        public void Release(T obj)
        {
            if (obj == null) return;

            obj.gameObject.SetActive(false);

            if (_pool.Count < _maxSize)
            {
                OnReleaseObj();
                _pool.Enqueue(obj);
            }
            else
            {
                Object.Destroy(obj.gameObject);
            }
        }
        /// <summary>
        /// 清空对象池
        /// </summary>
        public void Clear()
        {
            foreach (var obj in _pool)
            {
                Object.Destroy(obj.gameObject);
            }
            _pool.Clear();
        }
        private T CreateNewObject()
        {
            T newObj = Object.Instantiate(_prefab, _parent);
            newObj.gameObject.SetActive(false);
            return newObj;
        }

        // 子类重写取出时候的初始化
        protected abstract void OnGetObj();

        // 子类重写放回后的初始化
        protected abstract void OnReleaseObj();

    }

}
