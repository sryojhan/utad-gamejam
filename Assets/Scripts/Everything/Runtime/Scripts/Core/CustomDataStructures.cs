using System;
using System.Collections.Generic;


public class Map<TValue> : Dictionary<string, TValue> {

    public Map() : base() { }
    public Map(int capacity) : base(capacity) { }
    public Map(bool ignoreCase) : base(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal) { }
}
