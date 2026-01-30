using System.Collections.Generic;
using UnityEngine;


/*
    TODO: de esto hacer una clase base generica
 */

public abstract class Bank : ScriptableObject
{
    public abstract void Init();
    public abstract void Unload();
}

public class Bank<Content> : Bank where Content : Object
{
    [System.Serializable]
    public struct Entry
    {
        public string name;
        public Content content;
    }

    [SerializeField]
    protected Entry[] entries;

    private Map<Content> lookUpTable;

    public override void Init()
    {
        if (lookUpTable != null) return;

        lookUpTable = new();

        foreach (Entry entry in entries)
        {
            lookUpTable.Add(entry.name, entry.content);
        }
    }

    public override void Unload()
    {
        lookUpTable?.Clear();
        lookUpTable = null;
    }

    public Content Get(string key)
    {
        Init();

        if (lookUpTable.TryGetValue(key, out Content content))
        {
            return content;
        }

        Debug.LogError($"Tried to access element '{key}', that doesnt exist in the bank. ");

        return null;
    }
}

