using System.Collections;
using System.Runtime.CompilerServices;

namespace HLab.Base;

public class HashCode
{
    int _hash;
    public int Value => _hash;

    HashCode(int hash)
    {
        _hash = hash;

    }

    public static HashCode Start => new HashCode(17);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashCode Add(object obj)
    {
        if (obj is IEnumerable e) return Add(e);
        if (obj?.GetType().IsArray??false) return Add((object[]) obj);

        _hash = unchecked((_hash * 31) + (obj?.GetHashCode()??0));
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashCode Add(object[] objs)
    {
        unchecked
        {
            foreach (var obj in objs)
            {
                Add(obj);
            }
            return this;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashCode Add(IEnumerable objs)
    {
        unchecked
        {
            foreach (var obj in objs)
            {
                Add(obj);
            }
            return this;
        }
    }

}