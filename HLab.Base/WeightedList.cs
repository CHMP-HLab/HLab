using System;
using System.Collections;
using System.Collections.Generic;

namespace HLab.Base;

public class WeightedListConsistencyException : Exception
{
    public WeightedListConsistencyException(string message) : base(message)
    {}

}

public class WeightedList<T> : IEnumerable<T>
{
    Func<Node, Node, int> _comparator;

    public WeightedList()
    {
        _comparator = (a,b) => a.Weight - b.Weight;
    }


    public class Node
    {
        public override string ToString() => Value.ToString();

        public Node Next;
        public Node Previous;
        public T Value { get; }
        public int Weight { get; set; }


        public Node(T value, Node previous)
        {
            Value = value;
            Previous = previous;
        }

    }

    public void CheckConsistency()
    {
        if (_first == null)
        {
            if (_last == null)
                return;
            else throw new WeightedListConsistencyException("First is null but last is not");
        } 

        var node = _first;
        while (node != null)
        {
            if (node.Next == null)
            {
                if(node == _last)
                    return ;
                else
                {
                    throw new WeightedListConsistencyException("Next is null but is not Last");
                }
            }

            if (node != node.Next.Previous) throw new WeightedListConsistencyException("node.Next.Previous is not node");

            if (node.Previous == null)
            {
                if (node != _first) throw new WeightedListConsistencyException("node.Previous is null but not first");
            }
            else
            {
                if (node != node.Previous.Next) throw new WeightedListConsistencyException("node.Previous.Next is not node");
            }

            node = node.Next;
        }
    }


    Node _first;
    Node _last;

    public Node First { get => _first;}
    public Node Last { get => _last; }

    public void Add(T value)
    {
        var node = new Node(value, _last);
        if (_last != null)
        {
            _last = _last.Next = node;
            Relocate(node);
        }
        else _first = _last = node;

    }
    public int IndexOf(T value)
    {
        var node = _first;
        int i = 0;
        while (node != null)
        {
            if (Equals(value,node.Value))
            {
                return i;
            }

            node = node.Next;
            i++;
        }

        return -1;
    }
    public int IndexOf(Func<T, bool> condition)
    {
        var node = _first;
        int i = 0;
        while (node != null)
        {
            if (condition(node.Value))
            {
                return i;
            }

            node = node.Next;
            i++;
        }

        return -1;
    }

    public void ReduceWeights()
    {
        var node = _first;
        while (node != null)
        {
            node.Weight /= 2;
            node = node.Next;
        }
    }

    void Unlink(Node node)
    {
        var previous = node.Previous;

        var next = node.Next;

        if (previous == null)
            _first = next;
        else
            previous.Next = next;

        if (next == null)
            _last = previous;
        else
            next.Previous = previous;
    }


    void Link(ref Node previousNext, ref Node nextPrevious, Node node)
    {
        var next = previousNext;
        var previous = nextPrevious;

        node.Next = next;
        node.Previous = previous;

        previousNext = node;
        nextPrevious = node;
    }

    void LinkAfter(Node previous, Node node)
    {
        if (previous == null)
        {
            if(_first==null)
                Link(ref _first,ref _last, node);
            else
                Link(ref _first,ref _first.Previous, node);
        }
        else
        {
            if(previous.Next==null)
                Link(ref previous.Next, ref _last, node);
            else
                Link(ref previous.Next,ref previous.Next.Previous, node);
        }
    }

    void LinkBefore(Node next, Node node)
    {
        if (next == null)
        {
            if (_last == null)
                Link(ref _first, ref _last, node);
            else
                Link(ref _last.Next, ref _first, node);
        }
        else
        {
            if (next.Previous == null)
                Link(ref _first, ref next.Previous, node);
            else
                Link(ref next.Previous.Next, ref next.Previous, node);
        }
    }

    public WeightedList<T> AddComparator(Func<Node,Node,int> comparator)
    {
        var previous = _comparator;
        _comparator = (a, b) =>
        {
            var result = comparator(a, b);
            if (result != 0) return result;
            return previous(a, b);
        };

        return this;
    }

    void Relocate(Node node)
    {
        var newPrevious = node.Previous;

        while (newPrevious != null && _comparator(node,newPrevious)>0)
            newPrevious = newPrevious.Previous;

        if (newPrevious != node.Previous)
        {
            Unlink(node);
            LinkAfter(newPrevious,node);

        }
    }

    public T Get(Func<T, bool> condition)
    {
        var node = _first;
        while (node != null)
        {
            if (condition(node.Value))
            {
                node.Weight++;

                Relocate(node);

                if (node.Weight == int.MaxValue)
                {
                    ReduceWeights();
                }

                return node.Value;
            }

            node = node.Next;
        }

        return default(T);
    }

    public IEnumerable<T> Where(Func<T, bool> condition)
    {
        var node = _first;
        while (node != null)
        {
            if (condition(node.Value))
            {
                yield return node.Value;
            }
            node = node.Next;
        }
    }

    class Enumerator : IEnumerator<T>
    {
        readonly WeightedList<T> _list;
        Node _current;
        public Enumerator(WeightedList<T> list)
        {
            _list = list;
            _current = _list._first;
        }

        public bool MoveNext()
        {
            _current = _current.Next;
            return _current != null;
        }

        public void Reset()
        {
            _current = _list._first;
        }

        public T Current
        {
            get
            {
                if (_current==null) throw new InvalidOperationException();
                return _current.Value;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}