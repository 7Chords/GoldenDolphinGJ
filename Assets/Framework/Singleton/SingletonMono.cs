using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GJFramework
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _g_instance;

        public static T instance
        {
            get { return _g_instance; }
            set { _g_instance = value; }
        }

        protected virtual void Awake()
        {
            _g_instance = this as T;
        }
    }
}