using System.Collections.Generic;
using UnityEngine;

public class CrossReferencedMap<T>
{
    readonly Dictionary<T, T> pairs;

    readonly HashSet<T> all;

    public CrossReferencedMap()
    {
        pairs = new();

        all = new();
    }


    public bool Contains(T elem)
    {
        return all.Contains(elem);
    }

    public void Add(T first, T second)
    {
        if (Contains(first) || Contains(second))
            throw new UnityException("repetition in map");

        all.Add(first);
        all.Add(second);

        pairs.Add(first, second);
        pairs.Add(second, first);
    }


    public T GetPair(T elem)
    {
        if (Contains(elem))
            throw new UnityException("doesnt exist");

        if (pairs.ContainsKey(elem))
            return pairs[elem];

        throw new UnityException("doesnt exist");
    }

}
