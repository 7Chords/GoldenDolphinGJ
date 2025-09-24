using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GJFramework
{
    // 单例基类
    public class Singleton<T> where T : new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }
}


