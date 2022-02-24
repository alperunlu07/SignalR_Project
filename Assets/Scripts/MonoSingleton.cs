using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static volatile T instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = FindObjectOfType((typeof(T))) as T;
    }
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType((typeof(T))) as T;
            }

            return instance;
        }
    }
   
}