using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class EnumArray<TEnum, TValue> : IEnumerable<TValue> where TEnum : Enum
{
    [SerializeField]
    public TValue[] internalArray; 

    public TValue this[TEnum index]
    {
        get
        {
            int i = ConvertToInt(index);
            if (internalArray == null || i < 0 || i >= internalArray.Length) return default;
            return internalArray[i];
        }
        set
        {
            int i = ConvertToInt(index);
            if (internalArray != null && i >= 0 && i < internalArray.Length) internalArray[i] = value;
        }
    }

    public int Length => internalArray != null ? internalArray.Length : 0;

    public IEnumerator<TValue> GetEnumerator() => ((IEnumerable<TValue>)internalArray).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => internalArray.GetEnumerator();

    private static int ConvertToInt(TEnum e) => System.Runtime.CompilerServices.Unsafe.As<TEnum, int>(ref e);
}