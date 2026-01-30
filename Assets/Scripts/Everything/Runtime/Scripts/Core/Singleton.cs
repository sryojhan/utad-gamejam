using System;
using System.Runtime.Serialization;
using UnityEngine;


//TODO: muchas de estas cosas deberian hacerse protected en lugar de publicas...

[DefaultExecutionOrder(-1000)]
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    protected virtual bool ConserveBetweenScenes => false;
    protected virtual bool AutoInitialise => false;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();

                if (_instance == null)
                {
                    T prototype = (T)FormatterServices.GetUninitializedObject(typeof(T));

                    if (prototype.AutoInitialise)
                    {
                        GameObject newGO = new ($"{typeof(T)} - Singleton");
                        _instance = newGO.AddComponent<T>();
                    }
                    else
                    {
                        throw new UnityException("Could not locate singleton of type " + typeof(T).ToString());
                    }
                }

                //Check if the singleton needs to persists between scenes
                if(_instance.ConserveBetweenScenes)
                {
                    _instance.transform.SetParent(null);
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// This method is a safe way to check if a singleton is initialized without initialising it
    /// </summary>
    /// <returns>Returns true if the singleton has been initialised</returns>
    public static bool IsInitialised()
    {
        return _instance != null;
    }

    public static T EnsureInitialised()
    {
        return instance;
    }

    public static bool ImTheOne(T me)
    {
        return _instance == me;
    }

    public static bool DestroyIfInitialised(T me)
    {
        if(_instance != null && _instance != me)
        {
            Destroy(me.gameObject);

            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (ImTheOne(this as T)) _instance = null;
    }

}
