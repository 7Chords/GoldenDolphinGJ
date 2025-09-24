using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GJFramework
{
    public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        private static T _instance;

        // 线程安全的实例访问器
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 尝试在场景中查找已存在的实例
                    _instance = FindObjectOfType<T>();

                    // 如果场景中没有实例，则创建一个新的
                    if (_instance == null)
                    {
                        GameObject singletonObj = new GameObject(typeof(T).Name + "_Singleton");
                        _instance = singletonObj.AddComponent<T>();

                        DontDestroyOnLoad(singletonObj);
                    }
                }

                return _instance;
            }
        }

        // 确保实例唯一 怕的就是自己在Scene放了一个或者多个挂这个继承类的脚本的Go 有多的就直接销毁
        // 否者就把自己赋值给_instance
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;

                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                Debug.LogWarning($"已存在{typeof(T)}的单例实例，当前实例已销毁");
            }
        }
    }
}
