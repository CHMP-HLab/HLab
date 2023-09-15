#nullable enable
using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLab.Base;

public class SortedQueue<T>(Func<T, T, int> comparator)
{
    class Node        
    {
        public Node? Next;
        public T? Value;
    }

    Node? _head = null;

    public Func<T, T, int> Comparator { get; } = comparator;

    public void Enqueue(T item)
    {
        ref var node = ref _head;
        while(node is { } && Comparator(item,node.Value) > 0)
        {
            node = ref node.Next;
        }
        node = new Node { Value = item, Next = node };
    }

    public bool TryDequeue(out T? item)
    {
        var node = _head;
        if (node == null) {
            item = default;
            return false;
        }
        _head = node.Next;
        item = node.Value;
        return true;
    }
}