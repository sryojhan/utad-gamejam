using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Pool;


public class Pool<T> where T : Component
{
    readonly Queue<T> avaliable;

    public Transform Parent { get; set; } = null;
    public GameObject Prototype { get; set; } = null;

    public bool AllowInstantiateWithoutPrototype { get; set; } = false;
    public bool AllowDynamicGrowth { get; set; } = true;

    public Action<T> OnInstantiate { get; set; }
    public Action<T> OnSpawn { get; set; }
    public Action<T> OnDespawn { get; set; }

    private Pool()
    {
        avaliable = new();
    }

    public static Pool<T> Create(Transform parent)
    {
        return new Pool<T>()
        {
            Parent = parent
        };
    }

    public static Pool<T> Create(Transform parent, GameObject prototype)
    {
        return new Pool<T>()
        {
            Parent = parent,
            Prototype = prototype
        };
    }

    public Pool<T> LoadChildren(Transform parent = null)
    {
        if (!parent) parent = Parent;

        //TODO: 

        return this;
    }


    public Pool<T> Prewarm(int count = 10)
    {
        for (int i = 0; i < count; i++)
        {
            CreateInstance();
        }

        return this;
    }


    private bool CreateInstance()
    {
        if(!Prototype)
        {
            if (AllowInstantiateWithoutPrototype)
            {
                GameObject go = new ($"{typeof(T)} - Pool instance");
                go.transform.SetParent(Parent);

                T elem = go.AddComponent<T>();

                OnInstantiate?.Invoke(elem);
                avaliable.Enqueue(elem);

                go.SetActive(false);

                return true;
            }

            Debug.LogError($"Pool: Tried to instantiate a new {typeof(T)} but no prototype was found and dynamic creation is disabled");
            return false;
        }

        else
        {
            GameObject go = GameObject.Instantiate(Prototype, Parent);
            go.SetActive(false);

            T elem = go.GetComponentInChildren<T>();

            if(!elem)
            {
                Debug.LogError($"Pool: The given prototype does not contain {typeof(T)} component");
                return false;
            }

            OnInstantiate?.Invoke(elem);
            avaliable.Enqueue(elem);
            return true;
        }
    }



    public T Spawn()
    {
        if(avaliable.Count == 0)
        {
            if (!AllowDynamicGrowth)
            {
                Debug.LogError($"Pool: Tried to spawn from an empty pool and dynamic growth is disabled.");
                return null;
            }

            CreateInstance();
        }


        T element = avaliable.Dequeue();
        element.gameObject.SetActive(true);

        if(element is IPoolable poolable)
        {
            poolable.OnSpawn();
        }

        OnSpawn?.Invoke(element);

        return element;
    }


    public void Despawn(T element)
    {
#if UNITY_EDITOR
        Debug.Assert(!avaliable.Contains(element), $"Pool: Trying to despawn an element already inside the pool.");
#endif
        OnDespawn?.Invoke(element);

        if (element is IPoolable poolable)
        {
            poolable.OnDespawn();
        }

        element.gameObject.SetActive(false);
        avaliable.Enqueue(element);


    }

    public bool Contains(T element)
    {
        return avaliable.Contains(element);
    }

}

public interface IPoolable
{
    public void OnSpawn();
    public void OnDespawn();
}