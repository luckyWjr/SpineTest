using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager<T> : IManger where T : IManger, new()
{
    static T mInstance;

    public static T Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new T();
            return mInstance;
        }
    }

    public virtual void Start()
    {
        
    }

    public virtual void Update(float deltaTime)
    {
        
    }

    public virtual void Destroy()
    {
    }
}
