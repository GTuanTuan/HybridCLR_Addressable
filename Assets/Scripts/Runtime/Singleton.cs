using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Singleton<T> where T : class, new()
{
    protected Singleton() { }
    ~Singleton() { singleton = null; }
    private static T singleton;
    private static readonly object locker = new object();
    public static T Inst
    {
        get
        {
            if (singleton == null)
            {
                lock (locker)
                {
                    if (singleton == null)
                        singleton = new T();
                    Debug.Log($"Create {typeof(T)}");
                }
            }
            return singleton;
        }

    }
}

