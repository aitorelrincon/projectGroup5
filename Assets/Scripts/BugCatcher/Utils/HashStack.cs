using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Quick HashSet + Stack.
/// </summary>
/// <typeparam name="T">Any Type</typeparam>
public class HashStack<T>
    : IEnumerable<T>
    , IReadOnlyCollection<T>
{
    const int DEFAULT_CAPACITY = 4;

    HashSet<T>  _keys;
    Stack<T>    _vals;
    public int Count { get => _keys.Count; }

    public HashStack()
    {
        _keys = new( DEFAULT_CAPACITY );
        _vals = new( DEFAULT_CAPACITY );
    }

    public HashStack( int capacity )
    {
        if ( capacity < 0 )
            throw new ArgumentOutOfRangeException( "Capacity must be > 0" );

        _keys = new( capacity );
        _vals = new( capacity );
    }

    public HashStack( IEnumerable<T> collection ) 
    {
        if ( collection == null ) 
            throw new ArgumentNullException( "IEnumerable cannot be null" );

        var c = collection as ICollection<T>;
        if ( c != null )
        {
            _keys = new( c );
            _vals = new( c );
        }
        else
        {
            _keys = new( DEFAULT_CAPACITY );
            _vals = new( DEFAULT_CAPACITY );

            using( IEnumerator<T> en = collection.GetEnumerator() )
            {
                while ( en.MoveNext() )
                    Push( en.Current );
            }
        }
    }

    public void Clear()
    {
        _keys.Clear();
        _vals.Clear();
    }

    public bool Contains( T item ) => _keys.Contains( item );

    public void CopyTo( T[] array, int arrayIndex )
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()   => _vals.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public T Peek() => _vals.Peek();

    public T Pop()
    {
        var item = _vals.Pop();
        _keys.Remove( item );
        return item;
    }

    public void Push( T item )
    {
        if ( Contains( item ) )
            return;

        _keys.Add( item );
        _vals.Push( item );
    }

    public T[] ToArray() => _vals.ToArray();
    
    public void TrimExcess()
    {
        _keys.TrimExcess();
        _vals.TrimExcess();
    }

    public bool TryPeek( out T item ) => _vals.TryPeek( out item );
    public bool  TryPop( out T item ) => _vals.TryPop( out item );
}
